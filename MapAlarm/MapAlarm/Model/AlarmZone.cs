using SQLite;
using System;
using Xamarin.Forms.Maps;

namespace MapAlarm.Models
{
  public class AlarmZone
  {
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string Name { get; set;}
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Radius { get; set; }
    public bool IsEnabled { get; set; }
    public bool CanSendNotification { get; set; }
  }
}
