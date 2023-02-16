using Android;
using Android.App;
using Android.Runtime;

[assembly: UsesPermission(Manifest.Permission.AccessCoarseLocation)]
[assembly: UsesPermission(Manifest.Permission.AccessFineLocation)]
[assembly: UsesFeature("android.hardware.location", Required = true)]
[assembly: UsesFeature("android.hardware.location.gps", Required = true)]
[assembly: UsesFeature("android.hardware.location.network", Required = false)]
[assembly: UsesPermission(Manifest.Permission.AccessBackgroundLocation)]
[assembly: UsesPermission(Manifest.Permission.ForegroundService)]
[assembly: UsesPermission(Manifest.Permission.ActivityRecognition)]

namespace Tractivity;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
