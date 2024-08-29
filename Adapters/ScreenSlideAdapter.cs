using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gestures;
using Android.OS;

using Android.Views;
using Android.Widget;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager.Widget;
using AndroidX.ViewPager2.Adapter;
using AndroidX.Fragment.App;
using Java.Lang;
using Android.Net.Wifi.Aware;
using Android.Support.V4.App;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace sRadio.Resources
{
    class ScreenSlidePageAdapter : FragmentStateAdapter 
     {
        private List<AndroidX.Fragment.App.Fragment> fragementArrayList = new List<AndroidX.Fragment.App.Fragment>();

        public ScreenSlidePageAdapter(AndroidX.Fragment.App.FragmentActivity fa) : base (fa)
        {

        }
        public ScreenSlidePageAdapter (AndroidX.Fragment.App.FragmentManager fm, Lifecycle lifecycle) : base(fm, lifecycle) { }
        public override AndroidX.Fragment.App.Fragment CreateFragment(int p0)
        {
            return fragementArrayList[p0];
        }

        public override int ItemCount => fragementArrayList.Count;

       
    
        public void addFragment(AndroidX.Fragment.App.Fragment fragment)
        {
            fragementArrayList.Add(fragment);
        }

    }
}
