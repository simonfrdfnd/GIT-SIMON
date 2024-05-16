namespace NexioMax3.Domain.Model
{
  using System;
  using Newtonsoft.Json;
  using Nexio.Bat.Common.Domain.Infrastructure;

  [IgnoreT4Generation]
  [JsonObject(MemberSerialization.OptIn)]
  public class Marker
  {
    [JsonProperty(PropertyName = "Frequency")]
    public double Frequency { get; set; }

    [JsonProperty(PropertyName = "SubRangeID")]
    public Guid SubRangeID { get; set; }

    [JsonProperty(PropertyName = "FunctionID")]
    public int FunctionID { get; set; }

    [JsonProperty(PropertyName = "Text")]
    public string Text { get; set; }

    [JsonProperty(PropertyName = "AssociatedCurveName")]
    public string AssociatedCurveName { get; set; }

    [JsonProperty(PropertyName = "Position")]
    public (double X, double Y) Position { get; set; }
  }
}