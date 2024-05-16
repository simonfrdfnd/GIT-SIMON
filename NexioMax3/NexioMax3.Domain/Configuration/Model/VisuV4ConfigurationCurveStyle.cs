namespace NexioMax3.Domain.Configuration.Model
{
  using Newtonsoft.Json;

  public class VisuV4ConfigurationCurveStyle : AbstractVisuV4ConfigurationStyle
  {
    [JsonIgnore]
    public bool OverrideColor { get; set; } = false;

    [JsonIgnore]
    public bool OverrideSize { get; set; } = false;
  }
}