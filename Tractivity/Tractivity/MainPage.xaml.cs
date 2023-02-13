using System.Collections.ObjectModel;
using Tractivity.Common.Environment;
using Tractivity.Managers;
using Tractivity.Messaging;

namespace Tractivity;

public partial class MainPage : ContentPage
{
    private readonly EnvironmentManager _environmentManager;
    private readonly LocationManager _locationManager;
    private int totalLogCounter = 0;

    //private CancellationTokenSource _cancelTokenSource;

    //private bool _isCheckingLocation;

    //private int checkDuration = 3000;

    //private bool isTracking = false;

    public MainPage(EnvironmentManager environmentManager, LocationManager locationManager)
    {
        this._environmentManager = environmentManager;
        this._locationManager = locationManager;
        InitializeComponent();
        BindingContext = this;
    }

    public ObservableCollection<Label> Locations { get; private set; } = new ObservableCollection<Label>();

    public void BeginWatchingPosition(object sender, EventArgs e)
    {
        this.Locations.Clear();
        
        this.ActivityMessage.Text = "Logging Started";

        this._locationManager.Initialize();

        MessagingCenter.Subscribe<LocationUpdateEvent>(this, "location-updates", (update) =>
        {
            this.totalLogCounter += 1;
            this.TotalLogCount.Text = this.totalLogCounter.ToString();
            this.Locations.Add(new Label()
            {
                Text = update.Value
            });
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
        this.totalLogCounter = 0;
        this.TotalLogCount.Text = this.totalLogCounter.ToString();

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
        this.ActivityMessage.Text = "Logging Stopped";

        MessagingCenter.Unsubscribe<LocationUpdateEvent>(this, "location-updates");

        this._locationManager.Stop();
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