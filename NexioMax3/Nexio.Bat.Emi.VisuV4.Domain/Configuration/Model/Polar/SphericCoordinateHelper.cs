namespace Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model.Polar
{
  using System;
  using System.IO;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Converters;
  using Nexio.Geom3D;

  public class SphericCoordinateHelper : CoordinateHelper
  {
    public SphericCoordinateHelper()
    {
    }

    [JsonProperty(nameof(Convention))]
    [JsonConverter(typeof(StringEnumConverter))]
    public SphericalConvention Convention { get; set; } = SphericalConvention.AzimuthElevation;

    [JsonProperty(nameof(Rotation1))]
    [JsonConverter(typeof(StringEnumConverter))]
    public SphericalAngles Rotation1 { get; set; } = SphericalAngles.Angle1;

    [JsonProperty(nameof(InvRotation1), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool InvRotation1 { get; set; } = false;

    [JsonProperty(nameof(InvRotation2), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool InvRotation2 { get; set; } = false;

    [JsonProperty(nameof(RotationOrder))]
    [JsonConverter(typeof(StringEnumConverter))]
    public RotationOrder RotationOrder { get; set; } = RotationOrder.HV;

    public override void ToCartesian(double radius, double angle1, double height, double angle2, out double x, out double y, out double z)
    {
      var pt = new Coordinate((float)radius, 0, 0);
      var rotation = this.GetRotation((float)(angle1 * Deg2Rad), (float)(angle2 * Deg2Rad));
      var rotated = rotation.Rotate(pt);
      x = rotated.X;
      y = rotated.Y;
      z = rotated.Z;
    }

    private static Quaternion Theta(float angle)
    {
      return new Quaternion(((float)Math.PI / 2) - angle, -Vector.AxisY);
    }

    private static Quaternion Phi(float angle)
    {
      return new Quaternion(angle, Vector.AxisZ);
    }

    private static Quaternion Azimuth(float angle)
    {
      return new Quaternion(angle, Vector.AxisZ);
    }

    private static Quaternion Elevation(float angle)
    {
      return new Quaternion(angle, -Vector.AxisY);
    }

    private Quaternion GetRotation(float a1, float a2)
    {
      var angle1 = this.Rotation1 == SphericalAngles.Angle1 ?
        this.InvRotation1 ? -a1 : a1 :
        this.InvRotation2 ? -a2 : a2;
      var angle2 = this.Rotation1 == SphericalAngles.Angle1 ?
        this.InvRotation2 ? -a2 : a2 :
        this.InvRotation1 ? -a1 : a1;

      switch (this.Convention)
      {
        case SphericalConvention.ThetaPhi:
          {
            var q1 = this.RotationOrder == RotationOrder.HV ? Phi(angle1) : Theta(angle1); // rotation on phi
            var q2 = this.RotationOrder == RotationOrder.HV ? Theta(angle2) : Phi(angle2);// rotation on theta
            return q1 * q2;
          }

        case SphericalConvention.AzimuthElevation:
        default:
          {
            var q1 = this.RotationOrder == RotationOrder.HV ? Azimuth(angle1) : Elevation(angle1); // rotation on azimuth
            var q2 = this.RotationOrder == RotationOrder.HV ? Elevation(angle2) : Azimuth(angle2);// rotation on elevation
            return q1 * q2;
          }
      }
    }
  }
}
