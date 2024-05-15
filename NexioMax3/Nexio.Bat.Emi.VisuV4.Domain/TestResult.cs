namespace Nexio.Bat.Emi.VisuV4.Domain
{
  using System;
  using System.Collections.Generic;
  using Newtonsoft.Json;
  using Nexio.Bat.Common.Domain.Infrastructure;
  using Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model;
  using Nexio.Bat.Emi.VisuV4.Domain.Model;

  [IgnoreT4Generation]
  [JsonObject(MemberSerialization.OptIn)]
  public class TestResult : IEntityBase
  {
    private static readonly Version CurrentModelVersion = new Version("1.0.0.0");

    [JsonIgnore]
    public Guid ID { get; set; }

    [JsonProperty(PropertyName = "Options")]

    /// <summary>
    /// Guid du prescan,
    /// </summary>
    public GraphicOptions Options { get; set; } = new GraphicOptions();

    [JsonProperty(PropertyName = "MarkersList")]
    public List<Marker> MarkersList { get; private set; } = new List<Marker>();

    [JsonProperty(PropertyName = "YLimits")]
    public YAxisLimit YLimits { get; set; } = new YAxisLimit();

    [JsonProperty(nameof(VisibleLimits))]
    public List<LimitIdentifier> VisibleLimits { get; set; } = new List<LimitIdentifier>();

    [JsonProperty(nameof(PlotSettings))]
    public PlotSettings PlotSettings { get; set; } = new PlotSettings();

    [JsonProperty(nameof(LastFilter))]
    public Filter LastFilter { get; set; } = Filter.BySubRange;
  }
}