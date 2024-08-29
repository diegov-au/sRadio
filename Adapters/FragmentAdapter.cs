using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using AndroidX.Lifecycle;
using AndroidX.ViewPager2.Adapter;

namespace sRadio.Resources
{
    class FragmentAdapter : FragmentStateAdapter
    {
        List<AndroidX.Fragment.App.Fragment> fragmentList = new List<AndroidX.Fragment.App.Fragment>();

        public FragmentAdapter (AndroidX.Fragment.App.FragmentActivity fragementActivity)
        :base(fragementActivity)   { }

        public FragmentAdapter(AndroidX.Fragment.App.FragmentManager fragmentManager, Lifecycle lifecycle)
        : base(fragmentManager,lifecycle) { }

        public override AndroidX.Fragment.App.Fragment CreateFragment(int position)
        {
      
        return fragmentList[position];
        }

        public void addFragment(AndroidX.Fragment.App.Fragment fragment)
        {
            fragmentList.Add(fragment);
        }

      
        public override int ItemCount => fragmentList.Count;
       
            

    }
}
