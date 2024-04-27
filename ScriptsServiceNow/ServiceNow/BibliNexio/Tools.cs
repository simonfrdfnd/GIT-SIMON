namespace ServiceNow.BibliNexio
{
  using System;
  using System.Globalization;
  using System.IO;
  using System.Windows;
  using System.Windows.Forms;

  public static class Tools
  {
    #region Methods

    public static string IntToStringSizeConverter(int sizeInt)
    {
      if (sizeInt > 1024 * 1024)
      {
        return Math.Round(((double)sizeInt) / 1024 / 1024, 2) + " Mo";
      }
      else if (sizeInt > 1024)
      {
        return Math.Round(((double)sizeInt) / 1024, 2) + " Ko";
      }
      else
      {
        return sizeInt + " o";
      }
    }

    public static void RenameFile(string file, string newName)
    {
      string folder = Path.GetDirectoryName(file);
      string newFile = Path.Combine(folder, newName);

      System.IO.FileInfo fi = new System.IO.FileInfo(file);
      if (fi.Exists)
      {
        fi.MoveTo(newFile);
      }
    }

    public static Screen GetCurrentScreen(Window window)
    {
      var parentArea = new System.Drawing.Rectangle((int)window.Left, (int)window.Top, (int)window.Width, (int)window.Height);
      return Screen.FromRectangle(parentArea);
    }

    public static int GetScreenId(string name)
    {
      //var screens = Screen.AllScreens;
      //for (int i = 0; i < screens.Length; i++)
      //{
      //  var screen = screens[i];
      //  if (screen.DeviceName == name)
      //  {
      //    return i;
      //  }
      //}
      foreach (var screen in Screen.AllScreens)
      {
        if (screen.DeviceName == name)
        {
          var strId = screen.DeviceName.Replace(@"\\.\DISPLAY", string.Empty);
          return strId.ToInt();
        }
      }

      return 0;
    }

    public static int ToInt(this string value)
    {
      int tmp = 0;
      if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp))
      {
        return tmp;
      }
      else
      {
        return 0;
      }
    }
    public static Screen GetScreen(int requestedScreen)
    {
      var screens = Screen.AllScreens; var mainScreen = 0;
      if (screens.Length > 1 && mainScreen < screens.Length)
      {
        return screens[requestedScreen];
      }
      return screens[0];
    }

    #endregion Methods
  }
}