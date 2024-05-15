namespace Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model
{
  using Newtonsoft.Json;

  public class PageSetupConfig
  {
    [JsonProperty(nameof(Header), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Header { get; set; } = string.Empty;

    [JsonProperty(nameof(Footer), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Footer { get; set; } = string.Empty;
  }
}
