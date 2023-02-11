using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using AndroidX.Core.App;
using Tractivity.Messaging;
using AndroidApp = Android.App.Application;
using Location = Android.Locations.Location;

namespace Tractivity.Platforms.Android.Services
{
    /// <summary>
    /// https://medium.com/nerd-for-tech/maui-native-mobile-location-updates-444939dff3af
    /// https://stackoverflow.com/questions/73829758/how-to-create-an-android-foreground-service-in-maui
    /// https://stackoverflow.com/questions/71259615/how-to-create-a-background-service-in-net-maui
    /// </summary>
    [Service]
    public partial class LocationService : Service, ILocationListener
    {
        private LocationManager _androidLocationManager;

        private string NOTIFICATION_CHANNEL_ID = "1000";

        private string NOTIFICATION_CHANNEL_NAME = "notification";

        private int NOTIFICATION_ID = 1;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            this.AndroidInitialize();
        }

        public async void OnLocationChanged(Location location)
        {
            if (location != null)
            {
                // Write to disk
                try
                {
                    string cacheDir = FileSystem.Current.CacheDirectory;
                    string fileName = $"records.txt";
                    string targetFile = System.IO.Path.Combine(cacheDir, fileName);
                    string[] locations =
                        {
                            $"Lat {location.Latitude}, Long {location.Longitude}, Bearing {location.Bearing}, Speed {location.Speed}"
                        };

                    var message = new LocationUpdateEvent()
                    {
                        Value = locations[0]
                    };

                    // Publish a message to any listeners
                    MessagingCenter.Send(message, "location-updates");

                    if (!File.Exists(targetFile))
                    {
                        await File.WriteAllLinesAsync(targetFile, locations);
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
                .SetContentTitle("ForegroundService")
                .SetContentText("Foreground Service is running")
                .Build();

            StartForeground(NOTIFICATION_ID, notification);

            return StartCommandResult.NotSticky;
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            // Nothing
        }

        protected void AndroidInitialize()
        {
            _androidLocationManager ??= (LocationManager)AndroidApp.Context.GetSystemService(Context.LocationService);

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

                _androidLocationManager.RequestLocationUpdates(LocationManager.GpsProvider, 2000, 4, this);
            });
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _androidLocationManager?.RemoveUpdates(this);
            StopSelf();
        }

        private void CreateNotificationChannel(NotificationManager notificationMnaManager)
        {
            var channel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, NOTIFICATION_CHANNEL_NAME, NotificationImportance.Low);
            notificationMnaManager.CreateNotificationChannel(channel);
        }
    }
}
