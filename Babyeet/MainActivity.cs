using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Babyeet.Classes;
using AlertDialog = Android.App.AlertDialog;
using Android.Content;

namespace Babyeet
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        static AccelerometerReader accelerometerReader;
        ImageView babyEmotion;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            this.babyEmotion = FindViewById<ImageView>(Resource.Id.babyEmotion);
            //Initialisation du référentiel
            AlertDialog.Builder alertDialog = new AlertDialog.Builder(this);
            alertDialog.SetTitle("Info");
            alertDialog.SetMessage("Posez le téléphone sur une surface plane, face vers le haut, et parallèle au sol");
            alertDialog.SetPositiveButton("C'est fait !", start);
            alertDialog.SetCancelable(false);
            alertDialog.Show();
        }

        public void start(object sender, DialogClickEventArgs e)
        {
            // Init
            accelerometerReader = new AccelerometerReader();

            // Gestion des évenèments
            accelerometerReader.OnRoll += (sender, e) =>
            {
                SoundManager.Play(this, Resource.Raw.Voice01_02, imageView: babyEmotion);
                accelerometerReader.OnEnd += AccelerometerReader_OnEnd;
            };
            accelerometerReader.OnLongRoll += (sender, e) =>
            {
                SoundManager.Play(this, Resource.Raw.Voice01_03, imageView: babyEmotion);
                accelerometerReader.OnEnd += AccelerometerReader_OnEnd;
            };

            // Greetings 
            SoundManager.Play(this, Resource.Raw.Voice02_01);
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Start();
            timer.Interval = 100;
            timer.Enabled = true;
            timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                RunOnUiThread(() =>
                {
                    accelerometerReader.ToggleAccelerometer();
                });
            };
        }

        private void AccelerometerReader_OnEnd(object sender, System.EventArgs e)
        {
            SoundManager.Play(this, Resource.Raw.Voice01_10);
            accelerometerReader.OnEnd -= AccelerometerReader_OnEnd;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}