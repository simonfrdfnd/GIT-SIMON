namespace NexioMax3.Domain.Model
{
  using System.Collections.Generic;

  public enum FilteredDataType
  {
    Suspect,

    Final,
  }

  public abstract class FilteredData
  {
    public abstract FilteredDataType DataType { get; }

    public int IdPoint { get; set; }

    /// <summary>
    /// Fréquence en Hz
    /// </summary>
    public double Frequency { get; set; }

    public Function Function { get; set; }

    public string Position { get; set; }

    public List<string> Values { get; set; } = new List<string>(); // Valeurs de chaque colonne

    public List<double> DoubleValues { get; set; } = new List<double>();
  }
}