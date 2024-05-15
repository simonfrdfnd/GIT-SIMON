namespace Nexio.Bat.Emi.VisuV4.Domain.Model
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using NexioMax3.Model;

  public class SubRange
  {
    public string Name { get; set; }

    public double FStep { get; set; }

    /// <summary>
    /// Fréquence max en Hz
    /// </summary>
    public double FMax { get; set; }

    /// <summary>
    /// Fréquence Min en Hz
    /// </summary>
    public double FMin { get; set; }

    public TypesProgression ProgressionType { get; set; }

    public int Count { get; set; }

    public string PositionScriptName { get; set; }

    public int PositionScriptId { get; set; }

    public string PositionTypeName { get; set; }

    public int Id { get; set; }

    public Guid GuidSB { get; set; }

    public SB_State State { get; set; }

    public List<EMISousBandeFonction> FunctionList { get; } = new List<EMISousBandeFonction>();

    public ConcurrentBag<Prescan> PrescanList { get; } = new ConcurrentBag<Prescan>();

    public List<Suspect> SuspectList { get; } = new List<Suspect>();

    public List<Final> FinalList { get; } = new List<Final>();
  }
}