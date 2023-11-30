using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MapAlarm
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class AlarmFrame : ContentView
  {
    public int AlarmZoneID { get; set; }

    public AlarmFrame(string alarmName, int alarmZoneID)
    {
      InitializeComponent();
      this.AlarmName.Text = alarmName;
      this.AlarmZoneID = alarmZoneID;
    }

    async void OnTapped(object sender, EventArgs e)
    {
      await Navigation.PushAsync(new NavigationPage(new MapView(this.AlarmZoneID)));
    }
    public void OnToggle(object sender, ToggledEventArgs e)
    {
      MapAlarm.Models.AlarmZone alarmZone = App.Database.GetAlarmZoneAsync(this.AlarmZoneID).Result;
      if (alarmZone.IsEnabled != e.Value)
      {
        alarmZone.IsEnabled = e.Value;
        App.Database.SaveAlarmZoneAsync(alarmZone);
      }
    }

    public bool IsSelected()
    {
      return this.cb.IsChecked;
    }

    public void ToggleSwitch()
    {
      this.styleSwitch.IsToggled = true;
    }
  }
}