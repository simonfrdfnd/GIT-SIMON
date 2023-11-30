using MapAlarm.Data;
using MapAlarm.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MapAlarm
{
  public partial class App : Application
  {
    static AlarmZoneDatabase database;

    // Create the database connection as a singleton.
    public static AlarmZoneDatabase Database
    {
      get
      {
        if (database == null)
        {
          database = new AlarmZoneDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Notes.db3"));
        }
        return database;
      }
    }
    public App()
    {
      InitializeComponent();

      MainPage = new NavigationPage(new MainPage());
    }
  }
}
