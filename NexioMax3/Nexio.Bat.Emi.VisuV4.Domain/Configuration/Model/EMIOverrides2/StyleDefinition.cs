namespace NexioMax3.Domain.Configuration.Model.EMIOverrides2
{
  public abstract class StyleDefinition
  {
    public bool OverrideColor { get; set; }

    public string Color { get; set; }

    public bool OverrideIsVisible { get; set; }

    public bool IsVisible { get; set; }

    public bool OverrideSize { get; set; }

    public double Size { get; set; }
  }
}