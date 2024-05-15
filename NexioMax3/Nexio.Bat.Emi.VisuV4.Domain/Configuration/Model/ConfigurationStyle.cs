namespace NexioMax3.Domain.Configuration.Model
{
  using global::NexioMax3.Domain.Model;
  using Newtonsoft.Json;

  [JsonObject(MemberSerialization.OptIn)]
  public class ConfigurationStyle : Nexio.Base.ModelBase
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
