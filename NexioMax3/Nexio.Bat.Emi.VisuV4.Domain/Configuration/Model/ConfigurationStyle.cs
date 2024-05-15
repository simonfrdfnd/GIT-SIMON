namespace Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model
{
  using Newtonsoft.Json;
  using Nexio.Bat.Emi.VisuV4.Domain.Model;

  [JsonObject(MemberSerialization.OptIn)]
  public class ConfigurationStyle : Base.ModelBase
  {
    public ConfigurationStyle()
    {
    }

    public ConfigurationStyle(CurveTag tag, AbstractVisuV4ConfigurationStyle style)
    {
      this.Tag = tag;
      this.Style = style;
    }

    [JsonProperty(PropertyName = "Tag")]
    public CurveTag Tag { get; set; }

    [JsonProperty(PropertyName = "Style")]
    public AbstractVisuV4ConfigurationStyle Style { get; set; }
  }
}
