namespace NexioMax3.Domain.Service
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using log4net;
  using Nexio.Bat.Common.Domain.Infrastructure;
  using Nexio.Bat.Common.Domain.Infrastructure.AccessDataBase;
  using NexioMax3.Domain.Model;
  using Nexio.Helper;

  /// <summary>
  /// Copie de DlgEssai.cpp et n otement de l'algo DeleteResults
  /// </summary>
  public class DlgEssaiEdit
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(DlgEssaiEdit));

    public void DeleteResults(Guid testId, List<Guid> subrangesToRemove)
    {
      if (subrangesToRemove.Count == 0)
      {
        return;
      }

      Guid guidHarm = new Guid("{6D133928-F4B7-4874-A80D-4FD7936E7855}");

      var transac = DataBase.Instance.BeginTransaction();
      try
      {
        this.DeleteResultsSB(testId, subrangesToRemove, transac);

        string resultPath = RegistryHelper.CreateStoragePathForCurrentProject();

        foreach (var sb in subrangesToRemove)
        {
          // supprime les captures de la sous bande
          var toDelete = Directory.EnumerateFiles(resultPath, string.Format(@"{0}*.jpg", sb.ToString("B")));
          foreach (var file in toDelete)
          {
            File.Delete(file);
          }

          // supprime les balayages
          var files = Directory.GetFiles(resultPath);
          var sbStr = sb.ToString("B").ToUpper();
          toDelete = files.Where(s => Path.GetFileName(s).StartsWith(sbStr) && s.EndsWith(".index"));
          foreach (var file in toDelete)
          {
            File.Delete(file);
          }

          toDelete = Directory.GetFiles(resultPath).Where(s => Path.GetFileName(s).StartsWith(sbStr) && s.EndsWith(".dat"));
          foreach (var file in toDelete)
          {
            File.Delete(file);
          }
        }

        // Màj de l'état de l'essai
        var newState = this.GetTestState(testId, transac);
        var req = string.Format(@"UPDATE g_ESSAIS
                                SET sVersion = NULL,
                                sVersionMateriels = NULL,
                                DateExecution = NULL,
                                iEtatExecution = {0}
                                WHERE guidEssai ={1}", ((int)newState).ToSQL(), testId.ToSQL());

        DataBase.Instance.ExecuteNoQuery(transac, req);

        //// GD : Suppression de la liste des ambiants
        var joinSB = "(" + String.Join(", ", subrangesToRemove.Select(g => g.ToSQL())) + ")";
        req = string.Format(@"DELETE FROM EMI_Ambiants
                              WHERE guidEssai={0}
                              AND guidSB IN {1}",
                              testId.ToSQL(),
                              joinSB);

        DataBase.Instance.ExecuteNoQuery(transac, req);

        DataBase.Instance.CommitTransaction(transac);
      }
      catch (Exception e)
      {
        Log.Error(e);
        try
        {
          DataBase.Instance.RollBackTransaction(transac);
        }
        catch (Exception e2)
        {
          Log.Error(e2);
          throw;
        }

        throw;
      }
    }

    private Domain.Model.Test_State GetTestState(Guid testId, System.Data.OleDb.OleDbTransaction transac)
    {
      var req = string.Format(@"SELECT COUNT(guidSousBande) AS NbEnCours
                                FROM g_SousBandes
                                WHERE iEtatExecution<> 0
                                AND guidEssai = {0}", testId.ToSQL());

      using (var reader = DataBase.Instance.ExecuteReader(req, transac))
      {
        if (reader.Read())
        {
          int nbrEnCours = reader.GetInt32(reader.GetOrdinal("NbEnCours"));
          return nbrEnCours > 0 ? Test_State.ESSAI_EN_COURS : Test_State.ESSAI_NON_COMMENCE;
        }
      }

      return Test_State.ESSAI_NON_COMMENCE;
    }

    private void DeleteResultsSB(Guid testId, IEnumerable<Guid> listSB, System.Data.OleDb.OleDbTransaction transac)
    {
      var joinSB = "(" + String.Join(", ", listSB.Select(g => g.ToSQL())) + ")";

      // mise à jour mode exécution des sous-bandes de l'essai
      var req = string.Format(@"UPDATE g_SousBandes
                                SET iEtatExecution = {0}
                                WHERE guidEssai = {1} AND guidSousBande IN {2}",
                                ((int)Domain.Model.Test_State.ESSAI_NON_COMMENCE).ToSQL(),
                                testId.ToSQL(),
                                joinSB);
      DataBase.Instance.ExecuteNoQuery(transac, req);

      if (DataBase.Instance.IsMSSQL)
      {
        // suppression des matériels joints aux sous bandes de l'essai
        req = string.Format(@"DELETE MAT
                            FROM g_SousBandesMateriels MAT
                            INNER JOIN g_SousBandes SBA ON MAT.guidSousBande = SBA.guidSousBande
                            WHERE SBA.guidEssai= {0}
                            AND SBA.guidSousBande IN {1}",
                              testId.ToSQL(),
                              joinSB);
        DataBase.Instance.ExecuteNoQuery(transac, req);

        // suppression des suspects liés aux sous bandes de l'essai
        req = string.Format(@"DELETE FIN
                            FROM EMI_Finaux FIN
                            INNER JOIN g_SousBandes SBA ON FIN.guidSB = SBA.guidSousBande
                            WHERE SBA.guidEssai= {0}
                            AND SBA.guidSousBande IN {1}",
                              testId.ToSQL(),
                              joinSB);
        DataBase.Instance.ExecuteNoQuery(transac, req);
      }
      else
      {
        // suppression des matériels joints aux sous bandes de l'essai
        req = string.Format(@"DELETE DISTINCTROW MAT.*
                            FROM g_SousBandesMateriels MAT
                            INNER JOIN g_SousBandes SBA ON MAT.guidSousBande = SBA.guidSousBande
                            WHERE SBA.guidEssai= {0}
                            AND SBA.guidSousBande IN {1}",
                              testId.ToSQL(),
                              joinSB);
        DataBase.Instance.ExecuteNoQuery(transac, req);

        // suppression des suspects liés aux sous bandes de l'essai
        req = string.Format(@"DELETE DISTINCTROW FIN.*
                            FROM EMI_Finaux FIN
                            INNER JOIN g_SousBandes SBA ON FIN.guidSB = SBA.guidSousBande
                            WHERE SBA.guidEssai= {0}
                            AND SBA.guidSousBande IN {1}",
                              testId.ToSQL(),
                              joinSB);
        DataBase.Instance.ExecuteNoQuery(transac, req);
      }

      // Suppression des screenshot du montage
      string sResultsPath = RegistryHelper.CreateStoragePathForCurrentProject();
      string screenPath;

      foreach (var idSB in listSB)
      {
        screenPath = string.Format("{0}\\{1}_{2}.jpg", sResultsPath, idSB.ToString("B").ToUpper(), 0);

        bool bExist = File.Exists(screenPath);
        int i = 0;
        while (bExist)
        {
          File.Delete(screenPath);
          i++;
          screenPath = string.Format("{0}\\{1}_{2}.jpg", sResultsPath, idSB.ToString("B").ToUpper(), i);
          bExist = File.Exists(screenPath);
        }

        // supprime les logs d'execution
        var exLogPath = Path.Combine(sResultsPath, $"{idSB:B}.exlog");

        if (File.Exists(exLogPath))
        {
          File.Delete(exLogPath);
        }
      }
    }
  }
}