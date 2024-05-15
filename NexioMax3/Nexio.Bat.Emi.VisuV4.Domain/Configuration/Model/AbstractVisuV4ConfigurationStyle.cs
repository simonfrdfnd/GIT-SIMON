namespace NexioMax3.Domain.Configuration.Model
{
  using Newtonsoft.Json;

  public abstract class AbstractVisuV4ConfigurationStyle : Nexio.Base.ModelBase
  {
    [JsonProperty(PropertyName = "Color")]
    public string Color { get; set; }

    [JsonProperty(PropertyName = "Visible")]
    public bool IsVisible { get; set; } = true;

    [JsonIgnore]
    public bool OverrideIsVisible { get; set; } = false;

    [JsonProperty(PropertyName = "Size")]
    public double Size { get; set; }
  }
}