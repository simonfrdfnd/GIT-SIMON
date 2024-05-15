namespace Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model
{
  using Newtonsoft.Json;
  using Nexio.Bat.Emi.VisuV4.Domain.Model;

  public class EnvSuspectConfigurationStyle
  {
    [JsonProperty(PropertyName = "Tag")]
    public CurveTag Tag { get; set; }

    [JsonProperty(PropertyName = "Style")]
    public VisuV4ConfigurationSuspectStyle Style { get; set; }

    public static EnvSuspectConfigurationStyle From(ConfigurationStyle style)
    {
      if (style.Style is VisuV4ConfigurationSuspectStyle s)
      {
        return new EnvSuspectConfigurationStyle()
               {
                 Tag = style.Tag,
                 Style = s,
               };
      }

      return null;
    }
  }
}