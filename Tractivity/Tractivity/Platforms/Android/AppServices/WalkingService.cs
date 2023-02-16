using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Runtime;
using Tractivity.Messaging;
using AndroidApp = Android.App.Application;

namespace Tractivity.AppServices
{
    [Service]
    public class WalkingService : LocationService, ISensorEventListener
    {
        private SensorManager _androidSensorManager;

        private int _totalSteps = 0;

        public WalkingService()
        {
            this.Initialize(2000, 4);
            this.BeginStepTracking();
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            // Do nothing
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            this._androidSensorManager?.UnregisterListener(this);
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type == SensorType.StepDetector)
            {
                // Add a step
                this._totalSteps++;

                var message = new StepDetectorUpdateEvent()
                {
                    Value = this._totalSteps
                };

                // Publish a message to any listeners
                // NOTE: This is extremely non performant. Improve later.
                MessagingCenter.Send(message, "step-detector-updates");
            }
        }

        private void BeginStepTracking()
        {
            this._androidSensorManager ??= (SensorManager)AndroidApp.Context.GetSystemService(Context.SensorService);
            this._androidSensorManager?.RegisterListener(this, this._androidSensorManager.GetDefaultSensor(SensorType.StepDetector), SensorDelay.Normal);
        }
    }
}