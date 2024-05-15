namespace Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model
{
  using System;
  using System.IO;
  using Newtonsoft.Json;
  using Nexio.Bat.Common.Domain.Infrastructure.Service;
  using Nexio.Bat.Emi.VisuV4.Domain;

  public class VisuV4Config
  {
    public bool fileExists;

    private static VisuV4Config instance;

    private VisuV4Config()
    {
    }

    public event EventHandler ConfigurationChanged;

    public static VisuV4Config Instance => instance ?? (instance = Load());

    [JsonIgnore]
    public bool IsFromReferential { get; private set; }

    [JsonProperty(nameof(SkipIntro))]
    public bool SkipIntro { get; set; }

    [JsonProperty(nameof(IsDark))]
    public bool IsDark { get; set; }

    [JsonProperty(nameof(TableFrequencySignificantFigures), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public byte TableFrequencySignificantFigures { get; set; } = 9;

    [JsonProperty(nameof(TableValuesSignificantFigures), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public byte TableValuesSignificantFigures { get; set; } = 3;

    [JsonProperty(nameof(AxesFrequencySignificantFigures), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public byte AxesFrequencySignificantFigures { get; set; } = 9;

    [JsonProperty(nameof(AxesValuesSignificantFigures), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public byte AxesValuesSignificantFigures { get; set; } = 3;

    [JsonProperty(nameof(FontSetting), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public FontSetting FontSetting { get; set; } = new FontSetting();

    [JsonProperty(nameof(ManualPoint), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public ManualPointConfig ManualPoint { get; set; } = new ManualPointConfig();

    [JsonProperty(nameof(Chart), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public ChartConfig Chart { get; set; } = new ChartConfig();

    [JsonProperty(nameof(SuspectTables), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public SuspectTablesConfig SuspectTables { get; set; } = new SuspectTablesConfig();

    [JsonProperty(nameof(PageSetup), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public PageSetupConfig PageSetup { get; set; } = new PageSetupConfig();

    /// <summary>
    /// Forme du suspect OU du final selectionné
    /// </summary>
    public string SelectedSuspectMarkerType { get; set; } = "Diamond";

    /// <summary>
    /// Couleur du suspect OU du final selectionné
    /// </summary>
    public string SelectedSuspectColor { get; set; } = "Red";

    public static void Reload()
    {
      VisuV4Config.instance = VisuV4Config.Load();
    }

    public void Save()
    {
      if (File.Exists(Directories.AppConfigFile))
      {
        File.Delete(Directories.AppConfigFile);
      }

      File.WriteAllText(Directories.AppConfigFile, JsonConvert.SerializeObject(this));
    }

    public void RaiseConfigurationChanged()
    {
      this.ConfigurationChanged?.Invoke(this, EventArgs.Empty);
    }

    private static VisuV4Config Load()
    {
      var referentialDirectory = OptionProvider.GetOptionPath(OptionProvider.Options.Archives);
      var file = Path.Combine(referentialDirectory, Directories.VisuV4ReferentialName);

      if (File.Exists(file))
      {
        var instance = JsonConvert.DeserializeObject<VisuV4Config>(File.ReadAllText(file));
        instance.fileExists = true;
        instance.IsFromReferential = true;
        return instance;
      }

      if (!File.Exists(Directories.AppConfigFile))
      {
        return new VisuV4Config() { fileExists = false };
      }
      else
      {
        var instance = JsonConvert.DeserializeObject<VisuV4Config>(File.ReadAllText(Directories.AppConfigFile));
        instance.fileExists = true;
        return instance;
      }
    }
  }
}