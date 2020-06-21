using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace Babyeet.Classes
{
    class AccelerometerReader
    {
        SensorSpeed speed = SensorSpeed.UI;
        #region Events
        public event EventHandler OnRoll;
        public event EventHandler OnLongRoll;
        public event EventHandler OnClash;
        public event EventHandler OnWell;
        public event EventHandler OnLongWell;
        public event EventHandler OnEnd;
        #endregion
        float precision = 0.3f;
        static System.Timers.Timer timer;
        #region Refresh Values
        float accX;
        float accY;
        float accZ;
        #endregion
        #region Referencial Values
        float refX = -2;
        float refY = -2;
        float refZ = -2;
        #endregion
        public AccelerometerReader()
        {
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
        }
        void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs args)
        {
            var value = args.Reading.Acceleration;
            this.accX = value.X;
            this.accY = value.Y;
            this.accZ = value.Z;
            // Initialisation du référentiel
            if (this.refX == -2)
            {
                this.refX = value.X;
                this.refY = value.Y;
                this.refZ = value.Z;
            }
            if (Math.Abs(Math.Abs(this.refX) - Math.Abs(this.accX)) < this.precision &&
                Math.Abs(Math.Abs(this.refY) - Math.Abs(this.accY)) < this.precision &&
                Math.Abs(Math.Abs(this.refZ) - Math.Abs(this.accZ)) < this.precision)
            {
                try
                {
                    OnEnd(this, EventArgs.Empty);
                } catch(Exception e)
                {
                    // Lié au fait que la position actuelle soit identique à l'initiale 
                    // L'évènement OnEnd n'est pas encore géré
                }
            }
            // Détection du roulis
            else if (Math.Abs(Math.Abs(this.refX) - Math.Abs(this.accX)) > this.precision ||
                Math.Abs(Math.Abs(this.refY) - Math.Abs(this.accY)) > this.precision ||
                Math.Abs(Math.Abs(this.refZ) - Math.Abs(this.accZ)) > this.precision)
            {
                OnRoll(this, EventArgs.Empty);
                //Détection du long roll
                if (timer == null)
                {
                    timer = new System.Timers.Timer();
                    timer.Start();
                    timer.Interval = 5000;
                    timer.Enabled = true;
                    timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
                    {
                        if (Math.Abs(Math.Abs(this.refX) - Math.Abs(this.accX)) > this.precision ||
                               Math.Abs(Math.Abs(this.refY) - Math.Abs(this.accY)) > this.precision ||
                               Math.Abs(Math.Abs(this.refZ) - Math.Abs(this.accZ)) > this.precision)
                        {
                            OnLongRoll(this, EventArgs.Empty);
                        }
                    };
                }
            }
        }
        public void ToggleAccelerometer()
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                    Accelerometer.Stop();
                else
                    Accelerometer.Start(speed);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.

            }
        }
    }
}