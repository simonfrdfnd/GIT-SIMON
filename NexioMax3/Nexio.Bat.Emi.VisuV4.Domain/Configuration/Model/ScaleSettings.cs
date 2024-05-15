namespace NexioMax3.Domain.Configuration.Model
{
  public class ScaleSettings
  {
    public double MinimumFrequency { get; set; }

    public double MaximumFrequency { get; set; }

    public int PrecisionFrequency { get; set; }

    public bool IsLogFrequency { get; set; }

    public string UnitFrequency { get; set; }

    public bool IsAdjustLevel { get; set; }

    public double MinimumLevel { get; set; }

    public double MaximumLevel { get; set; }

    public int PrecisionLevel { get; set; }

    public string UnitLevel { get; set; }
  }
}