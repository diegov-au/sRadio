using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Google.Android.Material.Card;
using sRadio.DataModel;



namespace sRadio
{
    class MediaSearchItemAdapter : RecyclerView.Adapter
    {
        public event EventHandler<MediaSearchItemAdapterClickEventArgs> ItemClick;
        public event EventHandler<MediaSearchItemAdapterClickEventArgs> ItemLongClick;
    
        List<StationSearchItem> mediaSearchItems;

        public string accentColor = "#3498db";
        public string defaultColor = "#2a2a2a";

        public int selectedPos = RecyclerView.NoPosition;

        List<int> stationAddedPos;

        public MediaSearchItemAdapter(List<StationSearchItem> mediaSearchItems)
        {
            this.mediaSearchItems = mediaSearchItems;

        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            View itemView = LayoutInflater.From(parent.Context).Inflate((Resource.Layout.search_row_item), parent, false);

            var vh = new MediaSearchItemAdapterViewHolder(itemView, OnClick, OnLongClick);

            //our buttons click listener
            vh.stationAddButton.Click += (s, e) => { StationAddButton(vh); };

            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            // Replace the contents of the view with that element
            var holder = viewHolder as MediaSearchItemAdapterViewHolder;
            InitializeStationAddedPos();

            holder.stationName.Text = mediaSearchItems[position].name;
            holder.stationGenre.Text = mediaSearchItems[position].genre;
            holder.stationContentTypeAndBitrate.Text = mediaSearchItems[position].contentType + " - " + mediaSearchItems[position].bitRate +" Kbps"; 
            Glide.With(holder.stationLogo.Context)
                .Load(mediaSearchItems[position].logoUrl)
                .Placeholder(Resource.Drawable.ic_headset_white_24dp)
                .Into(holder.stationLogo);

            //sets background to blue or default based on if row is selected
            holder.cardView.SetCardBackgroundColor(selectedPos == position ? Android.Graphics.Color.ParseColor(accentColor) : Android.Graphics.Color.ParseColor(defaultColor)) ;

            //sets button image to plus or checked depending on whether a station has been added in the current search cycle
            holder.stationAddButton.SetImageResource(stationAddedPos[position] == 0 ? Resource.Drawable.ic_add_white_48dp : Resource.Drawable.ic_check_white_48dp);

        }
        public override int ItemCount => mediaSearchItems.Count;
        void OnClick(MediaSearchItemAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(MediaSearchItemAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        private void InitializeStationAddedPos()
        {
            if (stationAddedPos == null && mediaSearchItems.Count > 0)
            {
                stationAddedPos = new List<int>(new int[mediaSearchItems.Count]);
            }
        }
        private async void StationAddButton(RecyclerView.ViewHolder viewHolder)
        {
            var holder = viewHolder as MediaSearchItemAdapterViewHolder;

            if (stationAddedPos[holder.AdapterPosition] == 1)
                return;

            var ma = (MainActivity)viewHolder.ItemView.Context;
            var dlg = new MediaUtils();

            var result = await dlg.DisplayCustomDialog(viewHolder.ItemView.Context, "Add to favourites?", "Would you like to add " + mediaSearchItems[holder.AdapterPosition].name + " to your favourites?", "Yes", "No");

            if (result == "Yes")
            {
                var mi = new MediaItemClass();


                mi.MediaName = mediaSearchItems[holder.AdapterPosition].name;
                mi.MediaUri = mediaSearchItems[holder.AdapterPosition].baseUrl;
                mi.MediaDescription = mediaSearchItems[holder.AdapterPosition].genre;
                mi.MediaLogoUrl = mediaSearchItems[holder.AdapterPosition].logoUrl;

                //to prevent the tick for showing when exceeding favourites limit in free mode we check to see if app is locked and items >2
                //The actual message to let the user now they have exceeded the limits of favourites in free mode is handled by
                //AddToFavourites method
                if (!(MainActivity.isUnlocked == 0 && MainActivity.mediaItems.Count > 2))
                {
                    holder.stationAddButton.SetImageResource(Resource.Drawable.ic_check_white_48dp);
                    stationAddedPos[holder.AdapterPosition] = 1;
                }

                ma.AddToFavourites(mi);

            }

        }
        public void ResetAdapterProperties()
        {
            selectedPos = RecyclerView.NoPosition;
            stationAddedPos = null;
        }
         
    }

    public class MediaSearchItemAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView stationName { get; set; }
        public TextView stationDescription { get; set; }
        public TextView stationContentTypeAndBitrate { get; set; }
        public TextView stationCurrentTrack { get; set; }
        public TextView stationGenre { get; set; }
        public ImageView stationLogo { get; set; }
        public MaterialCardView cardView { get; set; }
        public ImageButton stationAddButton { get; set; }
        public MediaSearchItemAdapterViewHolder(View itemView, Action<MediaSearchItemAdapterClickEventArgs> clickListener,
                            Action<MediaSearchItemAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            stationName = (TextView)itemView.FindViewById(Resource.Id.search_station_name_textview);
            stationGenre = (TextView)itemView.FindViewById(Resource.Id.search_station_genre_textview);
            stationContentTypeAndBitrate = (TextView)itemView.FindViewById(Resource.Id.search_media_type_and_bitrate_textview);
            stationLogo = (ImageView)itemView.FindViewById(Resource.Id.search_media_logo);
            stationAddButton = (ImageButton)itemView.FindViewById(Resource.Id.search_station_add_button);
            cardView = (MaterialCardView)itemView.FindViewById(Resource.Id.search_material_cardview);
           // stationAddButton.Click += (s, e) => { StationAddButton(itemView, AdapterPosition); };

            itemView.Click += (sender, e) => clickListener(new MediaSearchItemAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new MediaSearchItemAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }

    }
    public class MediaSearchItemAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }

       
    }

}


