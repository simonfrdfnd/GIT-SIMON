namespace Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model
{
  using Newtonsoft.Json;

  public abstract class AbstractVisuV4ConfigurationStyle : Base.ModelBase
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