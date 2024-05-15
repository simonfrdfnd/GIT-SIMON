namespace NexioMax3.Domain.Engine
{
  using System.Collections.Generic;
  using NexioMax3.Domain.Model;

  public class SubRangeSettings
  {
    public int SubrangeId { get; set; }

    public List<string> Reglages { get; } = new List<string>();

    public TypesProgression TypeProgression { get; set; }

    public double Step { get; set; }
  }
}