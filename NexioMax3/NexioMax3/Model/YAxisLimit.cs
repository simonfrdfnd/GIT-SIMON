namespace Nexio.Bat.Emi.VisuV4.Domain.Model
{
  using Newtonsoft.Json;
  using Nexio.Bat.Emi.VisuV4.Domain.Engine;

  [JsonObject(MemberSerialization.OptIn)]
  public class YAxisLimit
  {
    [JsonProperty(PropertyName = "AdjustLevel")]
    public bool AdjustLevel { get; set; } = true;

    [JsonProperty(PropertyName = "Range")]
    public AxisRange Range { get; set; } = (0, 1);
  }
}