using Android.App;
using System;
using MapAlarm;
using MapAlarm.Models;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;
using System.Threading;
using Android.Widget;
using Xamarin.Forms;
using MapAlarm.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(MessageAndroid))]
namespace MapAlarm.Droid
{
  [Service]
  [Obsolete]
  public class DemoIntentService : IntentService
  {
    [Obsolete]
    public DemoIntentService() : base("DemoIntentService")
    {
    }
    
    [Obsolete]
    protected override void OnHandleIntent(Android.Content.Intent intent)
    {
      try
      {
        List<AlarmZone> zones = new List<AlarmZone>();
        var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
        Position currentPos;
        Position centerPos;
        Distance distance;
        while (true)
        {
          if (App.Database.GetAlarmZonesAsync().Result.Where(x => x.IsEnabled == true).Count() > 0)
          {
            zones = App.Database.GetAlarmZonesAsync().Result.Where(x => x.IsEnabled == true).ToList();
          }
          else
          {
            continue;
          }

          var location = Geolocation.GetLocationAsync(request);
          currentPos = new Position(location.Result.Latitude, location.Result.Longitude);
          if (location != null)
          {
            foreach (AlarmZone alarmZone in zones)
            {
              {
                centerPos = new Position(alarmZone.Latitude, alarmZone.Longitude);
                distance = Distance.BetweenPositions(currentPos, centerPos);
                if (distance.Meters <= alarmZone.Radius * 1000)
                {
                  if (alarmZone.CanSendNotification)
                  {
                    DependencyService.Get<IMessage>().ShortAlert(alarmZone.Name);
                    alarmZone.CanSendNotification = false;
                    App.Database.SaveAlarmZoneAsync(alarmZone);
                  }
                }
                else
                {
                  alarmZone.CanSendNotification = true;
                  App.Database.SaveAlarmZoneAsync(alarmZone);
                }
              }
            }
          }
        }
      }
      catch (Exception ex) 
      {
        DependencyService.Get<IMessage>().ShortAlert("Error");
      }
    }
  }

  public interface IMessage
  {
    void LongAlert(string message);
    void ShortAlert(string message);
  }

  public class MessageAndroid : IMessage
  {
    public void LongAlert(string message)
    {
      Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
    }

    public void ShortAlert(string message)
    {
      Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
    }
  }
}