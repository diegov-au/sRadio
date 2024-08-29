using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

//used to store state of currently playing media

namespace sRadio.DataModel
{
    public class MediaMetadataStateClass
    {
        public string MediaName { get; set; }
        public string MediaTitle{ get; set; }
        public string MediaArtist { get; set; }
        public string MediaAlbum { get; set; }
        public string MediaBitrate { get; set; }
        public string MediaSamplerate { get; set; }
        public string MediaCodec { get; set; }
        public Bitmap MediaArt { get; set; }
   
    }
}