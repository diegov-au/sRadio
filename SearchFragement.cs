using System;
using System.Collections;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using sRadio.DataModel;

namespace sRadio
{
    public class SearchFragment : AndroidX.Fragment.App.Fragment
    {
        List<StationSearchItem> stationSearchResults;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            stationSearchResults = new List<StationSearchItem>();
            // Activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.Visible | SystemUiFlags.ImmersiveSticky);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            var view = inflater.Inflate(Resource.Layout.search_fragment, container, false);

            return view;
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var items = new List<string>();

            var buttonCancel = (ImageButton)view.FindViewById(Resource.Id.search_close_button);
            var loadingSpinner = (RelativeLayout)view.FindViewById(Resource.Id.loadingPanel);
            var searchView = (AndroidX.AppCompat.Widget.SearchView)view.FindViewById(Resource.Id.searchview_search_fragment);

            var recyclerView = (RecyclerView)view.FindViewById(Resource.Id.search_recycler_view);
            var adapter = new MediaSearchItemAdapter(stationSearchResults);

            recyclerView.SetAdapter(adapter);
            recyclerView.SetLayoutManager(new LinearLayoutManager(recyclerView.Context));


            searchView.QueryHint = "Search for stations";
            searchView.OnActionViewExpanded();
            searchView.Iconified = false;

            searchView.QueryTextSubmit += async (s, e) =>
            {

                var ma = (MainActivity)Activity;
                var stationSearch = new MediaUtils();

                loadingSpinner.Visibility = ViewStates.Visible;

                searchView.ClearFocus();
                searchView.SetQuery("", false);

                var results = new List<StationSearchItem>();

                if (MainActivity.radioDirectory==0)
                    results = await stationSearch.SearchShoutCastDirectoryAsync(e.NewText);

                if (MainActivity.radioDirectory == 1)
                    results = await stationSearch.SearchDarFmDirectoryAsync(e.NewText);

                if (MainActivity.radioDirectory == 2)
                    results = await stationSearch.SearchRadioTimeDirectoryAsync(e.NewText);

                stationSearchResults.Clear();
                stationSearchResults.AddRange(results);

                adapter.ResetAdapterProperties();
                adapter.NotifyDataSetChanged();

                loadingSpinner.Visibility = ViewStates.Gone;
                e.Handled = true;

            };

            adapter.ItemClick += (sender, e) =>
            {

                var ma = (MainActivity)Activity;

                adapter.NotifyItemChanged(adapter.selectedPos);
                adapter.selectedPos = e.Position;
                adapter.NotifyItemChanged(e.Position);

                ma.PlayFromFragment(stationSearchResults[e.Position].baseUrl, stationSearchResults[e.Position].name);

            };

            adapter.ItemLongClick += async (sender, e) =>
            {

            };


            buttonCancel.Click += (sender, e) =>
            {
                CloseFragement();
            };
        }
        public override void OnStart()
        {
            base.OnStart();

            AnimateLayout(Resource.Id.search_view_rv_layout, Resource.Animation.abc_grow_fade_in_from_bottom, 300);
            AnimateLayout(Resource.Id.search_toolbar, Resource.Animation.enter_from_right, 300);

        }
        private void AnimateLayout(int layoutId, int animationId, int duration)
        {
            View layout = View.FindViewById(layoutId);
            var animation = AnimationUtils.LoadAnimation(Activity.Application, animationId);
            animation.Duration = duration;
            layout.StartAnimation(animation);

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
