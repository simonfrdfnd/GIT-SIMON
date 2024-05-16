namespace NexioMax3.Domain.Configuration.Model
{
  using Newtonsoft.Json;

  public class VisuV4ConfigurationSuspectStyle : AbstractVisuV4ConfigurationStyle
  {
    [JsonProperty(PropertyName = "Symbol")]
    public string Symbol { get; set; }

    [JsonProperty(PropertyName = "SymbolFilled")]
    public bool SymbolFilled { get; set; }

    [JsonProperty(PropertyName = "ProjectionLine")]
    public bool ProjectionLine { get; set; }

    [JsonIgnore]
    public bool OverrideColor { get; set; } = false;

    [JsonIgnore]
    public bool OverrideSymbol { get; set; } = false;

    [JsonIgnore]
    public bool OverrideSize { get; set; } = false;

    [JsonIgnore]
    public bool OverrideUseProjection { get; set; } = false;
  }
}