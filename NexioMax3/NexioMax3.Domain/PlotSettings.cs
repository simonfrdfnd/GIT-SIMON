namespace NexioMax3.Domain
{
  using System;
  using System.Collections.Generic;
  using Newtonsoft.Json;
  using Nexio.Bat.Common.Domain.Infrastructure;
  using NexioMax3.Domain.Configuration.Model;
  using NexioMax3.Domain.Model;

  [IgnoreT4Generation]
  [JsonObject(MemberSerialization.OptIn)]
  public class PlotSettings
  {
    private bool isLogarithmicAxisSetByUser;

    [JsonProperty(nameof(IsLogarithmicAxis))]
    public bool IsLogarithmicAxis { get; set; }

    [JsonProperty(nameof(IsMaxHoldTrace))]
    public bool IsMaxHoldTrace { get; set; } = true;

    [JsonProperty(nameof(IsLogarithmicAxisSetByUser))]
    public bool IsLogarithmicAxisSetByUser
    {
      get => this.isLogarithmicAxisSetByUser;
      set => this.isLogarithmicAxisSetByUser = value;
    }
  }
}