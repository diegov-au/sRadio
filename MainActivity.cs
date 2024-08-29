using System;
using System.IO;
using System.Net;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Core;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using Com.Google.Android.Exoplayer2.Metadata;
using Com.Google.Android.Exoplayer2.Metadata.Icy;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.UI;
using Com.Google.Android.Exoplayer2.Upstream;
using Android.Support.V4.Media.Session;
using Java.Lang;
using AndroidX.Media.Session;
using static AndroidX.Media.App.NotificationCompat;
using Android.Support.V4.Content;
using Android.Graphics;
using Android.Media.Session;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AndroidX.RecyclerView.Widget;
using JP.Wasabeef.BlurryLib;
using System.Collections.Generic;
using sRadio.DataModel;
using AndroidX.ViewPager2.Widget;
using sRadio.Resources;
using Android.Support.V4.Media;
using AndroidX.AppCompat.App;
using Google.Android.Material.Tabs;
using Com.Google.Android.Exoplayer2.Audio;
using Google.Android.Material.AppBar;
using Com.Google.Android.Exoplayer2.Util;
using Android.BillingClient.Api;
using Android.Graphics.Drawables;
using AndroidX.Palette.Graphics;
using System.Drawing.Imaging;
using Android.Net.Wifi.Aware;
using Android.OS.Strictmode;
using System.Linq;




#nullable enable annotations

namespace sRadio
{

    //[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", LaunchMode = LaunchMode.SingleInstance, MainLauncher = true)]
    [Activity(Label = "@string/app_name", Name = "com.diegoviso.sRadio.MainActivity",Theme = "@style/AppTheme", LaunchMode = LaunchMode.SingleInstance)]
    public class MainActivity : AppCompatActivity
    {
        //global variables

        public static bool backgroundServicesStarted = false;
        public static int isUnlocked = 0;
        private static string ekey = "seattle_argentina_seattle_argent";
        public string defaultBackgroundColor = "#FF2B4866";

        public static MediaMetadataStateClass CurrentMediaItemMetadataState;
        public static List<MediaItemClass> mediaItems;  //used to store media items metadata

        public static int autoPlay = 0;
        public static int currentMediaPlaylistItem = 0;
        public static int radioDirectory = 0;

        //local variables

        public static SimpleExoPlayer player;
        public static PlayerControlView playerView;
        public static MediaSessionCompat mediaSession;
        private static long playbackStateActions;

        //   private PlayerNotificationManager playerNotificationManager;

        public static RelativeLayout mainLayout;
        public ViewPager2 viewPager2;

        public static PlayerViewFragment pvf;
        public static FavouritesFragment ff;

        string notificationChannelId = "sRadio";
        Bitmap applicationIcon;
        public Bitmap placeholderIcon;

        private static string mediaStreamUrl = "";
        public long currentPosition;

        private static string AppID = "sRadio";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //base.OnCreate(null);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            applicationIcon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_launcher);
            placeholderIcon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_headset_white_24dp);
            mediaItems = new List<MediaItemClass>();

            CurrentMediaItemMetadataState = new MediaMetadataStateClass() { 
                MediaName = "Let's play something...", MediaTitle = "Welcome To sRadio", MediaArtist = "", 
                MediaAlbum = "Let's play something...", MediaArt = BitmapFactory.DecodeResource(Resources, Resource.Drawable.radio_tower) 
            };

            SetContentView(Resource.Layout.main_activity);

            _ = Billing.Initialize(Application.Context);

            RestoreAppSettings();

            InitializeLayout();

            CreateNotificationChannel(); //required first to createnotification channel
                    
            InitializeMediaSession(); //initializes media session

            InitializePlayer();  //creates player and assigns mediasession    

        }
        protected override void OnStart()
        {
            base.OnStart();

            InitializePlayer();
            initializeFromParams(Intent);

            if (mediaSession != null)
            {
                mediaSession.Active = true;
            }
        }
        protected override void OnResume()
        {
            base.OnResume();
            InitializePlayer();
            if (mediaSession != null)
            {
                mediaSession.Active = true;
            }

        }
        protected override void OnStop()
        {

            base.OnStop();
            //  if (player.IsPlaying)
            //    ShowMediaNotificatons();
            //Release();

        }
        protected override void OnPause()
        {
            base.OnPause();
            // Release();
        }
        protected override void OnDestroy()
        {
            if (player != null)
            {
                // mediaSession.Release();
                // mediaSessionConnector.Dispose();
                player.Release();
                player = null;

            }
            StopService(new Intent(this, typeof(sRadioAudioPlayerService)));
           
            base.OnDestroy();
        }
        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            initializeFromParams(intent);
        }
        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            if (hasFocus)
            {
              //  Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.HideNavigation | SystemUiFlags.ImmersiveSticky);

            }
        }
        public override void OnBackPressed() { }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        private void InitializeLayout()
        {
            mainLayout = (RelativeLayout)FindViewById(Resource.Id.main_layout);

            //setup the tool bar
            var toolbar = (MaterialToolbar)FindViewById(Resource.Id.topAppBar);

            if(isUnlocked == 0)
                toolbar.Title = "sRadio Free";
            else
                toolbar.Title = "sRadio Premium";

            toolbar.MenuItemClick += Toolbar_MenuItemClick;

            //hide nav bar
          //  Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.HideNavigation | SystemUiFlags.ImmersiveSticky);

            //setup the viewpager,adapter and tablelayou
            viewPager2 = FindViewById<ViewPager2>(Resource.Id.viewPager1);
            var adapter = new ScreenSlidePageAdapter(SupportFragmentManager, Lifecycle);

            //android:background="@drawable/app_background_gradient"
            var gradientColors = new int[] { Color.ParseColor("#FF2B4866"), Color.Black };
            var gd = new GradientDrawable(GradientDrawable.Orientation.TlBr, gradientColors);

            mainLayout.Background = gd;
            //viewPager2.Background = gd;

            viewPager2.Adapter = adapter;
            viewPager2.SaveEnabled = false;
            viewPager2.OffscreenPageLimit = 1;
        
            pvf = new PlayerViewFragment();
            ff = new FavouritesFragment();
            adapter.addFragment(pvf);
            adapter.addFragment(ff);

            //create tab layout
            var tabLayout = FindViewById<Google.Android.Material.Tabs.TabLayout>(Resource.Id.tabLayout1);
            var tabLayoutMediator = new TabLayoutMediator(tabLayout, viewPager2, new TabStrategy());
            tabLayoutMediator.Attach();
        }
        private void Toolbar_MenuItemClick(object sender, AndroidX.AppCompat.Widget.Toolbar.MenuItemClickEventArgs e)
        {

            var toolbar = (MaterialToolbar)sender;
            var menu = toolbar.Menu;
            var fm = SupportFragmentManager;

            switch (e.Item.ItemId)
            {
                case Resource.Id.action_search:
                    {
                        var ft = fm.BeginTransaction();
                        ft.Add(Resource.Id.main_layout, new SearchFragment());
                        ft.Commit();
                        break;
                    }

                case Resource.Id.action_add:
                    {
                        if (isUnlocked == 1)
                        {
                            var fragment = new CustomUrlFragment();
                            var ft = fm.BeginTransaction();
                            ft.SetCustomAnimations(Resource.Animation.enter_from_right, Resource.Animation.exit_to_right, Resource.Animation.enter_from_right, Resource.Animation.abc_slide_out_bottom);
                            ft.Add(Resource.Id.main_layout, fragment);

                            ft.Commit();

                            break;
                        }
                        else
                        {
                            Toast.MakeText(ApplicationContext, "Adding a custom URL is a premium feature.", ToastLength.Long).Show();
                            break;
                        }
                    }
                case Resource.Id.action_about:
                    {
                        var AboutDialog = new AboutDialogFragement();

                        AboutDialog.Show(fm, "aboutdialog");
                        break;
                    }

                case Resource.Id.action_settings:
                    {

                        var ft = fm.BeginTransaction();
                        ft.SetCustomAnimations(Resource.Animation.enter_from_right, Resource.Animation.exit_to_right, Resource.Animation.enter_from_right, Resource.Animation.abc_slide_out_bottom);
                        ft.Add(Resource.Id.main_layout, new SettingsFragment());
                        ft.Commit();
                        break;
                    }

                case Resource.Id.action_unlock:
                    {

                        var ft = fm.BeginTransaction();
                        ft.SetCustomAnimations(Resource.Animation.enter_from_right, Resource.Animation.exit_to_right, Resource.Animation.enter_from_right, Resource.Animation.abc_slide_out_bottom);
                        ft.Add(Resource.Id.main_layout, new UnlockFragment());
                        ft.Commit();
                        break;
                    }

            }

        }
        public class TabStrategy : Java.Lang.Object, TabLayoutMediator.ITabConfigurationStrategy
        {
            public void OnConfigureTab(Google.Android.Material.Tabs.TabLayout.Tab p0, int p1)
            {
                switch (p1)
                {
                    case 0:
                        p0.SetText("Player");
                        break;
                    case 1:
                        p0.SetText("Favourites");
                        break;
                }
            }
        }
        private void InitializeMediaSession()
        {

            mediaSession = new MediaSessionCompat(ApplicationContext, AppID);

            playbackStateActions = (PlaybackStateCompat.ActionPlayPause | PlaybackStateCompat.ActionPause | PlaybackStateCompat.ActionPlay | PlaybackStateCompat.ActionStop | PlaybackStateCompat.ActionSkipToNext | PlaybackStateCompat.ActionSkipToPrevious);

            //  mediaSession.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons | MediaSessionCompat.FlagHandlesTransportControls);

            // mediaSession.SetMediaButtonReceiver();
            mediaSession.SetCallback(new MediaSessionCallback(this));

            mediaSession.Active = true;

        }
        public void Release()
        {
            player.Release();
        }
        private void InitializePlayer()
        {
            if (player != null)
                return;

            //initialize mediastatemanager and set defaults

            player = new SimpleExoPlayer.Builder(this).Build();

            //connected to Google Assistant
            // mediaSessionConnector = new MediaSessionConnector(mediaSession);
            // mediaSessionConnector.SetPlayer(player);

            player.PlayWhenReady = true;

            //wires up player outputs
            player.AddListener(new PlayerEventListener(this));
            player.AddMetadataOutput(new EventMetadataOuput(this));

            //set player flags
            player.SetWakeMode(C.WakeModeNetwork);
            AudioAttributes audioAttributes = new AudioAttributes.Builder()
            .SetUsage(C.UsageMedia)
            .SetContentType(C.ContentTypeMovie)
            .Build();

            player.SetAudioAttributes(audioAttributes, true);

        }
        private static async void PlayStream(Context context, string mediaUri, string? mediaName)
        {

            if (mediaName != null)
            {
                CurrentMediaItemMetadataState.MediaTitle = "";
                CurrentMediaItemMetadataState.MediaArtist = "";

                CurrentMediaItemMetadataState.MediaName = mediaName;
                CurrentMediaItemMetadataState.MediaAlbum = mediaName;
                
                pvf.SetPlayerViewLabelMetadata();
            }

            //check if mediaurl is a pls file and extract the uri

            if (mediaUri == "" || mediaUri == null)
            {
                Toast.MakeText(context, "Please select a media item", ToastLength.Short).Show();
                return;
            }
            if (mediaUri.ToLower().Contains(".ashx"))
            {
                var p = new MediaUtils();
                mediaUri = await p.ParseAshxFileAsync(mediaUri);
            }

            //we scan pls after .ashx parsing may return pls file
            if (mediaUri.ToLower().Contains(".pls"))
            {
                var p = new MediaUtils();
                mediaUri = await p.ParsePlsFileAsync(mediaUri);
            }

            var HttpDataSourceFactory = new DefaultHttpDataSourceFactory(Util.GetUserAgent(context, "ExoPlayer"), null,
                DefaultHttpDataSource.DefaultConnectTimeoutMillis, DefaultHttpDataSource.DefaultReadTimeoutMillis, true);

            var DataSourceFactory = new DefaultDataSourceFactory(Application.Context, HttpDataSourceFactory);
            var mediaSource = new ProgressiveMediaSource.Factory(DataSourceFactory).CreateMediaSource(Android.Net.Uri.Parse(mediaUri));

            BackgroundNotificationService.Start(context);

            player.Prepare(mediaSource);
            player.PlayWhenReady = true;

            mediaSession.SetPlaybackState(new PlaybackStateCompat.Builder()
            .SetState(PlaybackStateCompat.StatePlaying, 0, 1)
            .SetActions(playbackStateActions)
            .Build());


        }
        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {

                NotificationManager notificationManager = (NotificationManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.NotificationService);

                ICharSequence channelName = new Java.Lang.String("sRadio");
                var importance = NotificationImportance.Low;
                string description = "Radio";
                NotificationChannel notificationChannel = new NotificationChannel(notificationChannelId, channelName, importance);
                notificationChannel.Description = description;
                notificationChannel.LockscreenVisibility = NotificationVisibility.Public;
                notificationManager.CreateNotificationChannel(notificationChannel);

            }
        }
        public void ShowMediaNotificatons()
        {
            MediaDescriptionCompat description;

            if (mediaSession.Controller.Metadata != null)
                description = mediaSession.Controller.Metadata.Description;
            else
                description = new MediaDescriptionCompat.Builder().Build();

            PendingIntent contentPendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(MainActivity)), 0);


            int playPauseIcon = 0;
            string playPauseString;
            bool showChronometer = true;
            var content = new NotificationContent();

            //validate metadata if its blank set the default values
            if (description.Title == null)
            {
                content.Title = "Live Radio";
                content.Subtitle = CurrentMediaItemMetadataState.MediaName;
                content.Description = "sRadio";
                content.IconBitmap = applicationIcon;

            }
            else
            {
                content.Title = description.Title;
                content.Subtitle = description.Subtitle;
                content.Description = description.Description;
                content.IconBitmap = description.IconBitmap;
            }


            if (player.IsPlaying)
            {
                playPauseIcon = Resource.Drawable.exo_icon_pause;
                playPauseString = "pause";
            }
            else
            {
                playPauseIcon = Resource.Drawable.exo_icon_play;
                playPauseString = "play";
                showChronometer = false;
            }

            var playPauseAction = new AndroidX.Core.App.NotificationCompat.Action(playPauseIcon, playPauseString, MediaButtonReceiver.BuildMediaButtonPendingIntent(Application.Context, PlaybackStateCompat.ActionPlayPause));

            var builder = new AndroidX.Core.App.NotificationCompat.Builder(Application.Context, notificationChannelId);
            builder
                .SetContentTitle(content.Title)
                .SetContentText(content.Subtitle)
                // .SetSubText(description.Description)
                .SetLargeIcon(content.IconBitmap)

                // Enable launching the player by clicking the notification
                .SetContentIntent(contentPendingIntent)

                // Stop the service when the notification is swiped away
                .SetDeleteIntent(GetDeleteIntent())

                // Make the transport controls visible on the lockscreen
                .SetVisibility(AndroidX.Core.App.NotificationCompat.VisibilityPublic)

                // Add an app icon and set its accent color
                // Be careful about the color
                .SetSmallIcon(Resource.Drawable.exo_icon_play)
                .SetColor(ContextCompat.GetColor(ApplicationContext, Resource.Color.colorPrimaryDark))

                 // Add a stop button
                .AddAction(new AndroidX.Core.App.NotificationCompat.Action(
                  Resource.Drawable.exo_icon_stop, "stop",
                  MediaButtonReceiver.BuildMediaButtonPendingIntent(Application.Context,
                  PlaybackStateCompat.ActionStop)))

                //skip backwards
                .AddAction(new AndroidX.Core.App.NotificationCompat.Action(Resource.Drawable.exo_icon_previous,
                "previous", MediaButtonReceiver.BuildMediaButtonPendingIntent(Application.Context, PlaybackStateCompat.ActionSkipToPrevious)))

                // Add a pause button
                .AddAction(playPauseAction)

                //skip forward
                .AddAction(new AndroidX.Core.App.NotificationCompat.Action(Resource.Drawable.exo_icon_next,
                "next", MediaButtonReceiver.BuildMediaButtonPendingIntent(Application.Context, PlaybackStateCompat.ActionSkipToNext)))

                // MediaStyle features
                .SetStyle(new MediaStyle()
                .SetMediaSession(mediaSession.SessionToken)
                .SetShowActionsInCompactView(0)

                // Add a cancel button
                .SetShowCancelButton(true)
                .SetCancelButtonIntent(MediaButtonReceiver.BuildMediaButtonPendingIntent(ApplicationContext,
                 PlaybackStateCompat.ActionStop)))

                .SetWhen(JavaSystem.CurrentTimeMillis() - player.CurrentPosition)
                .SetShowWhen(true)
                .SetUsesChronometer(showChronometer);

            // Display the notification and place the service in the foreground*/

            Notification notification = builder.Build();
            var notificationManager = AndroidX.Core.App.NotificationManagerCompat.From(this);

            // notificationId is a unique int for each notification that you must define
            notificationManager.Notify(1, builder.Build());
        }
        protected PendingIntent GetDeleteIntent()
        {
            Intent intent = new Intent(this, typeof(Receiver));
            intent.SetAction("NOTIFICATION_CANCELLED");
            //FinishAndRemoveTask();
            return PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.CancelCurrent);
        }
        public async void UpdateTrackMetadata(string trackInfo)
        {
            string title, artist;

            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            trackInfo = myTI.ToTitleCase(trackInfo.ToLower());

            try
            {
                string[] trackInfoSplit = trackInfo.Split("- "); //ICY FORMAT artist(0) - title(1)

                title = trackInfoSplit[1].TrimStart();
                artist = trackInfoSplit[0].TrimEnd();
            }
            catch
            {
                title = "Live Radio";
                artist = "sRadio";
            }

            //update metadata state machine and PlayerView labels
            CurrentMediaItemMetadataState.MediaTitle = title;
            CurrentMediaItemMetadataState.MediaArtist = artist;
            CurrentMediaItemMetadataState.MediaArt = placeholderIcon;

            //here we update the PlayerView labels, to quickly update the labels
            pvf.SetPlayerViewLabelMetadata();
            
            //get artwork
            if (title != "Live Radio")
            {
                var mu = new MediaUtils();
                var artworkUrl = await mu.GetTitleArtworkUrlAsync(trackInfo);
                CurrentMediaItemMetadataState.MediaArt= await mu.DownloadBitmapAsync(artworkUrl);
            }

            //update playerview artwork - we do this as a second step due to latency induced to fetching artwork
            pvf.SetPlayerViewArtwork();
            
            StylizeUIElementsWithAlburmArt(CurrentMediaItemMetadataState.MediaArt);

            SetSessionMetadata(CurrentMediaItemMetadataState.MediaTitle,
            CurrentMediaItemMetadataState.MediaArtist,
                CurrentMediaItemMetadataState.MediaAlbum, CurrentMediaItemMetadataState.MediaArt);
        }
        public void SetSessionMetadata(string? Title, string? Artist, string? Album, Bitmap? imageBitmap)
        {

            if (Title == null) // we've only passed the bitmap so lets keep existing string metadata in mediaSession
                CurrentMediaItemMetadataState.MediaArt = Bitmap.CreateScaledBitmap(imageBitmap, 256, 256, true);



            mediaSession.SetMetadata(new MediaMetadataCompat.Builder()
            .PutString(MediaMetadataCompat.MetadataKeyAlbum, CurrentMediaItemMetadataState.MediaName)  //required to invalidate session metadata data
            .Build());


            mediaSession.SetMetadata(new MediaMetadataCompat.Builder()
               .PutString(MediaMetadataCompat.MetadataKeyTitle, CurrentMediaItemMetadataState.MediaTitle)
               .PutString(MediaMetadataCompat.MetadataKeyArtist, CurrentMediaItemMetadataState.MediaArtist) //using artist info in both artist and album metadatakeys
               .PutString(MediaMetadataCompat.MetadataKeyAlbum, CurrentMediaItemMetadataState.MediaAlbum)  //since streaming radio want provide album name
               .PutBitmap(MediaMetadataCompat.MetadataKeyDisplayIcon, CurrentMediaItemMetadataState.MediaArt)
               .Build());


            ShowMediaNotificatons();

        }
        public void UpdateTimeProgressStatus(object sender, System.Timers.ElapsedEventArgs e)
        {



            RunOnUiThread(() => currentPosition = player.CurrentPosition);

            TimeSpan t = TimeSpan.FromMilliseconds(currentPosition + 1000);

            string time = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds);


           // TextView timerLabel = (TextView)pvf.view.FindViewById(Resource.Id.title);
           // timerLabel.Text = time;
        }
        public void StylizeUIElementsWithAlburmArt(Bitmap albumart)
        {
            if (isUnlocked == 0)
                return;

            GradientDrawable gd;
            int[] gradientColors;

            var toolbar = (MaterialToolbar)FindViewById(Resource.Id.topAppBar);
   
            if (!albumart.SameAs(placeholderIcon))
            {

                //update status toolbar with palette apu
                //[0] background
                //[1] text color
                //[2] secondary text color
                var colors = PaletteProcessor.GetColorsFromBitmapPalette(albumart);
          
                Window.SetStatusBarColor(new Android.Graphics.Color(colors[0]));
                gradientColors = new int[] { new Color(colors[0]), Color.Black};
                gd = new GradientDrawable(GradientDrawable.Orientation.BrTl, gradientColors);
                pvf.SetTrackTitleColor(colors[1]);
            
            }
            else 
            {
                Window.SetStatusBarColor(new Color(ContextCompat.GetColor(ApplicationContext, Resource.Color.colorPrimaryDark)));
                gradientColors = new int [] { Color.ParseColor(defaultBackgroundColor), Color.Black };
                gd = new GradientDrawable(GradientDrawable.Orientation.TlBr, gradientColors);
                pvf.SetTrackTitleColor(Color.White);
            }

           // viewPager2.Background = gd;
            mainLayout.Background = gd;
            pvf.SetTitleBackgroundGradient(gd);
            ff.SetBackground(gd);
            toolbar.Background = gd;
        }
        private void RestoreAppSettings()
        {
            //restore Track List

           //  Preferences.Clear();

            //check to see if we have an existing saved list in local storage
            var savedMediaItemString = Preferences.Get("MediaList", "[]");

            if (savedMediaItemString != null && savedMediaItemString.ToString() != "[]")
            {
                //decryption required for this property
                mediaItems = JsonConvert.DeserializeObject<List<MediaItemClass>>(MediaUtils.DecryptString(ekey, savedMediaItemString));
               
                if(mediaItems.Count == 0)
                    {
                    mediaItems.Add(new MediaItemClass { MediaName = "Music One", MediaUri = "https://stream.musicone.fm/musicone.aac", MediaDescription = "Dance Radio Worldwide", MediaLogoUrl = "https://static-media.streema.com/media/cache/6a/0f/6a0f884d8c3a81a3b8e40ab1051dafc5.jpg", MediaType = MediaTypes.StreamingRadio });
                    Preferences.Set("MediaList", MediaUtils.EncryptString(ekey, JsonConvert.SerializeObject(mediaItems)));
                    }
            }
            else
            {
                mediaItems.Add(new MediaItemClass { MediaName = "Music One", MediaUri = "https://stream.musicone.fm/musicone.aac", MediaDescription = "Dance Radio Worldwide", MediaLogoUrl = "https://static-media.streema.com/media/cache/6a/0f/6a0f884d8c3a81a3b8e40ab1051dafc5.jpg", MediaType = MediaTypes.StreamingRadio });
                Preferences.Set("MediaList", MediaUtils.EncryptString(ekey, JsonConvert.SerializeObject(mediaItems)));
            }

            //decryption required for this property
            var unLockSetting = Preferences.Get("zk", "[]");

            if (unLockSetting != null && unLockSetting != "[]")
            {
                isUnlocked = Convert.ToInt32(MediaUtils.DecryptString(ekey, unLockSetting));
            }

            var autoPlaySetting = Preferences.Get("AutoPlay", -1);

            if (autoPlaySetting != -1)
            {
                autoPlay = Convert.ToInt32(autoPlaySetting);
            }

            var radioDirectorySetting = Preferences.Get("RadioDirectory", -1);

            if (radioDirectorySetting != -1)
            {
                radioDirectory = Convert.ToInt32(radioDirectorySetting);
            }

          

        }
        public static void SaveAppSettings()
        {

            var mediaItemTemp = mediaItems.ToList();

            if (isUnlocked == 0 && mediaItemTemp.Count >3) //trim media items down to 3 before saving if app is locked
                {
                mediaItemTemp.RemoveRange(3, mediaItemTemp.Count - 3);
                }
            //save media list
            Preferences.Set("MediaList", MediaUtils.EncryptString(ekey,JsonConvert.SerializeObject(mediaItemTemp)));
            //save unlocked settings
            Preferences.Set("zk", MediaUtils.EncryptString(ekey, JsonConvert.SerializeObject(isUnlocked)));

            //save auto play preferences
            Preferences.Set("AutoPlay", autoPlay);

            //save radio directory preferences
            Preferences.Set("RadioDirectory", radioDirectory);


        }
        public void AddToFavourites(MediaItemClass mediaItem)
        {

            if (mediaItems.Count > 2 && isUnlocked == 0)
            {
                Toast.MakeText(this, "Only 3 favourites are supported in the free version.", ToastLength.Long).Show();
                return;
            }

            if (mediaItems.Count > 30)
            {
                Toast.MakeText(this, "At this stage we are only supporting 30 favourites.", ToastLength.Long).Show();
                return;
            }

            mediaItems.Add(mediaItem);

            var fvAdapter = ff.GetAdapter();
            fvAdapter.NotifyDataSetChanged();
            
            SaveAppSettings();

        }
        public void SetRadioDirectoryFromFragement(int radioInt)
        {
            radioDirectory = radioInt;
            SaveAppSettings();
        }
        public void PlayFromFragment(string mediaUrl, string albumTitle)
        {

            PlayStream(this, mediaUrl, albumTitle);
        }
        public static void PlayListActionManager(int skipDirection)
        {
            var totalTracks = mediaItems.Count;

            // var mediaName = (TextView)pvf.FindViewById(Resource.Id.mediaName);
            // mediaName.Text = mediaItems[currentMediaItem].MediaName

            //check current item is within the playlist count

            if (currentMediaPlaylistItem > mediaItems.Count - 1)
                currentMediaPlaylistItem = mediaItems.Count - 1;

            if (skipDirection == 1) //skip next
            {
                if (currentMediaPlaylistItem < mediaItems.Count - 1)
                {
                    currentMediaPlaylistItem++;

                    PlayStream(Application.Context, mediaItems[currentMediaPlaylistItem].MediaUri, mediaItems[currentMediaPlaylistItem].MediaName);
                }
            }
            else
            {
                if (currentMediaPlaylistItem > 0)
                {
                    currentMediaPlaylistItem--;

                    PlayStream(Application.Context, mediaItems[currentMediaPlaylistItem].MediaUri, mediaItems[currentMediaPlaylistItem].MediaName);
                }

            }


        }
        public void RestoreDiegosStations()
        {
            Preferences.Clear();


            mediaItems.Clear();
            mediaItems.Add(new MediaItemClass { MediaName = "Music One", MediaUri = "https://stream.musicone.fm/musicone.aac", MediaDescription = "Dance Radio Worldwide", MediaLogoUrl = "https://static-media.streema.com/media/cache/6a/0f/6a0f884d8c3a81a3b8e40ab1051dafc5.jpg", MediaType = MediaTypes.StreamingRadio });
            mediaItems.Add(new MediaItemClass { MediaName = "Hot Dance FM", MediaUri = "http://stream01.hotdanceradio.com:80/64.aac", MediaType = MediaTypes.StreamingRadio, MediaDescription = "Dance music from Prague", MediaLogoUrl = "https://i1.sndcdn.com/artworks-000540079284-s4v9w5-t500x500.jpg" });
            mediaItems.Add(new MediaItemClass { MediaName = "ZM", MediaUri = "http://ais-nzme.streamguys1.com/nz_008_aac", MediaType = MediaTypes.StreamingRadio, MediaDescription = "ZM New Zealand Top 40", MediaLogoUrl = "https://i.iheart.com/v3/re/new_assets/58a3bce4193955e8f6ff9a06?ops=fit(480%2C480)" });
            mediaItems.Add(new MediaItemClass { MediaName = "Jack2Hits", MediaUri = "https://playerservices.streamtheworld.com/api/livestream-redirect/JACK2_LOWAAC.aac", MediaType = MediaTypes.StreamingRadio, MediaDescription = "Play what you want", MediaLogoUrl = "https://d3kle7qwymxpcy.cloudfront.net/images/broadcasts/a0/36/11903/c300.png" });

            Preferences.Set("MediaList", MediaUtils.EncryptString(ekey,JsonConvert.SerializeObject(mediaItems)));


            var adapter = ff.GetAdapter();
            adapter.NotifyDataSetChanged();

            SaveAppSettings();

        }
        private void initializeFromParams(Intent intent)
        {
            if ( intent.Action == "android.media.action.MEDIA_PLAY_FROM_SEARCH")
            {
                var extra = intent.Extras.GetString(SearchManager.Query);

                PlayStream(this, mediaItems[0].MediaUri, mediaItems[0].MediaName);
            }
        }
        public static void UnlockApp(bool state)
        {
            if (state)
                isUnlocked = 1;
            else
                isUnlocked = 0;

            if (mainLayout != null)
            {
                var toolbar = (MaterialToolbar)mainLayout.FindViewById(Resource.Id.topAppBar);
                if (isUnlocked == 0)
                    toolbar.Title = "sRadio Free";
                else
                    toolbar.Title = "sRadio Premium";
            }

            SaveAppSettings();


        }
        public static class BackgroundNotificationService
        {
            public static void Start(Context context)
            {
                if (!backgroundServicesStarted)
                {
                    backgroundServicesStarted = true;
                    context.StartForegroundService(new Intent(context, typeof(sRadioAudioPlayerService)));

                }
            }

            public static void Stop(Context context)
            {
                context.StopService(new Intent(context.ApplicationContext, typeof(sRadioAudioPlayerService)));
                backgroundServicesStarted = false;

            }

        }
        public struct NotificationContent
        {
            public string Title;
            public string Subtitle;
            public string Description;
            public Bitmap IconBitmap;
        }
        public class PlayerEventListener : Java.Lang.Object, IPlayerEventListener
        {
            MainActivity activity;
            public PlayerEventListener(MainActivity activity)
            {

                this.activity = activity;
                //Bring in any variables needed and implement the functions for the listener in this class
            }

            public void OnLoadingChanged(bool p0) { }
            public void OnPlayerError(ExoPlaybackException p0)
            {

                Toast.MakeText(Application.Context, "Could not play stream.", ToastLength.Long).Show();

            }
            public void OnPlayerStateChanged(bool playWhenReady, int playbackState)
            {


                if (playbackState == IPlayer.StateBuffering)
                {
                    _ = "Buffering";
                }
              
            }
            public void OnPositionDiscontinuity(int reason) { }
            public void OnPlaybackSuppressionReasonChanged(int reason) { }
            public void OnSeekProcessed() { }
            public void OnIsPlayingChanged(bool isPlaying)
            {

                if (isPlaying)
                {
                    _ = "Playing...";

                }
                else
                {
                    _ = "Stopped";

                }
                activity.ShowMediaNotificatons();
            }
            public void OnTimelineChanged(Timeline p0, int p1)
            {



            }
            public void OnTracksChanged(TrackGroupArray p0, TrackSelectionArray p1)
            {

                try
                {
                    var Track = p0.Get(0);

                    Metadata.IEntry Metadata = Track.GetFormat(0).Metadata.Get(0);

                    if (Metadata is IcyHeaders)
                    {
                        var IcyHeaders = (IcyHeaders)Metadata;
                        CurrentMediaItemMetadataState.MediaBitrate = IcyHeaders.Bitrate.ToString().Remove(IcyHeaders.Bitrate.ToString().Length - 3);

                    }
                }
                catch { }

                //  var Tracks = p0.Get(1).GetFormat(0);

            }
        }
        public class EventMetadataOuput : Java.Lang.Object, IMetadataOutput
        {
            MainActivity activity;
            public EventMetadataOuput(MainActivity activity)
            {

                this.activity = activity;
                //Bring in any variables needed and implement the functions for the listener in this class
            }
            public void OnMetadata(Metadata metadata)
            {
                var trackInfo = "";

                if (metadata.Length() > 0)
                {
                    for (int i = 0; i < metadata.Length(); i++)
                    {
                        Metadata.IEntry entry = metadata.Get(i);
                        if (entry is IcyInfo)
                        {
                            var IcyInfo = (IcyInfo)entry;
                            trackInfo = IcyInfo.Title;
                        }
                        if (entry is IcyHeaders)
                        {
                            var IcyHeaders = (IcyHeaders)entry;

                        }
                    }
                }

                if (trackInfo == " " || trackInfo == "")
                    trackInfo = "Live Radio";



                CurrentMediaItemMetadataState.MediaCodec = player.AudioFormat.SampleMimeType.ToString().Contains("mp4a") ? "AAC" : "MP3";


                var sampleRate = CurrentMediaItemMetadataState.MediaSamplerate = player.AudioFormat.SampleRate.ToString().Remove(4, player.AudioFormat.SampleRate.ToString().Length - 4);

                //check samplerate if its less than 24.5 is likely its AAC+
                if (Convert.ToInt32(sampleRate) <= 2400 && CurrentMediaItemMetadataState.MediaCodec == "AAC")
                {
                    CurrentMediaItemMetadataState.MediaSamplerate = (Convert.ToInt32(sampleRate) * 2).ToString();
                    CurrentMediaItemMetadataState.MediaSamplerate = CurrentMediaItemMetadataState.MediaSamplerate.Remove(CurrentMediaItemMetadataState.MediaSamplerate.Length - 1, 1);
                    CurrentMediaItemMetadataState.MediaCodec = "AAC+";

                }
                CurrentMediaItemMetadataState.MediaSamplerate = CurrentMediaItemMetadataState.MediaSamplerate.Insert(2, ".");


                var mm = CurrentMediaItemMetadataState.MediaSamplerate + " Khz, " + player.AudioFormat.ChannelCount.ToString() + " ch, " + CurrentMediaItemMetadataState.MediaBitrate + " kbps, " + CurrentMediaItemMetadataState.MediaCodec;

                activity.UpdateTrackMetadata(trackInfo);

                pvf.SetMediaMetadataLabel(mm);
             


            }

        }
        public class PlayerViewControlDispatcher : DefaultControlDispatcher
        {
            MainActivity activity;

            public PlayerViewControlDispatcher(MainActivity activity)
            {
                this.activity = activity;
            }
            public override bool DispatchSetPlayWhenReady(IPlayer player, bool playWhenReady)
            {
                if (playWhenReady && player.PlaybackState == (int)PlaybackStateCode.Stopped)
                {

                    PlayStream(activity.ApplicationContext, mediaStreamUrl, null);
                }
                return base.DispatchSetPlayWhenReady(player, playWhenReady);
            }

            public override bool DispatchSeekTo(IPlayer player, int windowIndex, long positionMs)
            {
                return base.DispatchSeekTo(player, windowIndex, positionMs);
            }



        }
        public class MediaSessionCallback : MediaSessionCompat.Callback
        {

            MainActivity activity;

            public MediaSessionCallback(MainActivity activity)
            {
                this.activity = activity;
            }

            public override bool OnMediaButtonEvent(Intent mediaButtonEvent)
            {
                string intentAction = mediaButtonEvent.Action;

                if (Intent.ActionMediaButton.Equals(intentAction))
                {
                    KeyEvent keyEvent = (KeyEvent)mediaButtonEvent.GetParcelableExtra(Intent.ExtraKeyEvent);
                    if (keyEvent == null)
                        return base.OnMediaButtonEvent(mediaButtonEvent);

                    var keyCode = keyEvent.KeyCode;
                    var action = keyEvent.Action;

                    if (keyEvent.RepeatCount == 0 && action == KeyEventActions.Down)
                    {
                        switch (keyCode)
                        {
                            // Do what you want in here
                            case Keycode.MediaStop:
                                OnStop();
                                break;
                            case Keycode.MediaPause:
                                
                                break;
                            case Keycode.MediaPlay:
                               // OnPlay();
                                break;
                            case Keycode.MediaPlayPause:
                              
                                break;
                            case Keycode.MediaNext:
                                //OnSkipToNext();
                                break;
                            case Keycode.MediaPrevious:
                                //OnSkipToPrevious();
                                break;
                        }
                    }

                }

               // activity.ShowMediaNotificatons();
                return base.OnMediaButtonEvent(mediaButtonEvent);
            }

            public override void OnPlay()
            {
                base.OnPlay();
                player.PlayWhenReady = true;
             
                if (mediaSession != null)
                {
                    mediaSession.Active = true;
                    mediaSession.SetPlaybackState(new PlaybackStateCompat.Builder().SetActions(playbackStateActions).SetState(PlaybackStateCompat.StatePlaying, player.CurrentPosition, 1).Build());
                 
                }
            }
            public override void OnStop()
            {
                base.OnStop();
                if (mediaSession != null)
                {
                    player.Stop();
                    BackgroundNotificationService.Stop(activity.ApplicationContext);
                    mediaSession.SetPlaybackState(new PlaybackStateCompat.Builder().SetActions(playbackStateActions).SetState(PlaybackStateCompat.StateStopped, player.CurrentPosition, 1).Build());
                    mediaSession.Active = false;
                }
            }
            public override void OnPause()
            {
                base.OnPause();

                var state = PlaybackStateCompat.StatePlaying;

                if (player.IsPlaying)
                {
                    player.PlayWhenReady = false;
                    state = PlaybackStateCompat.StatePaused;
                }
                else
                    player.PlayWhenReady = true;

                mediaSession.SetPlaybackState(new PlaybackStateCompat.Builder().SetActions(playbackStateActions).SetState(state, player.CurrentPosition, 1).Build());
                
            }
            public override void OnSkipToNext()
            {
                base.OnSkipToNext();
                PlayListActionManager(1);
            }
            public override void OnSkipToPrevious()
            {
                base.OnSkipToPrevious();
                 PlayListActionManager(0);
            }
            public override void OnPlayFromMediaId(string mediaId, Bundle extras)
            {
                base.OnPlayFromMediaId(mediaId, extras);
                
                var trackNumber = Convert.ToInt32(mediaId);
                
                if (trackNumber + 1 > mediaItems.Count)
                    trackNumber = 0;

                PlayStream(activity.ApplicationContext ,mediaItems[trackNumber].MediaUri, mediaItems[trackNumber].MediaName);
            }

            public override void OnPlayFromSearch(string query, Bundle extras)
            {
                base.OnPlayFromSearch(query, extras);
                PlayStream(activity.ApplicationContext, mediaItems[0].MediaUri, mediaItems[0].MediaName);

            }


        }
   

        [BroadcastReceiver(Enabled = true, Exported = false, Name = "com.diegoviso.sRadio.Receiver")]
        public class Receiver : BroadcastReceiver
        {

            public override void OnReceive(Context context, Intent intent)
            {

                KeyEvent keyEvent = (KeyEvent)intent.GetParcelableExtra(Intent.ExtraKeyEvent);

                if (intent.Action == "NOTIFICATION_CANCELLED")
                {
                    mediaSession.Active = false;
                    player.Stop();
                    context.StopService(new Intent(context, typeof(sRadioAudioPlayerService)));              
                

                }

            }
        }
    }

 
} 




/*
 * 
 *  else if ((intent.Action == "android.intent.action.MEDIA_BUTTON") && (keyEvent.KeyCode == Keycode.MediaPlayPause) && (player.PlayWhenReady && player.PlaybackState == (int)PlaybackStateCode.Stopped))
                {

                    PlayStream(context, mediaStreamUrl,null);
                }
                else if ((intent.Action == "android.intent.action.MEDIA_BUTTON") && (keyEvent.KeyCode == Keycode.MediaStop))
                {

                    mediaSession.Active = false;
                    player.Stop();
                    context.StopService(new Intent(context, typeof(sRadioAudioPlayerService)));
                }
                else if ((intent.Action == "android.intent.action.MEDIA_BUTTON") && (keyEvent.KeyCode == Keycode.MediaNext))
                {
                    PlayListActionManager(1);
                }
                else if ((intent.Action == "android.intent.action.MEDIA_BUTTON") && (keyEvent.KeyCode == Keycode.MediaPrevious))
                {
                    PlayListActionManager(0);
                }

                else
                {
                    MediaButtonReceiver.HandleIntent(mediaSession, intent);
                }
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * private async Task GetTitleArtworkAsyncMB(string trackInfo)

{
    // trackInfo = "lady gaga - rain on me";

    //get track id
    Bitmap imageBitmap = null;
    string jsonResultString = null;

    //query musicbrainz for track MBID
    var q = new MetaBrainz.MusicBrainz.Query("sRadio", "1.0", "mailto: clubv@hotmail.com");
    var release = await q.FindReleasesAsync(trackInfo);


    if (release.TotalResults == 0)
        return;

    var MBID = release.Results[0].Item.Id.ToString();


    try
    {
        using (var webClient = new WebClient())
        {
            jsonResultString = await webClient.DownloadStringTaskAsync(new System.Uri("http://coverartarchive.org/release/" + MBID));

            if (jsonResultString != null)
            {
                var jsonArray = JObject.Parse(jsonResultString);

                string artUrl = (string)jsonArray["images"][0]["image"];

                var imageBytes = await webClient.DownloadDataTaskAsync(new System.Uri(artUrl));

                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

        }
    }
    catch (WebException)
    {
        imageBitmap = applicationIcon; //if we cannot find album art we default to application logo
    }

    UpdateUIAlburmArt(imageBitmap);


    SetSessionMetadata(null, null, null, imageBitmap);

}
}



public class MediaDescriptionAdapter : Java.Lang.Object, PlayerNotificationManager.IMediaDescriptionAdapter
    {
        MainActivity activity;
        public MediaDescriptionAdapter(MainActivity activity)
        {
            this.activity = activity;
        }

        public PendingIntent CreateCurrentContentIntent(IPlayer p0)
        {


            Intent intent = new Intent(Application.Context, typeof(MainActivity));
            return PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);
        }

        ICharSequence PlayerNotificationManager.IMediaDescriptionAdapter.GetCurrentContentTextFormatted(IPlayer p0)
        {
            ICharSequence Text = new Java.Lang.String(mediaSession.Controller.Metadata.Description.Description);
            return Text;
        }

        ICharSequence PlayerNotificationManager.IMediaDescriptionAdapter.GetCurrentContentTitleFormatted(IPlayer p0)
        {
            ICharSequence Title = new Java.Lang.String(mediaSession.Controller.Metadata.Description.Title);
            return Title;
        }
        ICharSequence PlayerNotificationManager.IMediaDescriptionAdapter.GetCurrentSubTextFormatted(IPlayer p0)
        {
            ICharSequence subText = new Java.Lang.String(mediaSession.Controller.Metadata.Description.Subtitle);
            return subText;
        }
        public Bitmap GetCurrentLargeIcon(IPlayer p0, PlayerNotificationManager.BitmapCallback p1)
        {
            Bitmap bitmap = BitmapFactory.DecodeResource(activity.Resources, Resource.Mipmap.ic_launcher);
            return bitmap;

        }


    }




    // private string mbOAuthClientID = "pJgWZTnefMbi-W1UUyDpw46-2TF1AlnR";
       // private string mbOAuthClientSecret = "5t4T4OEHZ8Z3OGn8We6bdp09LwdyY_xM";
*/











