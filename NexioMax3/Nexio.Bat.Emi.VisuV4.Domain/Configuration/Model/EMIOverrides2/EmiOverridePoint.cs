namespace Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model.EMIOverrides2
{
  public class EmiOverridePoint : StyleDefinition
  {
    public string Position { get; set; } = Nexio.Bat.Emi.VisuV4.Domain.Properties.Resources.All;

    public string Source { get; set; } = Nexio.Bat.Emi.VisuV4.Domain.Properties.Resources.All;

    public string Column { get; set; } = Nexio.Bat.Emi.VisuV4.Domain.Properties.Resources.All;

    public bool OverrideSymbol { get; set; }

    public int SymbolId { get; set; }

    public bool SymbolFilled { get; set; }

    public bool OverrideUseProjection { get; set; }

    public bool UseProjection { get; set; }
  }
}