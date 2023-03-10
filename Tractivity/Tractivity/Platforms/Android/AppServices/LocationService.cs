using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using AndroidX.Core.App;
using Tractivity.Common.Environment;
using Tractivity.Messaging;
using AndroidApp = Android.App.Application;
using Location = Android.Locations.Location;

namespace Tractivity.AppServices
{
    /// <summary>
    /// https://medium.com/nerd-for-tech/maui-native-mobile-location-updates-444939dff3af
    /// https://stackoverflow.com/questions/73829758/how-to-create-an-android-foreground-service-in-maui
    /// https://stackoverflow.com/questions/71259615/how-to-create-a-background-service-in-net-maui
    /// </summary>
    public class LocationService : Service, ILocationService, ILocationListener
    {
        private readonly EnvironmentManager _environmentManager;

        private LocationManager _androidLocationManager;

        private string NOTIFICATION_CHANNEL_ID = "1000";

        private string NOTIFICATION_CHANNEL_NAME = "notification";

        private int NOTIFICATION_ID = 1;

        public LocationService()
        {
            // Not ideal to new this up but haven't found a good way
            // to sort out some DI issues with base services.
            this._environmentManager = new EnvironmentManager();
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            this._androidLocationManager?.RemoveUpdates(this);

            // Kill the running service.
            StopSelf();
        }

        public async void OnLocationChanged(Location location)
        {
            if (location != null)
            {
                // Write to disk
                try
                {
                    string cacheDir = FileSystem.Current.CacheDirectory;
                    string fileName = this._environmentManager.LogToFileName;
                    string targetFile = System.IO.Path.Combine(cacheDir, fileName);

                    string fileCSVHeaderRow = $"Latitude,Longitude,Altitude,Bearing,Speed";

                    string[] locations =
                        {
                            $"{location.Latitude},{location.Longitude},{location.Altitude},{location.Bearing},{location.Speed}"
                        };

                    var message = new LocationUpdateEvent()
                    {
                        Latitude = location.Latitude,
                        Longitude = location.Longitude
                    };

                    // Publish a message to any listeners
                    MessagingCenter.Send(message, "location-updates");

                    if (!File.Exists(targetFile))
                    {
                        string[] locationsWithHeaderRow =
                        {
                            fileCSVHeaderRow,
                            locations[0]
                        };

                        await File.WriteAllLinesAsync(targetFile, locationsWithHeaderRow);
                    }
                    else
                    {
                        // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-write-to-a-text-file
                        using StreamWriter streamWriter = new StreamWriter(targetFile, append: true);
                        await streamWriter.WriteLineAsync(locations[0]);
                    }
                }
                catch (Exception e)
                {
                    // Nothin
                }
            }
        }

        public void OnProviderDisabled(string provider)
        {
            // Nothing
        }

        public void OnProviderEnabled(string provider)
        {
            // Nothing
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var notifcationManager = GetSystemService(Context.NotificationService) as NotificationManager;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                CreateNotificationChannel(notifcationManager);
            }

            var notification = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID)
                .SetAutoCancel(false)
                .SetOngoing(true)
                .SetSmallIcon(Resource.Drawable.navigation_empty_icon)
                .SetContentTitle("Tractivity")
                .SetContentText("Tractivity recording in progress.")
                .Build();

            StartForeground(NOTIFICATION_ID, notification);

            return StartCommandResult.NotSticky;
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            // Nothing
        }

        protected void Initialize(long minTimeMs, float minDistanceM)
        {
            this._androidLocationManager ??= (LocationManager)AndroidApp.Context.GetSystemService(Context.LocationService);

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                {
                    return;
                }

                if (!_androidLocationManager.IsLocationEnabled)
                {
                    return;
                }

                if (!_androidLocationManager.IsProviderEnabled(LocationManager.GpsProvider))
                {
                    return;
                }

                _androidLocationManager.RequestLocationUpdates(LocationManager.GpsProvider, minTimeMs, minDistanceM, this);
            });
        }

        private void CreateNotificationChannel(NotificationManager notificationMnaManager)
        {
            var channel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, NOTIFICATION_CHANNEL_NAME, NotificationImportance.Low);
            notificationMnaManager.CreateNotificationChannel(channel);
        }
    }
}