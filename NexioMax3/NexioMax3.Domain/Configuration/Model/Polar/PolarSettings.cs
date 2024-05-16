namespace NexioMax3.Domain.Configuration.Model.Polar
{
  using System;
  using System.IO;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Converters;
  using Nexio.Geom3D;

  [Flags]
  public enum PolarAxisMode
  {
    None = 0,
    Angle = 1,
    Height = 2,
    Angle2 = 4,
    Cylindric = Angle | Height,
    Polar = Angle | Angle2,
  }

  public enum SphericalConvention
  {
    ThetaPhi,
    AzimuthElevation,
  }

  public enum SphericalAngles
  {
    Angle1,
    Angle2,
  }

  public enum RotationOrder
  {
    HV,
    VH,
  }

  public class PolarSettings
  {
    private static PolarSettings intance;

    private PolarSettings()
    {
    }

    public static PolarSettings Instance
    {
      get
      {
        return intance ?? (intance = Load());
      }
    }

    [JsonProperty(nameof(SphericCoordinateDefinition), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public SphericCoordinateHelper SphericCoordinateDefinition { get; set; }

    public void Save()
    {
      if (File.Exists(Directories.PolarSettingsFile))
      {
        File.Delete(Directories.PolarSettingsFile);
      }

      File.WriteAllText(Directories.PolarSettingsFile, JsonConvert.SerializeObject(this));
    }

    private static PolarSettings Load()
    {
      if (!File.Exists(Directories.PolarSettingsFile))
      {
        return new PolarSettings() { SphericCoordinateDefinition = new SphericCoordinateHelper() };
      }
      else
      {
        var instance = JsonConvert.DeserializeObject<PolarSettings>(File.ReadAllText(Directories.PolarSettingsFile));
        return instance;
      }
    }
  }
}
