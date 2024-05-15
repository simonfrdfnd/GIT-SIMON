namespace Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model.EMIOverrides2
{
  public class EmiOverrideExternalCurve : StyleDefinition
  {
    public string Project { get; set; } = Nexio.Bat.Emi.VisuV4.Domain.Properties.Resources.All;

    public string Name { get; set; }

    public string Position { get; set; } = Nexio.Bat.Emi.VisuV4.Domain.Properties.Resources.All;
  }
}