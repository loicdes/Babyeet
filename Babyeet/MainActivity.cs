using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Babyeet.Classes;
using AlertDialog = Android.App.AlertDialog;
using Android.Content;
using System;
using Android.Graphics;

namespace Babyeet
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        static AccelerometerReader accelerometerReader;
        ImageView babyEmotion;
        Button btn_voix_1;
        Button btn_voix_2;
        Button btn_voix_off;
        Constants.VoiceMode voiceMode = Constants.VoiceMode.Voix_1;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            this.babyEmotion = FindViewById<ImageView>(Resource.Id.babyEmotion);

            btn_voix_1 = FindViewById<Button>(Resource.Id.btn_voix_1);
            btn_voix_2 = FindViewById<Button>(Resource.Id.btn_voix_2);
            btn_voix_off = FindViewById<Button>(Resource.Id.btn_voix_off);

            initListeners();

            //Initialisation du référentiel
            AlertDialog.Builder alertDialog = new AlertDialog.Builder(this);
            alertDialog.SetTitle("Info");
            alertDialog.SetMessage("Posez le téléphone sur une surface plane, face vers le haut, et parallèle au sol");
            alertDialog.SetPositiveButton("C'est fait !", start);
            alertDialog.SetCancelable(false);
            alertDialog.Show();
        }

        private void initListeners()
        {
            btn_voix_1.Click += Btn_Click;
            btn_voix_2.Click += Btn_Click;
            btn_voix_off.Click += Btn_Click;

        }
        public void start(object sender, DialogClickEventArgs e)
        {
            // Init
            accelerometerReader = new AccelerometerReader();

            // Gestion des évenèments
            accelerometerReader.OnRoll += (sender, e) =>
            {
                int resourceId = -1;
                switch (voiceMode)
                {
                    case Constants.VoiceMode.Voix_1:
                        resourceId = Resource.Raw.Voice01_02;
                        break;
                    case Constants.VoiceMode.Voix_2:
                        resourceId = Resource.Raw.Voice02_02;
                        break;
                }
                SoundManager.Play(this, resourceId, imageView: babyEmotion);
                accelerometerReader.OnEnd += AccelerometerReader_OnEnd;
            };
            accelerometerReader.OnLongRoll += (sender, e) =>
            {
                int resourceId = -1;
                switch (voiceMode)
                {
                    case Constants.VoiceMode.Voix_1:
                        resourceId = Resource.Raw.Voice01_03;
                        break;
                    case Constants.VoiceMode.Voix_2:
                        resourceId = Resource.Raw.Voice02_03;
                        break;
                }
                SoundManager.Play(this, resourceId, true,imageView: babyEmotion);
                accelerometerReader.OnEnd += AccelerometerReader_OnEnd;
            };

            // Greetings 
            SoundManager.Play(this, Resource.Raw.Voice02_01);
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Start();
            timer.Interval = 1000;
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
            babyEmotion.SetImageResource(Resource.Drawable.spongebob);
            int resourceId = -1;
            switch (voiceMode)
            {
                case Constants.VoiceMode.Voix_1:
                    resourceId = Resource.Raw.Voice01_10;
                    break;
                case Constants.VoiceMode.Voix_2:
                    resourceId = Resource.Raw.Voice02_10;
                    break;
            }
            SoundManager.Play(this, resourceId);
            accelerometerReader.OnEnd -= AccelerometerReader_OnEnd;
        }
        
        private void Btn_Click(object sender, EventArgs e)
        {
            switch(((Button) sender).Id)
            {
                case Resource.Id.btn_voix_1:
                    voiceMode = Constants.VoiceMode.Voix_1;
                    btn_voix_1.SetTextColor(Color.Red);
                    btn_voix_2.SetTextColor(Color.Gray);
                    btn_voix_off.SetTextColor(Color.Gray);
                    break;
                case Resource.Id.btn_voix_2:
                    voiceMode = Constants.VoiceMode.Voix_2;
                    btn_voix_1.SetTextColor(Color.Gray);
                    btn_voix_2.SetTextColor(Color.Red);
                    btn_voix_off.SetTextColor(Color.Gray);
                    break;
                case Resource.Id.btn_voix_off:
                    voiceMode = Constants.VoiceMode.Voix_Off;
                    btn_voix_1.SetTextColor(Color.Gray);
                    btn_voix_2.SetTextColor(Color.Gray);
                    btn_voix_off.SetTextColor(Color.Red);
                    break;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}