namespace Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model.Polar
{
  using System;
  using System.IO;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Converters;
  using Nexio.Geom3D;

  public class Trace3DSettings
  {
    private CoordinateHelper coordinateSystem;

    public Trace3DSettings(PolarAxisMode mode)
    {
      this.coordinateSystem = CoordinateHelper.Get(mode);
    }

    public bool IsRadiusScaled { get; set; } = false;

    public CoordinateHelper CoordinateSystem { get => this.coordinateSystem; }

    public void ToCartesian(double radius, double angle1, double height, double angle2, out double x, out double y, out double z)
    {
      this.coordinateSystem.ToCartesian(radius, angle1, height, angle2, out x, out y, out z);
    }
  }
}
