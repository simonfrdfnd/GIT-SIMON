namespace NexioMax3.Domain.Configuration.Model
{
  using System.IO;
  using Newtonsoft.Json;

  public class WindowPosition
  {
    public bool IsSet { get; set; }

    public double Left { get; set; }

    public double Width { get; set; } = 1200;

    public double Top { get; set; }

    public double Height { get; set; } = 900;

    public WindowState State { get; set; } = WindowState.Maximized;

    public int ScreenId { get; set; } = -1;

    public void Save()
    {
      if (File.Exists(Directories.WindowPositionConfigFile))
      {
        File.Delete(Directories.WindowPositionConfigFile);
      }

      File.WriteAllText(Directories.WindowPositionConfigFile, JsonConvert.SerializeObject(this));
    }

    public static WindowPosition Load()
    {
      if (!File.Exists(Directories.WindowPositionConfigFile))
      {
        return new WindowPosition();
      }
      else
      {
        try
        {
          var instance = JsonConvert.DeserializeObject<WindowPosition>(File.ReadAllText(Directories.WindowPositionConfigFile));
          return instance != null ? instance : new WindowPosition();
        }
        catch (System.Exception ex)
        {
          return new WindowPosition();
        }
      }
    }
  }
}