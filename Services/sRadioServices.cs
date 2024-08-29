using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media.Session;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using static AndroidX.Media.App.NotificationCompat;

namespace sRadio
{
    [Service]
    public class sRadioAudioPlayerService : Service
    {
        string notificationChannelId = "sRadio";
        public sRadioAudioPlayerService()
        {
        }

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        public override void OnCreate()
        {

        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
        
            var notification = new NotificationCompat.Builder(Application.Context, notificationChannelId);

            notification
                .SetContentTitle("Welcome to sRadio")
                .SetContentText("sRadio")
                .SetSmallIcon(Resource.Drawable.exo_icon_play)
                .SetColorized(true)
                .SetColor(Color.Black)
                .AddAction(new NotificationCompat.Action(Resource.Drawable.exo_icon_stop, "stop",null))
                .AddAction(new NotificationCompat.Action(Resource.Drawable.exo_icon_previous,"previous", null))
                .AddAction(new NotificationCompat.Action(Resource.Drawable.exo_icon_play, "play", null))
                .AddAction(new NotificationCompat.Action(Resource.Drawable.exo_icon_next,"next", null))
                .SetStyle(new MediaStyle());


            StartForeground(1, notification.Build());

            // MediaButtonReceiver.HandleIntent(mediaSession, intent);
            return base.OnStartCommand(intent, flags, startId);
        }

    }
	
}