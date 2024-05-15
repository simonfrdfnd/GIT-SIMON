namespace Nexio.Bat.Emi.VisuV4.Domain.Engine
{
  using System.Collections.Generic;
  using Nexio.Bat.Emi.VisuV4.Domain.Model;

  public class SubRangeSettings
  {
    public int SubrangeId { get; set; }

    public List<string> Reglages { get; } = new List<string>();

    public TypesProgression TypeProgression { get; set; }

    public double Step { get; set; }
  }
}