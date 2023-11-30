using MapAlarm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace MapAlarm
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class MapView : ContentPage
  {
    private Position Position { get; set; }

    private Distance Radius { get; set; }
    private int AlarmZoneID { get; set; }

    private Xamarin.Forms.Maps.Map Map { get; set; }

    public MapView(int alarmZoneID)
    {
      InitializeComponent();
      NavigationPage.SetHasNavigationBar(this, false);
      System.Threading.Thread.Sleep(1000);
      this.AlarmZoneID = alarmZoneID;
      if (this.AlarmZoneID >= 0)
      {
        AlarmZone alarmZone = App.Database.GetAlarmZoneAsync(this.AlarmZoneID).Result;
        this.nameEntry.Text= alarmZone.Name;
        this.slider.Value = alarmZone.Radius * 10;
        Position position = new Position(alarmZone.Latitude, alarmZone.Longitude);
        MapClickedEventArgs e = new MapClickedEventArgs(position);
        DisplayMap(e);
      }
      else
      {
        DisplayMap(null);
      }
    }

    CancellationTokenSource cts;

    async void DisplayMap(MapClickedEventArgs e)
    {
      try
      {
        var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
        this.cts = new CancellationTokenSource();
        var location = await Geolocation.GetLocationAsync(request, cts.Token);
        if (location != null)
        {
          Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
          this.Position = new Position(location.Latitude, location.Longitude);
        }
        else
        {
          this.Position = new Position(36.9628066, -122.0194722);
        }
      }
      catch (Exception ex)
      {
        this.Position = new Position(36.9628066, -122.0194722);
      }

      MapSpan mapSpan = new MapSpan(this.Position, 0.01, 0.01);
      this.Map = new Xamarin.Forms.Maps.Map(mapSpan);
      this.Map.MapType = MapType.Street;
      this.Map.MapClicked += new EventHandler<MapClickedEventArgs>(OnMapClick);
      this.Map.IsShowingUser = true;
      this.layout.Children.Add(this.Map);
      if (e != null)
      {
        OnMapClick(null, e);
      }
    }

    private void DrawCircle(Position p, Distance radius)
    {
      Circle circle = new Circle
      {
        Center = p,
              Radius = radius,
              StrokeColor = Color.FromHex("#88FF0000"),
              StrokeWidth = 8,
              FillColor = Color.FromHex("#88FFC0CB")
            };
      this.Radius = new Distance(100 * this.slider.Value);
      // Add the Circle to the map's MapElements collection
      this.Map.MapElements.Add(circle);
    }

    private Pin PlacePin(Position p)
    {
      Pin pin = new Pin
      {
        Label = "",
        Address = "",
        Type = PinType.Place,
        Position = new Position(p.Latitude, p.Longitude)
      };
      this.Map.Pins.Add(pin);
      this.Position = pin.Position;
      return pin;
    }
    private void OnMapClick(object sender, MapClickedEventArgs e)
    {
      this.Map.MapElements.Clear();
      this.Map.Pins.Clear();
      Pin pin = PlacePin(e.Position);
      DrawCircle(pin.Position, new Distance (100 * this.slider.Value));
    }

    private void OnSliderChange(object sender, EventArgs e)
    {
      if (this.Map == null)
      {
        return;
      }

      if (this.Map.MapElements.Count > 0)
      {
        Circle circle = (Circle)this.Map.MapElements[0];
        this.Map.MapElements.Clear();
        DrawCircle(circle.Center, new Distance(100 * this.slider.Value));
      }
    }

    protected override void OnDisappearing()
    {
      if (cts != null && !cts.IsCancellationRequested)
        cts.Cancel();
      base.OnDisappearing();
    }
    
    async void OnSave(object sender, EventArgs e)
    {
      this.btnSave.IsEnabled = false;
      AlarmZone alarmZone = new AlarmZone();
      alarmZone.Name = this.nameEntry.Text;
      alarmZone.Latitude = this.Position.Latitude;
      alarmZone.Longitude = this.Position.Longitude;
      alarmZone.Radius = this.Radius.Kilometers;
      alarmZone.CanSendNotification = true;
      if (this.AlarmZoneID != -1)
      {
        alarmZone.ID = this.AlarmZoneID;
      }
      await App.Database.SaveAlarmZoneAsync(alarmZone);
      await Application.Current.MainPage.Navigation.PopAsync(); //Remove the page currently on top.
    }

    async void OnCancel(object sender, EventArgs e)
    {
      await Application.Current.MainPage.Navigation.PopAsync(); //Remove the page currently on top.
    }
  }
}