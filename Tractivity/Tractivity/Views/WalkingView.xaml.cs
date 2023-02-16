using Microsoft.Maui.Maps;
using System.Collections.ObjectModel;
using Tractivity.Common.Environment;
using Tractivity.Contract.Enums;
using Tractivity.Managers;
using Tractivity.Messaging;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace Tractivity.Views;

public partial class WalkingView : ContentPage
{
    private readonly EnvironmentManager _environmentManager;

    private readonly ILocationManagerFactory _locationManagerFactory;

    private CancellationTokenSource _cancelTokenSource;

    private bool _isCheckingLocation;

    private int logCount = 0;

    public WalkingView(EnvironmentManager environmentManager, ILocationManagerFactory locationManager)
    {
        this._environmentManager = environmentManager;
        this._locationManagerFactory = locationManager;
        InitializeComponent();
        BindingContext = this;
    }

    public ObservableCollection<Label> Locations { get; private set; } = new ObservableCollection<Label>();

    public async void BeginWatchingPosition(object sender, EventArgs e)
    {
        PermissionStatus locationAlwaysPermission = await Permissions.RequestAsync<Permissions.LocationAlways>();
        PermissionStatus sensorPermission = await Permissions.RequestAsync<Permissions.Sensors>();

        await this.LoadPage();

        this.Locations.Clear();

        this.ActivityMessage.Text = "Logging Started";

        this._locationManagerFactory.Initialize(ServiceType.Walking);

        // Subscribe to location updates
        MessagingCenter.Subscribe<LocationUpdateEvent>(this, "location-updates", (update) =>
        {
            this.logCount++;
            this.ActivityMessage.Text = $"Logged {this.logCount} times";
            this.map.Pins.Add(new Microsoft.Maui.Controls.Maps.Pin()
            {
                Location = new Location(update.Latitude, update.Longitude)
            });
        });

        MessagingCenter.Subscribe<StepDetectorUpdateEvent>(this, "step-detector-updates", (update) =>
        {
            this.StepDetectorMessage.Text = $"{update.Value} steps taken";
        });
    }

    public async void ReadAllRecords(object sender, EventArgs e)
    {
        this.Locations.Clear();

        this.ActivityMessage.Text = "Loading Logs";

        string cacheDir = FileSystem.Current.CacheDirectory;
        string fileName = this._environmentManager.LogToFileName;
        if (File.Exists(Path.Combine(cacheDir, fileName)))
        {
            var lines = await File.ReadAllLinesAsync(Path.Combine(cacheDir, fileName));
            foreach (string line in lines)
            {
                this.Locations.Add(new Label()
                {
                    Text = line
                });
            }
        }
    }

    public async void ResetPositions(object sender, EventArgs e)
    {
        this.Locations.Clear();
        this.ActivityMessage.Text = "All logged data cleared.";

        MessagingCenter.Unsubscribe<LocationUpdateEvent>(this, "location-updates");

        string cacheDir = FileSystem.Current.CacheDirectory;
        string fileName = this._environmentManager.LogToFileName;
        if (File.Exists(Path.Combine(cacheDir, fileName)))
        {
            File.Delete(Path.Combine(cacheDir, fileName));
            await DisplayAlert("Success", "All positions deleted.", "Got It");
        }
        else
        {
            await DisplayAlert("Ope!", "No locations saved, yet.", "Just Grand");
        }
    }

    public void StopWatchingPosition(object sender, EventArgs e)
    {
        this.ActivityMessage.Text = "Logging Stopped";

        MessagingCenter.Unsubscribe<LocationUpdateEvent>(this, "location-updates");

        this._locationManagerFactory.Stop(ServiceType.Walking);
    }

    private void CancelRequest()
    {
        if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
            _cancelTokenSource.Cancel();
    }

    private async Task<Location> GetCurrentLocation()
    {
        Location location = null;

        try
        {
            _isCheckingLocation = true;

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);
        }

        // Catch one of the following exceptions:
        //   FeatureNotSupportedException
        //   FeatureNotEnabledException
        //   PermissionException
        catch (Exception ex)
        {
            // Unable to get location
        }
        finally
        {
            _isCheckingLocation = false;
        }

        return location;
    }

    private async Task LoadPage()
    {
        try
        {
            Location location = await this.GetCurrentLocation();

            if (location != null)
            {
                // Maps: https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/map?view=net-maui-7.0
                Map map = new Map();
                map.IsShowingUser = true;
                Location pinLocation = new Location(location.Latitude, location.Longitude);
                MapSpan mapSpan = new MapSpan(pinLocation, 0.01, 0.01);
                this.map = map;
            }
        }
        catch (FeatureNotSupportedException fnsEx)
        {
            // Handle not supported on device exception
        }
        catch (FeatureNotEnabledException fneEx)
        {
            // Handle not enabled on device exception
        }
        catch (PermissionException pEx)
        {
            // Handle permission exception
        }
        catch (Exception ex)
        {
            // Unable to get location
        }
    }
}