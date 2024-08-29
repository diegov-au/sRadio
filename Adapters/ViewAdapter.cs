using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gestures;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager.Widget;
using AndroidX.ViewPager2.Adapter;
using Java.Lang;

namespace sRadio.Resources
{
    class ViewAdapter : RecyclerView.Adapter

    {
        private Context context;
        private List<View> viewList = new List<View>();
        int pos = 0;

        public ViewAdapter(Context context)
        {
            this.context = context;
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup container, int viewType)
        {

            var layout = viewList[pos]; 
            layout.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

            pos++;

            return new ViewAdapterViewHolder(layout);
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {

          
            
        }

  
        public override int ItemCount => viewList.Count;

        public void addView(View view)
        {
            viewList.Add(view);
            this.NotifyItemInserted(pos);
         
           
        }
        public class ViewAdapterViewHolder : RecyclerView.ViewHolder
        {
          
            public ViewAdapterViewHolder(View itemView)
                : base(itemView)
            {
              
            }
        }
    }
}
