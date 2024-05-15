namespace Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model.Polar
{
  using System;
  using System.IO;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Converters;
  using Nexio.Geom3D;

  internal class CylindricCoordinateHelper : CoordinateHelper
  {
    public override void ToCartesian(double radius, double angle1, double height, double angle2, out double x, out double y, out double z)
    {
      CylindricToCartesian(radius, angle1 * Deg2Rad, height, out x, out y, out z);
    }

    private static void CylindricToCartesian(double r, double phi, double h, out double x, out double y, out double z)
    {
      x = r * Math.Cos(phi);
      y = r * Math.Sin(phi);
      z = h;
    }
  }
}
