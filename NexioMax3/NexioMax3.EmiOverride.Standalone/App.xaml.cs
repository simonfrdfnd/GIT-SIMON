using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NexioMax3.EmiOverride.Standalone
{
  using System.Globalization;
  using System.Threading;
  using log4net;
  using Nexio.Bat.Common.Domain.Infrastructure;
  using Nexio.Helper;

  /// <summary>
  /// Logique d'interaction pour App.xaml
  /// </summary>
  public partial class App : Application
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(App));

    public App()
    {
      var usCulture = new CultureInfo("en-US");
      Thread.CurrentThread.CurrentCulture = usCulture;
      Thread.CurrentThread.CurrentUICulture = usCulture;
      CultureInfo.CurrentCulture = usCulture;
      CultureInfo.CurrentUICulture = usCulture;
      var rd = new ResourceDictionary() { Source = new Uri("/Nexio.Wpf;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute) };
      this.Resources.MergedDictionaries.Add(rd);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
      this.ConnectDatabase();
      base.OnStartup(e);
    }

    private void ConnectDatabase()
    {
      string pathDatabase = RegistryHelper.GetDatabasePath();
      if (string.IsNullOrEmpty(pathDatabase))
      {
        Log.Error("pathDatabase is empty");
        throw new Exception("Configuration error for database path");
      }

      try
      {
        Nexio.Bat.Common.Domain.Infrastructure.AccessDataBase.DataBase.Instance.Open(pathDatabase, string.Empty, -1);
      }
      catch (Exception ex)
      {
        Log.Error(ex);
        throw;
      }
    }
  }
}
