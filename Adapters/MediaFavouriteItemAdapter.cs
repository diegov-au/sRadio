using System;
using System.Collections.Generic;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using sRadio.DataModel;


namespace sRadio
{
    public class MediaFavouriteItemAdapter : RecyclerView.Adapter
    {
        public event EventHandler<MediaFavouriteItemAdapterClickEventArgs> ItemClick;
        public event EventHandler<MediaFavouriteItemAdapterClickEventArgs> ItemLongClick;
        
        List<MediaItemClass> mediaItems;
     
        public string accentColor = "#3498db";

        public int selectedPos = RecyclerView.NoPosition;

        public MediaFavouriteItemAdapter(List<MediaItemClass> mediaItems)
        {
            this.mediaItems = mediaItems;
          
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            View itemView = LayoutInflater.From(parent.Context).Inflate((Resource.Layout.favourites_row_item), parent, false);

            var vh = new MediaFavouriteItemAdapterViewHolder(itemView, OnClick, OnLongClick);
           
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            // Replace the contents of the view with that element
            var holder = viewHolder as MediaFavouriteItemAdapterViewHolder;
            holder.stationName.Text = mediaItems[position].MediaName;
            holder.stationDescription.Text = mediaItems[position].MediaDescription;
            Glide.With(holder.mediaLogo.Context)
                 .Load(mediaItems[position].MediaLogoUrl)
                 .Placeholder(Resource.Drawable.ic_headset_white_24dp)
                 .Into(holder.mediaLogo);

        }

        public override int ItemCount => mediaItems.Count;

        void OnClick(MediaFavouriteItemAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(MediaFavouriteItemAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }
   

    public class MediaFavouriteItemAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public TextView stationName { get; set; }
        public TextView stationDescription { get; set; }
        public ImageView mediaLogo { get; set; }

        public MediaFavouriteItemAdapterViewHolder(View itemView, Action<MediaFavouriteItemAdapterClickEventArgs> clickListener,
                            Action<MediaFavouriteItemAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            stationName = (TextView)itemView.FindViewById(Resource.Id.stationNameTextView);
            stationDescription = (TextView)itemView.FindViewById(Resource.Id.stationDescriptionTextView);
            mediaLogo = (ImageView)itemView.FindViewById(Resource.Id.mediaLogo);

            //TextView = v;
            itemView.Click += (sender, e) => clickListener(new MediaFavouriteItemAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new MediaFavouriteItemAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class MediaFavouriteItemAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }

       
    }
}