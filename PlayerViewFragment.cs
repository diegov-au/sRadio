using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Drm;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using Com.Google.Android.Exoplayer2.UI;
using JP.Wasabeef.BlurryLib;

namespace sRadio
{
    public class PlayerViewFragment : AndroidX.Fragment.App.Fragment
    {
        public View view;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            view = inflater.Inflate(Resource.Layout.player_view_fragment, container, false);
            
            return view;

            //return base.OnCreateView(inflater, container, savedInstanceState);
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            var ma = (MainActivity)Activity;
            var buttonSkipNext = (ImageButton)view.FindViewById(Resource.Id.player_button_next);
            var buttonSkipPrevious = (ImageButton)view.FindViewById(Resource.Id.player_button_prev);
            
            InitiatePlayerView();
            SetPlayerViewLabelMetadata();
            SetPlayerViewArtwork();


            buttonSkipNext.Click += (sender, e) =>
            {
                MainActivity.PlayListActionManager(1);
            };

            buttonSkipPrevious.Click += (sender, e) =>
            {
                MainActivity.PlayListActionManager(0);
            };

        }

        public void InitiatePlayerView()
        {
            var ma = (MainActivity)Activity;

           MainActivity.playerView = (PlayerControlView)view.FindViewById(Resource.Id.local_player_view);
           MainActivity.playerView.RequestFocus();
           MainActivity.playerView.SetControlDispatcher(new MainActivity.PlayerViewControlDispatcher(ma));
           MainActivity.playerView.ShowTimeoutMs = -1;
           MainActivity.playerView.Player = MainActivity.player;
        }

        public void SetPlayerViewLabelMetadata()
        {
            if (view == null)
                return;

            var titleLabel = (TextView)view.FindViewById(Resource.Id.metadata_track);
            var artistLabel = (TextView)view.FindViewById(Resource.Id.metadata_artist);
            var mediaNameLabel = (TextView)view.FindViewById(Resource.Id.media_name);

            titleLabel.Text = MainActivity.CurrentMediaItemMetadataState.MediaTitle;
            artistLabel.Text = MainActivity.CurrentMediaItemMetadataState.MediaArtist;
            mediaNameLabel.Text = MainActivity.CurrentMediaItemMetadataState.MediaName;
         
        }

        public void SetPlayerViewArtwork()
        {
            if (view == null)
                return;

            var playerImage = view.FindViewById<ImageView>(Resource.Id.albumart);
            var blurredArtworkBackgroundImage = view.FindViewById<ImageView>(Resource.Id.albumartblurred);

            var albumart = MainActivity.CurrentMediaItemMetadataState.MediaArt;

            playerImage.SetImageBitmap(albumart);

            Blurry.With(Application.Context).From(albumart).Into(blurredArtworkBackgroundImage);

        }
        public void SetMediaMetadataLabel(string mediaMetadata)
        {
            if (view == null)
                return;
            var mediaMetadataLabel = (TextView)view.FindViewById(Resource.Id.media_metadata);
            mediaMetadataLabel.Text = mediaMetadata;
        }
        public void SetTitleBackgroundGradient (Android.Graphics.Drawables.GradientDrawable gd)
        {
            if (view == null)
                return;
            var tb = view.FindViewById(Resource.Id.title_player_view_linearlayout);
            tb.Background = gd;
        }

        public void SetTrackTitleColor(int color)
        {
            if (view == null)
                return;
            var titleTrackLabel = (TextView)view.FindViewById(Resource.Id.metadata_track);

            titleTrackLabel.SetTextColor(new Color(color));
        }
    
    }
}