namespace Nexio.Bat.Emi.VisuV4.Domain
{
  using System;
  using System.IO;

  public static class Directories
  {
    public static readonly string AppDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BAT-EMI VisuV4");
    public static readonly string VisuV4ReferentialName = "visuv4.cfg";
    public static readonly string LayoutsDirectory = Path.Combine(AppDataDirectory, "Layouts");
    public static readonly string AxisSettingFile = Path.Combine(AppDataDirectory, "AxisSettings.cfg");
    public static readonly string WindowPositionConfigFile = Path.Combine(AppDataDirectory, "Position.cfg");
    public static readonly string AppConfigFile = Path.Combine(AppDataDirectory, "App.cfg");
    public static readonly string LightLayoutConfigFile = Path.Combine(LayoutsDirectory, "layout.cfg");
    public static readonly string SourceListFile = Path.Combine(AppDataDirectory, "sourceCache.cfg");
    public static readonly string ColumnListFile = Path.Combine(AppDataDirectory, "columnCache.cfg");
    public static readonly string PolarSettingsFile = Path.Combine(AppDataDirectory, "polar.cfg"); // fichier de config locale du display des polars (pour l'instant que les settings spheric)
  }
}
