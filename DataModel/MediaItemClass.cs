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

namespace sRadio.DataModel
{
    public class MediaItemClass
    {
        public string MediaName { get; set; }
        public string MediaDescription { get; set; }
        public MediaTypes MediaType { get; set; } // eg. StreamingRadio, LocalFile, PodCast
        public string MediaUri { get; set; }
        public string MediaLogoUrl { get; set; }
    }

    public enum MediaTypes
    {
        StreamingRadio,
        LocalMedia,
        RemoteMedia,
        PodCast
    }
}