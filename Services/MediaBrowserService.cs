using System;
using System.Text;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Media.Browse;
using Android.Media.Session;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Android.Views;
using Android.Widget;
using AndroidX.Media;
using Com.Google.Android.Exoplayer2.Source;

namespace sRadio
{
    [Service(Enabled = true, Exported = true, Name = "com.diegoviso.sRadio.MediaBrowserService")]
    [IntentFilter(new[] { "android.media.browse.MediaBrowserService" })]
    public class sRadioBrowserService : MediaBrowserServiceCompat
    {
        private static String MY_MEDIA_ROOT_ID = "__ROOT__";
        private static String MY_EMPTY_MEDIA_ROOT_ID = "empty_root_id";

     //  private MediaSessionCompat mediaSession;

      //  MediaBrowserService.PackageValidator packageValidator;


        public override void OnCreate()
        {
            base.OnCreate();

            if (MainActivity.mediaSession.SessionToken != null)
                SessionToken = MainActivity.mediaSession.SessionToken;

            // var mediaBrowser = new MediaBrowserCompat(this, typeof(MainActivity),);

            /*     mediaSession = new MediaSessionCompat(ApplicationContext, "sRadioBrowserService");
                 mediaSession.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons| MediaSessionCompat.FlagHandlesTransportControls);
                 stateBuilder = new PlaybackStateCompat.Builder()
                                .SetActions(PlaybackStateCompat.ActionPlay| PlaybackStateCompat.ActionStop | PlaybackStateCompat.ActionPlayPause | PlaybackStateCompat.ActionSkipToNext | PlaybackState.ActionSkipToPrevious);
                 mediaSession.SetPlaybackState(stateBuilder.Build());
                 mediaSession.SetCallback(new MediaSessionCallback(this));*/



            // if(MainActivity.mediaSession.SessionToken!=null)
            //    SessionToken =MainActivity.mediaSession.SessionToken;

            //packageValidator = new PackageValidator(this);



            /* mediaBrowser = new MediaBrowserCompat(this,
                 new ComponentName(this, typeof(sRadioBrowserService).Name),
                 new MediaBrowserCallback(), null);*/

        }
        public override BrowserRoot OnGetRoot(string clientPackageName, int clientUid, Bundle rootHints)
        {
           /* if (!packageValidator.IsCallerAllowed(this, clientPackageName, clientUid))
            {
                System.Console.WriteLine("OnGetRoot: IGNORING request from untrusted package "
                + clientPackageName);
                return null;
            }*/

            return new BrowserRoot(MediaIDHelper.MediaIdRoot, null);
        }
        public override void OnLoadChildren(string parentId, Result result)
        {


                var mediaItems = new JavaList<MediaBrowserCompat.MediaItem>();

                if (parentId == MY_MEDIA_ROOT_ID)
                {

                mediaItems.Add(new MediaBrowserCompat.MediaItem(
                     new MediaDescriptionCompat.Builder()
                     .SetMediaId(MediaIDHelper.MediaIdMusicsByFavourties)
                     .SetTitle("Favourites")
                     .SetIconUri(Android.Net.Uri.Parse("android.resource://" +
                     "com.diegoviso.sradio/drawable/ic_favourite"))
                     .SetSubtitle("Saved streams")
                     .Build(), MediaBrowserCompat.MediaItem.FlagBrowsable ));

                mediaItems.Add(new MediaBrowserCompat.MediaItem(
                   new MediaDescriptionCompat.Builder()
                   .SetMediaId(MediaIDHelper.MediaIdMusicsByRecent)
                   .SetTitle("Recent")
                   .SetIconUri(Android.Net.Uri.Parse("android.resource://" +
                   "com.diegoviso.sradio/drawable/ic_replay"))
                   .SetSubtitle("Recently played streams")
                   .Build(), MediaBrowserCompat.MediaItem.FlagBrowsable));

                // build the MediaItem objects for the top level,
                // and put them in the mediaItems list
            }
                else if(parentId == MediaIDHelper.MediaIdMusicsByFavourties)
                {
                    for(int i = 0; i < MainActivity.mediaItems.Count; i++)
                    {
                    mediaItems.Add(new MediaBrowserCompat.MediaItem(
                         new MediaDescriptionCompat.Builder()
                         .SetMediaId(i.ToString())
                         .SetTitle(MainActivity.mediaItems[i].MediaName)
                         .SetSubtitle(MainActivity.mediaItems[i].MediaDescription)
                         .SetIconUri(Android.Net.Uri.Parse("android.resource://" +
                            "com.diegoviso.sradio/drawable/ic_album"))
                         .Build(), MediaBrowserCompat.MediaItem.FlagPlayable));

                    }
                    
                    
                    // examine the passed parentMediaId to see which submenu we're at,
                    // and put the children of that menu in the mediaItems list
                }


                result.SendResult(mediaItems);

        }

        public override void OnSearch(string query, Bundle extras, Result result)
        {
            base.OnSearch(query, extras, result);
        }

    }

    public static class MediaIDHelper
    {
        public const string MediaIdRoot = "__ROOT__";
        public const string MediaIdMusicsByFavourties = "__BY_FAVOURITES__";
        public const string MediaIdMusicsByRecent = "__BY_RECENT__";
        public const string MediaIdMusicsByGenre = "__BY_GENRE__";
        public const string MediaIdMusicsBySearch = "__BY_SEARCH__";

        const char CategorySeparator = '/';
        const char LeafSeparator = '|';

        public static string CreateMediaID(string musicID, params string[] categories)
        {
            var sb = new StringBuilder();
            if (categories != null && categories.Length > 0)
            {
                sb.Append(categories[0]);
                for (var i = 1; i < categories.Length; i++)
                {
                    sb.Append(CategorySeparator).Append(categories[i]);
                }
            }
            if (musicID != null)
            {
                sb.Append(LeafSeparator).Append(musicID);
            }
            return sb.ToString();
        }

        public static string CreateBrowseCategoryMediaID(string categoryType, string categoryValue)
        {
            return categoryType + CategorySeparator + categoryValue;
        }

        public static string ExtractMusicIDFromMediaID(string mediaId)
        {
            int pos = mediaId.IndexOf(LeafSeparator);
            return pos >= 0 ? mediaId.Substring(pos + 1) : null;
        }

        public static string[] GetHierarchy(string mediaId)
        {
            int pos = mediaId.IndexOf(LeafSeparator);
            if (pos >= 0)
                mediaId = mediaId.Substring(0, pos);
            return mediaId.Split(CategorySeparator);
        }

        public static string ExtractBrowseCategoryValueFromMediaID(string mediaId)
        {
            string[] hierarchy = GetHierarchy(mediaId);
            if (hierarchy != null && hierarchy.Length == 2)
                return hierarchy[1];
            return null;
        }

        static bool IsBrowseable(string mediaId)
        {
            return mediaId.IndexOf(LeafSeparator) < 0;
        }

        public static string GetParentMediaID(string mediaId)
        {
            var hierarchy = GetHierarchy(mediaId);

            if (!IsBrowseable(mediaId))
                return CreateMediaID(null, hierarchy);

            if (hierarchy == null || hierarchy.Length <= 1)
                return MediaIdRoot;

            var parentHierarchy = new string[hierarchy.Length - 1];
            Array.Copy(hierarchy, parentHierarchy, hierarchy.Length - 1);
            return CreateMediaID(null, parentHierarchy);
        }
    }
}