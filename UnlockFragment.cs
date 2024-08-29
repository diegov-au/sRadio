using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.BillingClient.Api;
using Android.Content;
using Android.Gestures;
using Android.Graphics;
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

using sRadio.DataModel;

namespace sRadio
{
    public class UnlockFragment : AndroidX.Fragment.App.Fragment
    {

        View view;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.unlock_fragment, container, false);
            return view;
        }


        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            view.Clickable = true;
            var ma = (MainActivity)Activity;
            var buttonBack = (ImageButton)view.FindViewById(Resource.Id.unlock_back_button);
            var buttonUnlock = (Button)view.FindViewById(Resource.Id.unlock_button);

            if(Billing.SKU_Full_Price!="" && Billing.SKU_Full_Price != null)
                buttonUnlock.Text = "Unlock - " + Billing.SKU_Full_Price;
            else
            {
                buttonUnlock.Text = "Error";
                buttonUnlock.Enabled = false;
                var dlg = new MediaUtils();
                dlg.DisplayCustomDialog(ma, "Google Play Store Error", "There was an error communicating with the Google Play Store. Please check your network connection and restart the app.", "OK", null);
         
            }

          
            buttonUnlock.Click += (sender, e) =>
            {
                if (!Billing._isBillingServiceConnected)
                    return;
                buttonUnlock.Enabled = false; //lock button during the purchase flow so it cant be clicked again
                
                Billing.OnPurchaseSuccessful += OnPurchaseSuccessful; //subscribe to billing event handlers
                Billing.OnPurchaseFailed += OnPurchaseFailed;
                
                Billing.StartPurchase(ma);
            };

            buttonBack.Click += (sender, e) =>
            {
                CloseFragement();
            };


            RestoreSettings(view);

        }

        public void OnPurchaseSuccessful(object o, EventArgs e)
        {
            MainActivity.UnlockApp(true);
            RestoreSettings(view);
            Billing.OnPurchaseSuccessful -= OnPurchaseSuccessful;
            Billing.OnPurchaseFailed -= OnPurchaseFailed;


        }

        public void OnPurchaseFailed(object o, EventArgs e)
        {
            var buttonUnlock = (Button)view.FindViewById(Resource.Id.unlock_button);
            
            MainActivity.UnlockApp(false);
            buttonUnlock.Enabled = true; //unlock the buy button when purchase fails
            RestoreSettings(view);
            Billing.OnPurchaseSuccessful -= OnPurchaseSuccessful;
            Billing.OnPurchaseFailed -= OnPurchaseFailed;


        }
        public void RestoreSettings(View view)
        {
            var ma = (MainActivity)Activity;
            var buttonUnlock = (Button)view.FindViewById(Resource.Id.unlock_button);
            if (MainActivity.isUnlocked == 1)
            {
                buttonUnlock.Text = "Purchased, Thank you!";
                buttonUnlock.Enabled = false;
            }


        }
        public void CloseFragement()
        {
            var ma = (MainActivity)this.Activity;
            var imm = (InputMethodManager)ma.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(this.View.WindowToken, 0);

            Billing.OnPurchaseSuccessful -= OnPurchaseSuccessful;
            Billing.OnPurchaseFailed -= OnPurchaseFailed;

            Activity.SupportFragmentManager.BeginTransaction()
            .SetCustomAnimations(Resource.Animation.enter_from_right, Resource.Animation.exit_to_right, Resource.Animation.enter_from_right, Resource.Animation.abc_slide_out_bottom)
            .Remove(this)
            .Commit();
        }
        


    }
}