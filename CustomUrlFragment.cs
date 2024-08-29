using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gestures;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using Google.Android.Material.AppBar;
using Java.Security;
using sRadio.DataModel;

namespace sRadio
{
    public class CustomUrlFragment : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
           // Activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.Visible | SystemUiFlags.ImmersiveSticky);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment

            var view = inflater.Inflate(Resource.Layout.custom_url_fragment, container, false);


            var buttonClose = (ImageButton)view.FindViewById(Resource.Id.custom_url_close_button);
            var buttonSave = (TextView)view.FindViewById(Resource.Id.custom_url_save_button);
            var buttonTest = (Button)view.FindViewById(Resource.Id.custom_url_test_button);
            var imageViewLogo = (ImageView)view.FindViewById(Resource.Id.custom_url_image);
            var editTextStationName = (EditText)view.FindViewById(Resource.Id.custom_url_stationname_edittext);
            var editTextDescription = (EditText)view.FindViewById(Resource.Id.custom_url_description_edittext);
            var editTextLogoUrl = (EditText)view.FindViewById(Resource.Id.custom_url_logourl_edittext);
            var editTextStreamUrl = (EditText)view.FindViewById(Resource.Id.custom_url_streamurl_edittext);


            editTextStationName.TextChanged += (sender, e) =>
            {
                var text = e.Text.ToString();
                if ((text.Length > 1) && (editTextStreamUrl.Text.Contains("://") && editTextStreamUrl.Text.Length > 3))
                {
                    buttonTest.Enabled = true;
                    buttonSave.Enabled = true;
                    buttonSave.SetTextColor(ContextCompat.GetColorStateList(Activity.ApplicationContext, Resource.Color.white));
                }
                else
                {
                    buttonTest.Enabled = false;
                    buttonSave.Enabled = false;
                    buttonSave.SetTextColor(ContextCompat.GetColorStateList(Activity.ApplicationContext, Resource.Color.material_grey_600));
                }
            };
            editTextStreamUrl.TextChanged += (sender, e) =>
            {
                var text = e.Text.ToString();
                if ((text.Contains("://") && text.Length > 3) && editTextStationName.Text.Length > 1)
                {
                    buttonTest.Enabled = true;
                    buttonSave.Enabled = true;
                    buttonSave.SetTextColor(ContextCompat.GetColorStateList(Activity.ApplicationContext, Resource.Color.white));


                }
                else
                {
                    buttonTest.Enabled = false;
                    buttonSave.Enabled = false;
                    buttonSave.SetTextColor(ContextCompat.GetColorStateList(Activity.ApplicationContext, Resource.Color.material_grey_600));
                }
            };

            buttonSave.Click += (sender, e) =>
            {
                var ma = (MainActivity)this.Activity;
                var mediaItem = new MediaItemClass();

                mediaItem.MediaName = editTextStationName.Text;
                mediaItem.MediaDescription = editTextDescription.Text;
                mediaItem.MediaLogoUrl = editTextLogoUrl.Text;
                mediaItem.MediaUri = editTextStreamUrl.Text;

                ma.AddToFavourites(mediaItem);

                CloseFragement();

            };
            buttonTest.Click += async (sender, e) =>
            {
                var ma = (MainActivity)this.Activity;
                var imm = (InputMethodManager)ma.GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(buttonSave.WindowToken, 0);

                ma.PlayFromFragment(editTextStreamUrl.Text, editTextStationName.Text);

                var imageDownload = new MediaUtils();

                imageViewLogo.SetImageBitmap(await imageDownload.DownloadBitmapAsync(editTextLogoUrl.Text));

            };
            buttonClose.Click += (sender, e) =>
            {
                CloseFragement();
            };

            return view;
        }

        public void CloseFragement()
        {
            var ma = (MainActivity)this.Activity;
            var imm = (InputMethodManager)ma.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(this.View.WindowToken, 0);

           
            Activity.SupportFragmentManager.BeginTransaction()
            .SetCustomAnimations(Resource.Animation.enter_from_right, Resource.Animation.exit_to_right, Resource.Animation.enter_from_right, Resource.Animation.abc_slide_out_bottom)
            .Remove(this)
            .Commit();
        }
        



    }
}