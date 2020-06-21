using System;
using Android.Content;
using Android.Media;
using Android.Widget;

namespace Babyeet.Classes
{
    static class SoundManager
    {
        static MediaPlayer mediaPlayer;
        public static bool isAnySoundPlaying;
        static ImageView babyEmotion;
        public static void Play(Context context, int resourceId, bool force = false, ImageView imageView = null)
        {
            if (!isAnySoundPlaying || force)
            {
                if (resourceId != -1) // Voix off
                {
                    mediaPlayer = MediaPlayer.Create(context, resourceId);
                    mediaPlayer.Start();
                }
                if (imageView != null)
                {
                    babyEmotion = imageView;
                    babyEmotion.SetImageResource(Resource.Drawable.afraid_spongebob);
                }
                isAnySoundPlaying = true;
                mediaPlayer.Completion += MediaPlayer_Completion;
            }
        }

        private static void MediaPlayer_Completion(object sender, EventArgs e)
        {
            isAnySoundPlaying = false;
            babyEmotion?.SetImageResource(Resource.Drawable.spongebob);

        }

    }
}