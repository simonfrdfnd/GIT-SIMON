namespace Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model.EMIOverrides2
{
  using Nexio.Bat.Emi.VisuV4.Domain.Model;

  public class EmiOverrideCurve : StyleDefinition
  {
    public Detector Detector { get; set; }

    public string Position { get; set; }
  }
}