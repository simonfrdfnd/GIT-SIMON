namespace Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model
{
  using System.Windows;
  using Newtonsoft.Json;

  public class FontSetting
  {
    [JsonProperty(nameof(FontFamily), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string FontFamily { get; set; } = "Segoe UI";

    [JsonProperty(nameof(FontWeight), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int FontWeight { get; set; } = FontWeights.Normal.ToOpenTypeWeight();

    [JsonProperty(nameof(FontSize), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int FontSize { get; set; } = 14;
  }
}