namespace Nexio.Bat.Emi.VisuV4.Domain.Model
{
  using System;
  using System.Collections.Generic;

  public class ExternalCurve
  {
    private List<(int id, double x, double y)> dataPoint = new List<(int id,double x, double y)>();

    public List<(int id, double x, double y)> DataPoints
    {
      get { return this.dataPoint; }
    }

    public CurveTag Tag { get; set; }

    public Function Function { get; set; }

    public string Name { get; set; }

    public List<int> ListIdSB { get; set; } = new List<int>();

    public Guid GuidSource { get; internal set; }

    public int Id { get; internal set; }

    public void Clear()
    {
      this.dataPoint.Clear();
    }

    public void AddPoint(int idPoint, double x, double y)
    {
      this.dataPoint.Add((idPoint, x, y));
    }
  }
}