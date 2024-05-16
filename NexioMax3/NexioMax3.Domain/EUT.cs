namespace NexioMax3.Domain
{
  using System;
  using Nexio.Bat.Common.Domain.TestDefinition.Model;

  /// <summary>
  /// Stocke les followups pour pouvoir les mettre dans les graphes commentés lors d'une copie vers le presse-papier
  /// </summary>
  public class EUT
  {
    public EUT(Guid id, string name, FollowUp followup)
    {
      this.EUTId = id;
      this.FollowUp = followup;
      this.Name = name;
    }

    public Guid EUTId { get; set; }

    public string Name { get; set; }

    public FollowUp FollowUp { get; set; } = new FollowUp();
  }
}