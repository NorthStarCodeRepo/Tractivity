using System.Collections.ObjectModel;
using Tractivity.Managers;
using Tractivity.Messaging;

namespace Tractivity;

public partial class MainPage : ContentPage
{
    //private CancellationTokenSource _cancelTokenSource;

    //private bool _isCheckingLocation;

    //private int checkDuration = 3000;

    //private bool isTracking = false;

    private LocationManager locationManager;

    public MainPage()
    {
        InitializeComponent();
        this.locationManager = new LocationManager();
        BindingContext = this;
    }

    public ObservableCollection<Label> Locations { get; private set; } = new ObservableCollection<Label>();

    public void BeginWatchingPosition(object sender, EventArgs e)
    {
        this.Locations.Clear();
        this.Locations.Add(new Label()
        {
            Text = "Logging started"
        });

        this.locationManager.Initialize();

        MessagingCenter.Subscribe<LocationUpdateEvent>(this, "location-updates", (update) =>
        {
            this.Locations.Add(new Label()
            {
                Text = update.Value
            });
        });
    }

    public async void ReadAllRecords(object sender, EventArgs e)
    {
        this.Locations.Clear();
        this.Locations.Add(new Label()
        {
            Text = "Loading locations..."
        });

        string cacheDir = FileSystem.Current.CacheDirectory;
        string fileName = $"records.txt";
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
        this.Locations.Add(new Label()
        {
            Text = "Locations cleared"
        });

        this.Locations.Clear();
        MessagingCenter.Unsubscribe<LocationUpdateEvent>(this, "location-updates");

        string cacheDir = FileSystem.Current.CacheDirectory;
        string fileName = $"records.txt";
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
        this.Locations.Add(new Label()
        {
            Text = "Logging stopped"
        });

        MessagingCenter.Unsubscribe<LocationUpdateEvent>(this, "location-updates");

        this.locationManager.Stop();
    }

    //private async void BeginLogging(object sender, EventArgs e)
    //{
    //    this.isTracking = true;

    //    this.Locations.Add(new Label()
    //    {
    //        Text = "Logging Started"
    //    });

    //    while (this.isTracking)
    //    {
    //        try
    //        {
    //            _isCheckingLocation = true;

    //            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

    //            _cancelTokenSource = new CancellationTokenSource();

    //            Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

    //            if (location != null)
    //            {
    //                this.Locations.Add(new Label()
    //                {
    //                    Text = $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}"
    //                });
    //            }
    //        }

    //        // Catch one of the following exceptions:
    //        //   FeatureNotSupportedException
    //        //   FeatureNotEnabledException
    //        //   PermissionException
    //        catch (Exception ex)
    //        {
    //            // Unable to get location
    //        }
    //        finally
    //        {
    //            _isCheckingLocation = false;
    //        }

    //        await Task.Delay(this.checkDuration);
    //    }
    //}

    //private void StopLogging(object sender, EventArgs e)
    //{
    //    this.isTracking = false;
    //    this.Locations.Clear();

    //    if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
    //    {
    //        _cancelTokenSource.Cancel();
    //    }

    //    this.Locations.Add(new Label()
    //    {
    //        Text = "Logging Stopped"
    //    });
    //}
}