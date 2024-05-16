namespace NexioMax3.Domain.Configuration.Model.EMIOverrides2
{
  public class EmiOverridePoint : StyleDefinition
  {
    public string Position { get; set; } = 
      Properties.Resources.All;

    public string Source { get; set; } = 
      Properties.Resources.All;

    public string Column { get; set; } = 
      Properties.Resources.All;

    public bool OverrideSymbol { get; set; }

    public int SymbolId { get; set; }

    public bool SymbolFilled { get; set; }

    public bool OverrideUseProjection { get; set; }

    public bool UseProjection { get; set; }
  }
}