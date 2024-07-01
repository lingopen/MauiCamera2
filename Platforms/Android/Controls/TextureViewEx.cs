using Android.Content;
using Android.Util;
using Android.Views;
using Java.Lang;

namespace MauiCamera2.Platforms.Droid.Controls
{
    public  class TextureViewEx: TextureView
    {
        private int mRatioWidth = 0;
        private int mRatioHeight = 0;

        public TextureViewEx(Context context) : base(context, null)
        {
        }

        public TextureViewEx(Context context, IAttributeSet attrs) : base(context, attrs, 0)
        {
        }

        public TextureViewEx(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        } 
        public void SetAspectRatio(int width, int height)
        {
            if (width < 0 || height < 0)
            {
                throw new IllegalArgumentException("Size cannot be negative.");
            }
            mRatioWidth = width;
            mRatioHeight = height;
            RequestLayout();
        }
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            int width = MeasureSpec.GetSize(widthMeasureSpec);
            int height = MeasureSpec.GetSize(heightMeasureSpec);
            if (0 == mRatioWidth || 0 == mRatioHeight)
            {
                SetMeasuredDimension(width, height);
            }
            else
            {
                if (width < height * mRatioWidth / mRatioHeight)
                {
                    SetMeasuredDimension(width, width * mRatioHeight / mRatioWidth);
                }
                else
                {
                    SetMeasuredDimension(height * mRatioWidth / mRatioHeight, height);
                }
            }

        }
    }
}
