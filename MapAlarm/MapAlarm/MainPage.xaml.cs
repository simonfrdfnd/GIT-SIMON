using MapAlarm.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MapAlarm
{
  public partial class MainPage : ContentPage
  {
    public MainPage()
    {
      InitializeComponent();
    }

    async void OnAdd(object sender, EventArgs e)
    {
      this.btnPlus.IsEnabled = false;
      await Navigation.PushAsync(new NavigationPage(new MapView(-1)));
    }

    private void OnDel(object sender, EventArgs e)
    {
      List<AlarmZone> zones = App.Database.GetAlarmZonesAsync().Result;
      List<int> indexes = new List<int>();
      foreach (AlarmFrame alarmFrame in this.layout.Children.Where(x => x.GetType() == typeof(AlarmFrame)))
      {
        if (alarmFrame.IsSelected())
        {
          indexes.Add(this.layout.Children.IndexOf(alarmFrame));
          App.Database.DeleteAlarmZoneAsync(zones.First(x => x.ID == alarmFrame.AlarmZoneID));
        }
      }
      indexes.Reverse();

      foreach (int index in indexes)
      {
        this.layout.Children.RemoveAt(index);
      }
    }

    private void OnAppearing (object sender, EventArgs e)
    {
      this.btnPlus.IsEnabled = true;
      this.layout.Children.Clear();
      List<AlarmZone> zones = App.Database.GetAlarmZonesAsync().Result;
      foreach (AlarmZone alarmZone in zones)
      {
        AlarmFrame alarmFrame = new AlarmFrame(alarmZone.Name, alarmZone.ID);
        if (alarmZone.IsEnabled)
        {
          alarmFrame.ToggleSwitch();
        }
        this.layout.Children.Add(alarmFrame);
      }
    }
  }
}
