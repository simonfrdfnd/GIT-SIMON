namespace Nexio.Bat.Emi.VisuV4.Domain.Repository
{
  using System;
  using System.Collections.Generic;
  using System.Data.OleDb;
  using System.Linq;
  using Nexio.Bat.Common.Domain.ATDB.Service;
  using Nexio.Bat.Common.Domain.Infrastructure;
  using Nexio.Bat.Common.Domain.Infrastructure.AccessDataBase;
  using Nexio.Bat.Emi.VisuV4.Domain.Engine;
  using Nexio.Bat.Emi.VisuV4.Domain.Model.Definition;
  using Nexio.Bat.Emi.VisuV4.Domain.Service;

  public class SubRangeProvider
  {
    private static SubRangeProvider intance;

    private SubRangeProvider()
    {
    }

    public static SubRangeProvider Instance
    {
      get
      {
        return intance ?? (intance = new SubRangeProvider());
      }
    }

    internal void DeleteSubrange(Guid guidsb)
    {
      // delete in EMI_SousBandeFonction
      var cmd = string.Format(@"DELETE FROM EMI_SousBandeFonction WHERE guidSousBande={0}", guidsb.ToSQL());
      DataBase.Instance.ExecuteNoQuery(cmd);

      // delete in EMI_FonctionParam
      cmd = string.Format(@"DELETE FROM EMI_FonctionParam WHERE guidSousBande={0}", guidsb.ToSQL());
      DataBase.Instance.ExecuteNoQuery(cmd);

      // delete in EMI_ReglageParam
      cmd = string.Format(@"DELETE FROM EMI_ReglageParam WHERE guidSousBande={0}", guidsb.ToSQL());
      DataBase.Instance.ExecuteNoQuery(cmd);

      // delete in g_SousBandes
      cmd = string.Format(@"DELETE FROM g_SousBandes WHERE guidSousBande={0}",
          guidsb.ToSQL());
      DataBase.Instance.ExecuteNoQuery(cmd);

      // delete in g_SousBandes
      ATDBProvider.Instance.DeleteObject(guidsb);
    }

    internal IEnumerable<EMISousBandeFonction> GetFunctions(Guid sb)
    {
      var request = string.Format("SELECT * FROM EMI_SousBandeFonction WHERE guidSousBande={0}", sb.ToSQL());
      using (OleDbDataReader objectReader = DataBase.Instance.ExecuteReader(request))
      {
        while (objectReader.Read())
        {
          yield return new EMISousBandeFonction()
          {
            GuidSousBande = objectReader.GetGuid(objectReader.GetOrdinal("guidSousBande")),
            IdDLL = objectReader.GetInt32(objectReader.GetOrdinal("idDll")),
            NumOrdre = objectReader.GetInt16(objectReader.GetOrdinal("iNumOrdre")),
            GuidConfig = objectReader.GetGuidSafely("guidConfig"),
          };
        }
      }
    }

    internal IEnumerable<EMIFonctionParam> GetFunctionParams(Guid sb)
    {
      var request = string.Format("SELECT * FROM EMI_FonctionParam WHERE guidSousBande={0}", sb.ToSQL());
      using (OleDbDataReader objectReader = DataBase.Instance.ExecuteReader(request))
      {
        while (objectReader.Read())
        {
          yield return new EMIFonctionParam()
          {
            GuidSousBande = objectReader.GetGuid(objectReader.GetOrdinal("guidSousBande")),
            IdDLL = objectReader.GetInt32(objectReader.GetOrdinal("idDll")),
            IndexParam = objectReader.GetInt32(objectReader.GetOrdinal("lIndexParam")),
            ValParam = objectReader["sValParam"].ToString(),
          };
        }
      }
    }

    internal IEnumerable<EMIReglageParam> GetReglages(Guid sb)
    {
      var request = string.Format("SELECT * FROM EMI_ReglageParam WHERE guidSousBande={0}", sb.ToSQL());
      using (OleDbDataReader objectReader = DataBase.Instance.ExecuteReader(request))
      {
        while (objectReader.Read())
        {
          yield return new EMIReglageParam()
          {
            GuidSousBande = objectReader.GetGuid(objectReader.GetOrdinal("guidSousBande")),
            IndexParam = objectReader.GetInt32(objectReader.GetOrdinal("lIndexParam")),
            ValParam = objectReader["sValParam"].ToString(),
            Nom = objectReader["sNom"].ToString(),
            Valeur = objectReader["sValeur"].ToString(),
          };
        }
      }
    }

    internal bool GetSubrange(Guid id, out GSousBandes sb)
    {
      var request = string.Format("SELECT * FROM g_SousBandes WHERE guidSousBande={0}", id.ToSQL());
      using (OleDbDataReader objectReader = DataBase.Instance.ExecuteReader(request))
      {
        if (objectReader.Read())
        {
          sb = new GSousBandes()
          {
            GuidSousBande = objectReader.GetGuid(objectReader.GetOrdinal("guidSousBande")),
            GuidEssai = objectReader.GetGuid(objectReader.GetOrdinal("guidEssai")),
            FreqMin = objectReader.GetDouble(objectReader.GetOrdinal("FreqMin")),
            FreqMax = objectReader.GetDouble(objectReader.GetOrdinal("FreqMax")),
            Commentaire = objectReader["sCommentaire"].ToString(),
            ValeurPasFreq = objectReader.GetDouble(objectReader.GetOrdinal("ValeurPasFreq")),
            TypeProgression = objectReader.GetInt32(objectReader.GetOrdinal("iTypeProgression")),
            FMinInclue = objectReader.GetBoolean(objectReader.GetOrdinal("bFMinInclue")),
            FMaxInclue = objectReader.GetBoolean(objectReader.GetOrdinal("bFMaxInclue")),
            Distance = objectReader.GetDouble(objectReader.GetOrdinal("fDistance")),
            GuidDomaine = objectReader.GetGuid(objectReader.GetOrdinal("guidDomaine")),
            GuidDomaineFinal = objectReader.GetGuidSafely("guidDomaineFinal"),
            ScriptPosition = objectReader.GetInt16(objectReader.GetOrdinal("iScriptPosition")),
            ScriptNomCustomLine = objectReader["sNomCustomLine"]?.ToString(),
            EtatExecution = objectReader.GetInt16(objectReader.GetOrdinal("iEtatExecution")),
            HauteurAuto = objectReader.GetBoolean(objectReader.GetOrdinal("bHauteurAuto")),
            Hauteur = objectReader.GetDouble(objectReader.GetOrdinal("fHauteur")),
            AngleAuto = objectReader.GetBoolean(objectReader.GetOrdinal("bAngleAuto")),
            Angle = objectReader.GetDouble(objectReader.GetOrdinal("fAngle")),
            ScriptDebutSB = objectReader["sScriptDebutSB"].ToString(),
            ScriptFinSB = objectReader["sScriptFinSB"].ToString(),
            GlobalPhase = objectReader.GetInt32Nullable("iGlobalPhase"),
            GlobalPhaseStep = objectReader.GetInt32Nullable("iGlobalPhaseStep"),
          };

          return true;
        }
      }

      sb = null;
      return false;
    }

    internal bool CreateSubrange(Guid original, GSousBandes sb)
    {
      // create in g_ListObjects
      var atdb = ATDBProvider.Instance.ReadObject(original);
      var parent = ATDBProvider.Instance.ReadGuidParent(original);
      atdb.GuidObject = sb.GuidSousBande;
      atdb.Name = sb.Commentaire;
      ATDBProvider.Instance.CreateObject(atdb, parent);

      // create in g_SousBandes
      var cmd = string.Format(
        @"INSERT INTO g_SousBandes
        (guidSousBande,guidEssai,FreqMin,FreqMax,ValeurPasFreq,
          iTypeProgression,iEtatExecution,bFMinInclue,bFMaxInclue,guidDomaine,
          bHauteurAuto,fHauteur,bAngleAuto,fAngle,iScriptPosition,
          fDistance,sScriptDebutSB,sScriptFinSB,guidDomaineFinal,sCommentaire,
          iGlobalPhase,iGlobalPhaseStep,sNomCustomLine)
        VALUES
        ({0}, {1}, {2}, {3}, {4},
          {5}, {6}, {7}, {8}, {9},
          {10}, {11}, {12}, {13}, {14},
          {15}, {16}, {17}, {18}, {19},
          {20}, {21}, {22})",
        sb.GuidSousBande.ToSQL(), sb.GuidEssai.ToSQL(), sb.FreqMin.ToSQL(), sb.FreqMax.ToSQL(), sb.ValeurPasFreq.ToSQL(),
        sb.TypeProgression.ToSQL(), sb.EtatExecution.ToSQL(), sb.FMinInclue.ToSQL(), sb.FMaxInclue.ToSQL(), sb.GuidDomaine.ToSQL(),
        sb.HauteurAuto.ToSQL(), sb.Hauteur.ToSQL(), sb.AngleAuto.ToSQL(), sb.Angle.ToSQL(), sb.ScriptPosition.ToSQL(),
        sb.Distance.ToSQL(), sb.ScriptDebutSB.ToSQL(), sb.ScriptFinSB.ToSQL(), sb.GuidDomaine.ToSQL(), sb.Commentaire.ToSQL(),
        sb.GlobalPhase.ToSQL(), sb.GlobalPhaseStep.ToSQL(), sb.ScriptNomCustomLine.ToSQL());
      var result = DataBase.Instance.ExecuteNoQuery(cmd) == 1;
      DataBase.Instance.Reconnect();

      return result;
    }

    internal void DeleteFunctionParams(Guid guidsb, int idDll)
    {
      // create in EMI_FonctionParam
      var cmd = string.Format(@"DELETE FROM EMI_FonctionParam  WHERE guidSousBande={0} AND idDLL={1}", guidsb.ToSQL(), idDll.ToSQL());
      DataBase.Instance.ExecuteNoQuery(cmd);
    }

    internal bool CreateFunction(EMISousBandeFonction fn)
    {
      // create in EMI_SousBandeFonction
      var cmd = string.Format(@"INSERT INTO EMI_SousBandeFonction
                                (guidSousBande,idDLL,iNumOrdre,guidConfig)
                                VALUES ({0}, {1}, {2}, {3})",
          fn.GuidSousBande.ToSQL(), fn.IdDLL.ToSQL(), fn.NumOrdre.ToSQL(), fn.GuidConfig.ToSQL());
      return DataBase.Instance.ExecuteNoQuery(cmd) == 1;
    }

    internal bool CreateFunctionParams(EMIFonctionParam par)
    {
      // create in EMI_FonctionParam
      var cmd = string.Format(@"INSERT INTO EMI_FonctionParam
                                (guidSousBande,idDLL,lIndexParam,sValParam)
                                VALUES ({0}, {1}, {2}, {3})",
          par.GuidSousBande.ToSQL(), par.IdDLL.ToSQL(), par.IndexParam.ToSQL(), par.ValParam.ToSQL());
      return DataBase.Instance.ExecuteNoQuery(cmd) == 1;
    }

    internal bool CreateReglage(EMIReglageParam rg)
    {
      // create in EMI_ReglageParam
      var cmd = string.Format(@"INSERT INTO EMI_ReglageParam
                                (guidSousBande,lIndexParam,sValParam,sNom,sValeur)
                                VALUES ({0}, {1}, {2}, {3}, {4})",
          rg.GuidSousBande.ToSQL(), rg.IndexParam.ToSQL(), rg.ValParam.ToSQL(), rg.Nom.ToSQL(), rg.Valeur.ToSQL());
      return DataBase.Instance.ExecuteNoQuery(cmd) == 1;
    }
  }
}