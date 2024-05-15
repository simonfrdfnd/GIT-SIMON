namespace NexioMax3.Domain.Service
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using NexioMax3.Domain.Engine;
  using NexioMax3.Domain.Model;
  using NexioMax3.Domain.Model.Definition;
  using NexioMax3.Domain.Repository;

  public class SubRangeDefinitionHelper
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SubRangeDefinitionHelper));

    private static Dictionary<string, int> prescanDllNameId = new Dictionary<string, int>();

    static SubRangeDefinitionHelper()
    {
      prescanDllNameId.Add("Prescan.dll", 1);
      prescanDllNameId.Add("Prescan_Global.dll", 32);
      prescanDllNameId.Add("Prescan_Mil.dll", 2);
      prescanDllNameId.Add("Prescan_Correlation.dll", 29);
      prescanDllNameId.Add("CRBM_EMI_TEST.dll", 16);
    }

    public SubRangeDefinitionHelper()
    {
    }

    public static Func<Task> DefinitionChanged { private get; set; }

    public static async Task ReloadTest(Test test)
    {
      await Task.Run(() =>
      {
        Provider.Instance.BatData.Terminer((Domain.Model.Test_State)(-1));
        bool ret = Provider.Instance.BatData.Initialiser(test.TestId.ToString("B"), true, test.NumEssai, test.ResultPath, false);
      });

      await DefinitionChanged();
    }

    public static void Rollback(List<Guid> srGuids)
    {
      foreach (var sr in srGuids)
      {
        SubRangeProvider.Instance.DeleteSubrange(sr);
      }
    }

    public List<Guid> Duplicate(double fmin, double fmax, List<Guid> ids, List<string> comments, List<SubRangeSettings> settings, string dllName)
    {
      int dllId = GetDllId(dllName);
      var copy = new List<Guid>();

      try
      {
        // requests original subranges
        var subranges = new Dictionary<Guid, GSousBandes>();
        var sbFunctions = new Dictionary<Guid, List<EMISousBandeFonction>>();
        var sbFunctionParams = new Dictionary<Guid, List<EMIFonctionParam>>();
        var sbReglages = new Dictionary<Guid, List<EMIReglageParam>>();
        foreach (var sb in ids)
        {
          if (SubRangeProvider.Instance.GetSubrange(sb, out GSousBandes request))
          {
            subranges.Add(sb, request);
            sbFunctions[sb] = new List<EMISousBandeFonction>();
            foreach (var item in SubRangeProvider.Instance.GetFunctions(sb))
            {
              // On ne conserve que les prescans
              if (prescanDllNameId.Values.Contains(item.IdDLL))
              {
                sbFunctions[sb].Add(item);
              }
            }

            sbFunctionParams[sb] = new List<EMIFonctionParam>();
            foreach (var item in SubRangeProvider.Instance.GetFunctionParams(sb))
            {
              // On ne conserve que les prescans
              if (prescanDllNameId.Values.Contains(item.IdDLL))
              {
                sbFunctionParams[sb].Add(item);
              }
            }

            sbReglages[sb] = new List<EMIReglageParam>();
            foreach (var item in SubRangeProvider.Instance.GetReglages(sb))
            {
              sbReglages[sb].Add(item);
            }
          }
        }

        // Update Guid + state + bornes
        foreach (var item in subranges)
        {
          var newGuid = Guid.NewGuid();
          item.Value.GuidSousBande = newGuid;
          item.Value.EtatExecution = (int)Domain.Model.SB_State.SB_NON_COMMENCEE;
          item.Value.FreqMin = Math.Max(item.Value.FreqMin, fmin);
          item.Value.FreqMax = Math.Min(item.Value.FreqMax, fmax);
          foreach (var fn in sbFunctions[item.Key])
          {
            fn.GuidSousBande = newGuid;
          }

          foreach (var par in sbFunctionParams[item.Key])
          {
            par.GuidSousBande = newGuid;
          }

          foreach (var rg in sbReglages[item.Key])
          {
            rg.GuidSousBande = newGuid;
          }
        }

        // MàJ des paramètres en cas de changement de fonction
        foreach (var item in subranges)
        {
          foreach (var fn in sbFunctions[item.Key])
          {
            // on change de fonction par rapport à la definition
            if (fn.IdDLL != dllId)
            {
              fn.IdDLL = dllId;
              sbFunctionParams[item.Key].Clear(); // !!!Beaucoup trop violent, mais devrait fonctionner tant qu'on ne fait qu'un prescan
            }
          }
        }

        // Update des commentaires
        for (int i = 0; i < ids.Count; i++)
        {
          subranges[ids[i]].Commentaire = comments[i];
        }

        // Update des settings
        for (int i = 0; i < ids.Count; i++)
        {
          var newsettings = settings[i];
          subranges[ids[i]].ValeurPasFreq = newsettings.Step;
          subranges[ids[i]].TypeProgression = (int)newsettings.TypeProgression;
          var modify = sbReglages[ids[i]];

          for (int j = 0; j < newsettings.Reglages.Count; j++)
          {
            var rp = modify.FirstOrDefault(r => r.IndexParam == j);
            if (rp != null)
            {
              rp.ValParam = newsettings.Reglages[j];
            }
          }
        }

        // Sauvegarde la sous bande
        foreach (var sb in subranges)
        {
          if (SubRangeProvider.Instance.CreateSubrange(sb.Key, sb.Value))
          {
            foreach (var fn in sbFunctions[sb.Key])
            {
              SubRangeProvider.Instance.CreateFunction(fn);
            }

            foreach (var par in sbFunctionParams[sb.Key])
            {
              SubRangeProvider.Instance.CreateFunctionParams(par);
            }

            foreach (var rg in sbReglages[sb.Key])
            {
              SubRangeProvider.Instance.CreateReglage(rg);
            }
          }

          copy.Add(sb.Value.GuidSousBande);
        }
      }
      catch (Exception ex)
      {
        // TODO : Log + rollback
        Log.Error("Fail to duplicate subranges");
        Log.Error(ex);
        Rollback(copy);
        copy.Clear();
      }

      return copy;
    }

    internal static void SaveFunctionParameters(string nomDllManu, Guid guidsb, List<string> parameters)
    {
      var toSave = new List<EMIFonctionParam>();
      int idDll = GetDllId(nomDllManu);
      for (int i = 0; i < parameters.Count; i++)
      {
        toSave.Add(new EMIFonctionParam()
        {
          GuidSousBande = guidsb,
          IdDLL = idDll,
          IndexParam = i,
          ValParam = parameters[i],
        });
      }

      SubRangeProvider.Instance.DeleteFunctionParams(guidsb, idDll);

      foreach (var par in toSave)
      {
        SubRangeProvider.Instance.CreateFunctionParams(par);
      }
    }

    private static int GetDllId(string nomDll)
    {
      if (prescanDllNameId.ContainsKey(nomDll))
      {
        return prescanDllNameId[nomDll];
      }
      else
      {
        throw new Exception("Unhandled dll " + nomDll);
      }
    }
  }
}