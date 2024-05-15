namespace Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model.EMIOverrides2
{
  using System.Collections.Generic;
  using System.IO;
  using Newtonsoft.Json;
  using Nexio.Bat.Common.Domain.Infrastructure.Service;
  using Nexio.Helper;
  using Nexio.Tools;

  public class EmiOverrideData
  {
    public const string OverrideFile = "Emi_Overrides.json";

    public List<EmiOverrideCurve> Prescans { get; set; } = new List<EmiOverrideCurve>();

    public List<EmiOverrideCurve> Limits { get; set; } = new List<EmiOverrideCurve>();

    public List<EmiOverrideExternalCurve> Externals { get; set; } = new List<EmiOverrideExternalCurve>();

    public List<EmiOverridePoint> Suspects { get; set; } = new List<EmiOverridePoint>();

    public List<EmiOverridePoint> Finals { get; set; } = new List<EmiOverridePoint>();

    public static EmiOverrideData Load(string path = null)
    {
      using (new NexioStopwatch($"{nameof(EmiOverrideData)}.Load"))
      {
        if (path == null)
        {
          path = OptionProvider.GetOptionPath(OptionProvider.Options.EmiOverrideDirectory);
          if (string.IsNullOrWhiteSpace(path))
          {
            path = ExecPathHelper.GetExecDirectory();
          }

          path = Path.Combine(path, OverrideFile);
        }

        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
          return new EmiOverrideData();
        }

        return JsonConvert.DeserializeObject<EmiOverrideData>(File.ReadAllText(path));
      }
    }

    public void Save(string path = null)
    {
      if (path == null)
      {
        var iniDirectory = OptionProvider.GetOptionPath(OptionProvider.Options.EmiOverrideDirectory);

        if (string.IsNullOrWhiteSpace(iniDirectory))
        {
          iniDirectory = ExecPathHelper.GetExecDirectory();
        }

        path = Path.Combine(iniDirectory, OverrideFile);
      }

      if (File.Exists(path))
      {
        File.Delete(path);
      }

      File.WriteAllText(path, JsonConvert.SerializeObject(this));
    }
  }
}