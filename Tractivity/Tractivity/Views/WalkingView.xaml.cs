using System.Collections.ObjectModel;
using Tractivity.Common.Environment;
using Tractivity.Managers;
using Tractivity.Messaging;
using Tractivity.Contract.Enums;

namespace Tractivity.Views;

public partial class WalkingView : ContentPage
{
    private readonly EnvironmentManager _environmentManager;

    private readonly ILocationManagerFactory _locationManagerFactory;

    private int totalLogCounter = 0;

    public WalkingView(EnvironmentManager environmentManager, ILocationManagerFactory locationManager)
    {
        this._environmentManager = environmentManager;
        this._locationManagerFactory = locationManager;
        InitializeComponent();
        BindingContext = this;
    }

    public ObservableCollection<Label> Locations { get; private set; } = new ObservableCollection<Label>();

    public void BeginWatchingPosition(object sender, EventArgs e)
    {
        this.Locations.Clear();

        this.ActivityMessage.Text = "Logging Started";

        this._locationManagerFactory.Initialize(ServiceType.Walking);

        // Subscribe to location updates
        MessagingCenter.Subscribe<LocationUpdateEvent>(this, "location-updates", (update) =>
        {
            this.totalLogCounter += 1;
            this.TotalLogCount.Text = this.totalLogCounter.ToString();
            this.Locations.Add(new Label()
            {
                Text = update.Value
            });
        });

        MessagingCenter.Subscribe<StepDetectorUpdateEvent>(this, "step-detector-updates", (update) =>
        {
            this.StepDetectorMessage.Text = $"{update.Value}";
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
}