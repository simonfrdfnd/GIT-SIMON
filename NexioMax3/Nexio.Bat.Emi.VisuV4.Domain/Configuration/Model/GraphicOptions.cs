namespace NexioMax3.Domain.Configuration.Model
{
  using System.Collections.Generic;
  using Newtonsoft.Json;

  [JsonObject(MemberSerialization.OptIn)]
  public class GraphicOptions : Nexio.Base.ModelBase
  {
    public string BackgroundColor { get; set; }

    public string GridStyle { get; set; }

    public string GridColor { get; set; }

    [JsonProperty(PropertyName = "Curves")]
    public List<ConfigurationStyle> Curves { get; set; } = new List<ConfigurationStyle>();

    [JsonProperty(PropertyName = "Suspects")]
    public List<ConfigurationStyle> Suspects { get; set; } = new List<ConfigurationStyle>();

    [JsonProperty(PropertyName = "Finals")]
    public List<ConfigurationStyle> Finals { get; set; } = new List<ConfigurationStyle>();

    [JsonProperty(PropertyName = "Limits")]
    public List<ConfigurationStyle> Limits { get; set; } = new List<ConfigurationStyle>();

    [JsonProperty(PropertyName = "ExternalCurves")]
    public List<ConfigurationStyle> ExternalCurves { get; set; } = new List<ConfigurationStyle>();
  }
}
