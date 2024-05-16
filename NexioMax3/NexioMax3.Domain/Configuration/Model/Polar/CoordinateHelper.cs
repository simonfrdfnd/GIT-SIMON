namespace NexioMax3.Domain.Configuration.Model.Polar
{
  using System;
  using System.IO;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Converters;
  using Nexio.Geom3D;

  public abstract class CoordinateHelper
  {
    protected static double Deg2Rad { get; } = Math.PI / 180d;

    public abstract void ToCartesian(double radius, double angle1, double height, double angle2, out double x, out double y, out double z);

    internal static CoordinateHelper Get(PolarAxisMode mode)
    {
      switch (mode)
      {
        case PolarAxisMode.Cylindric:
          return new CylindricCoordinateHelper();
        case PolarAxisMode.Polar:
          return PolarSettings.Instance.SphericCoordinateDefinition;
        case PolarAxisMode.None:
        case PolarAxisMode.Angle:
        case PolarAxisMode.Height:
        case PolarAxisMode.Angle2:
        default:
          return null;
      }
    }
  }
}
