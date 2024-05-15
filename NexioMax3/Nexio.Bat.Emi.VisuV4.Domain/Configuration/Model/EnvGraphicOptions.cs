namespace Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model
{
  using System.Collections.Generic;
  using System.Linq;
  using Newtonsoft.Json;

  [JsonObject(MemberSerialization.OptIn)]
  public class EnvGraphicOptions
  {
    public string BackgroundColor { get; set; }

    public string GridStyle { get; set; }

    public string GridColor { get; set; }

    [JsonProperty(PropertyName = "Curves")]
    public List<EnvCurveConfigurationStyle> Curves { get; set; } = new List<EnvCurveConfigurationStyle>();

    [JsonProperty(PropertyName = "Suspects")]
    public List<EnvSuspectConfigurationStyle> Suspects { get; set; } = new List<EnvSuspectConfigurationStyle>();

    [JsonProperty(PropertyName = "Finals")]
    public List<EnvSuspectConfigurationStyle> Finals { get; set; } = new List<EnvSuspectConfigurationStyle>();

    [JsonProperty(PropertyName = "Limits")]
    public List<EnvCurveConfigurationStyle> Limits { get; set; } = new List<EnvCurveConfigurationStyle>();

    [JsonProperty(PropertyName = "ExternalCurves")]
    public List<EnvCurveConfigurationStyle> ExternalCurves { get; set; } = new List<EnvCurveConfigurationStyle>();

    public static EnvGraphicOptions FromGraphicOptions(GraphicOptions options)
    {
      var envgo = new EnvGraphicOptions()
                  {
                    BackgroundColor = options.BackgroundColor,
                    GridColor = options.GridColor,
                    GridStyle = options.GridStyle,
                    Curves = options.Curves.Select(EnvCurveConfigurationStyle.From).ToList(),
                    ExternalCurves = options.ExternalCurves.Select(EnvCurveConfigurationStyle.From).ToList(),
                    Limits = options.Limits.Select(EnvCurveConfigurationStyle.From).ToList(),
                    Suspects = options.Suspects.Select(EnvSuspectConfigurationStyle.From).ToList(),
                    Finals = options.Finals.Select(EnvSuspectConfigurationStyle.From).ToList(),
                  };

      return envgo;
    }
  }
}