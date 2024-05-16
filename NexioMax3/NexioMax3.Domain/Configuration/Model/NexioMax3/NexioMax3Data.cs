namespace NexioMax3.Domain.Configuration.Model.NexioMax3
{
  using System.Collections.Generic;
  using System.IO;
  using Newtonsoft.Json;
  using Nexio.Bat.Common.Domain;
  using Nexio.Bat.Common.Domain.Infrastructure.Service;
  using Nexio.Helper;
  using Nexio.Tools;

  public class NexioMax3Data
  {
    public const string NexioMax3File = "NexioMax3.json";

    public List<string> Addresses { get; set; } = new List<string>();

    public static NexioMax3Data Load(string path = null)
    {
      using (new NexioStopwatch($"{nameof(NexioMax3Data)}.Load"))
      {
        if (path == null)
        {
          path = OptionProvider.GetOptionPath(OptionProvider.Options.EmiOverrideDirectory);
          if (string.IsNullOrWhiteSpace(path))
          {
            path = ExecPathHelper.GetExecDirectory();
          }

          path = Path.Combine(path, NexioMax3File);
        }

        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
          return new NexioMax3Data();
        }

        return JsonConvert.DeserializeObject<NexioMax3Data>(File.ReadAllText(path));
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

        path = Path.Combine(iniDirectory, NexioMax3File);
      }

      if (File.Exists(path))
      {
        File.Delete(path);
      }

      File.WriteAllText(path, JsonConvert.SerializeObject(this));
    }
  }
}