using NexioMax3.Domain.Model;

namespace NexioMax3.Domain.Configuration.Model.EMIOverrides2
{
  public class EmiOverrideCurve : StyleDefinition
  {
    public Detector Detector { get; set; }

    public string Position { get; set; }
  }
}