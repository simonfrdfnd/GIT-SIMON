namespace NexioMax3.Domain.Model
{
  using Newtonsoft.Json;
  using NexioMax3.Domain.Engine;

  [JsonObject(MemberSerialization.OptIn)]
  public class YAxisLimit
  {
    [JsonProperty(PropertyName = "AdjustLevel")]
    public bool AdjustLevel { get; set; } = true;

    [JsonProperty(PropertyName = "Range")]
    public AxisRange Range { get; set; } = (0, 1);
  }
}