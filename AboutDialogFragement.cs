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
using Google.Android.Material.AppBar;

namespace sRadio
{
    [Activity(Label = "AboutActivity")]
    public class AboutDialogFragement : AndroidX.Fragment.App.DialogFragment
    {

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle(StyleNormal, Resource.Style.DialogTheme);
            var ma = (MainActivity)this.Activity;
          //  ma.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.HideNavigation | SystemUiFlags.ImmersiveSticky);

        }
        //toolbar.SetNavigationIcon(Resource.Mipmap.ic_launcher_foreground);
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment

            var view = inflater.Inflate(Resource.Layout.about_dialog_fragement, container);

            var backButton = (ImageButton)view.FindViewById(Resource.Id.about_dialog_back_button);

            backButton.Click += (sender, e) =>
            {
                Dismiss();
            };

        
            return view;

            // Create your application here


        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
           
        }
    }
}