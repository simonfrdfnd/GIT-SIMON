namespace NexioMax3.Domain.Configuration.Model.EMIOverrides2
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using Newtonsoft.Json;

  public class SourcesCache
  {
    [Obsolete("Utilisé pour la retrocompatibilité avec l'ancienne forme de stockage")]
    public List<string> Sources { get; set; } = new List<string>();

    public Dictionary<string, DatasCache> SourceDic { get; set; } = new Dictionary<string, DatasCache>();

    public static void Append(string source, List<string> columns)
    {
      var cached = GetSources();
      if (!cached.ContainsKey(source))
      {
        if (!columns.Contains(Properties.Resources.All))
        {
          columns.Insert(0, Properties.Resources.All);
        }

        cached.Add(source, new DatasCache() { Columns = columns });
      }
      else if (!cached[source].Columns.SequenceEqual(columns))
      {
        cached[source].Columns.AddRange(columns);
        cached[source].Columns = cached[source].Columns.Distinct().ToList();
      }

      if (!Directory.Exists(Directories.AppDataDirectory))
      {
        Directory.CreateDirectory(Directories.AppDataDirectory);
      }

      if (File.Exists(Directories.SourceListFile))
      {
        File.Delete(Directories.SourceListFile);
      }

      var srcCached = new SourcesCache() { SourceDic = cached };

      File.WriteAllText(Directories.SourceListFile, JsonConvert.SerializeObject(srcCached));
    }

    public static void Append(KeyValuePair<string, DatasCache> keyValue)
    {
      Append(keyValue.Key, keyValue.Value.Columns);
    }

    public static void Append(string source, string column)
    {
      var temp = new List<string>() { column };
      Append(source, new List<string>() { column });
    }

    public static Dictionary<string, DatasCache> GetSources()
    {
      if (!Directory.Exists(Directories.AppDataDirectory))
      {
        Directory.CreateDirectory(Directories.AppDataDirectory);
      }

      if (!File.Exists(Directories.SourceListFile))
      {
        return InitSourceFile();
      }

      var srcCached = JsonConvert.DeserializeObject<SourcesCache>(File.ReadAllText(Directories.SourceListFile)) ?? new SourcesCache();
      // Utilisé pour la retrocompatibilité avec l'ancienne forme de stockage
      if (srcCached.Sources.Any())
      {
        foreach (var src in srcCached.Sources)
        {
          var data = new DatasCache() { Columns = new List<string>() { Properties.Resources.All } };
          srcCached.SourceDic.Add(src, data);
        }
      }

      return srcCached.SourceDic;
    }

    public static void Clear()
    {
      if (Directory.Exists(Directories.AppDataDirectory))
      {
        if (File.Exists(Directories.SourceListFile))
        {
          File.Delete(Directories.SourceListFile);
        }

        if (File.Exists(Directories.ColumnListFile))
        {
          File.Delete(Directories.ColumnListFile);
        }
      }
    }

    private static Dictionary<string, DatasCache> InitSourceFile()
    {
      var sourceDic = new Dictionary<string, DatasCache>();
      List<string> columns = new List<string>() { Properties.Resources.All };
      DatasCache dataCache = new DatasCache() { Columns = columns };
      sourceDic.Add("Manual", dataCache);
      sourceDic.Add("Max Hold Manual meas.", dataCache);
      sourceDic.Add("Suspects", dataCache);
      sourceDic.Add("Finals", dataCache);
      sourceDic.Add("Peak/Lim.Peak", dataCache);
      sourceDic.Add("Peak/Lim.Q-Peak", dataCache);
      sourceDic.Add("Peak/Lim.Avg", dataCache);
      sourceDic.Add("Peak/Lim.RMS", dataCache);
      sourceDic.Add("Peak/Lim.CAVG", dataCache);
      sourceDic.Add("Peak/Lim.CRMS", dataCache);
      sourceDic.Add("Peak", dataCache);
      sourceDic.Add("Avg", dataCache);
      sourceDic.Add("QuasiPeak", dataCache);
      sourceDic.Add("CAvg", dataCache);
      sourceDic.Add("RMS", dataCache);
      sourceDic.Add("CRMS", dataCache);
      return sourceDic;
    }
  }
}