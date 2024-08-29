using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Java.Security;
using sRadio.DataModel;

namespace sRadio
{
    public class FavouritesFragment : AndroidX.Fragment.App.Fragment
    {
        MediaFavouriteItemAdapter adapter;
        View view;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            
            view = inflater.Inflate(Resource.Layout.favourites_fragement, container, false);
            
            return view;

           // return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
           
            InitalizeRecyclerViews(view);
        }

        public void InitalizeRecyclerViews(View view)
        {
            var ma = (MainActivity)this.Activity;
            
            //favourites
            var rvFavourites = view.FindViewById<RecyclerView>(Resource.Id.recyclerViewFavourites);
            rvFavourites.SetLayoutManager(new LinearLayoutManager(rvFavourites.Context));
            adapter = new MediaFavouriteItemAdapter(MainActivity.mediaItems);
            adapter.ItemClick += OnRecyclerViewItemClick;
            adapter.ItemLongClick += OnRecyclerViewLongItemClick;
            rvFavourites.SetAdapter(adapter);

            /*
             * disables animation effects in RecycleView that may cause flashing
             * var a = (SimpleItemAnimator)rvFavourites.GetItemAnimator();
            a.SupportsChangeAnimations = false;*/


        }
        public void OnRecyclerViewItemClick(object sender, EventArgs e)
        {
            var ma = (MainActivity)this.Activity;

            var adapter = (MediaFavouriteItemAdapter)sender;

            var obj = (sRadio.MediaFavouriteItemAdapterClickEventArgs)e;

            int position = obj.Position;
           
            adapter.selectedPos = position; //sets the selectitem position in the adapter;
                    
            MainActivity.currentMediaPlaylistItem = position;
           
            ma.PlayFromFragment(MainActivity.mediaItems[position].MediaUri, MainActivity.mediaItems[position].MediaName);

        }
        async void OnRecyclerViewLongItemClick(object sender, EventArgs e)
        {
            var ma = (MainActivity)this.Activity;
            var adapter = (MediaFavouriteItemAdapter)sender;
            var obj = (sRadio.MediaFavouriteItemAdapterClickEventArgs)e;

            int position = obj.Position;
            var dlg = new MediaUtils();


            var result = await dlg.DisplayCustomDialog(ma, "Remove Favourite?", "Would you like to remove " + MainActivity.mediaItems[position].MediaName + " from your favourites?", "Yes", "No");
            if (result == "Yes")
            {
                MainActivity.mediaItems.RemoveAt(position);
                adapter.NotifyItemRemoved(position);
                MainActivity.SaveAppSettings();
               
            }

        }

        public void SetBackground (GradientDrawable gd)
        {
            if (view == null)
                return;
            view.Background = gd;

        }
        public MediaFavouriteItemAdapter GetAdapter()
        {
            return adapter;
        }
    }
}