namespace NexioMax3.Domain.Configuration.Model.EMIOverrides2
{
  public class EmiOverrideExternalCurve : StyleDefinition
  {
    public string Project { get; set; } = Properties.Resources.All;

    public string Name { get; set; }

    public string Position { get; set; } = Properties.Resources.All;
  }
}