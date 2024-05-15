namespace Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model
{
  using System;
  using Newtonsoft.Json;

  public class ManualPointComment
  {
    [JsonProperty(nameof(ProjectId), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Guid ProjectId { get; set; }

    [JsonProperty(nameof(Comments), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string[] Comments { get; set; } = new string[0];
  }
}