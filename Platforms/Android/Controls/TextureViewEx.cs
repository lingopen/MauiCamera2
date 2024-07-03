using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Java.Lang;
using Color = Android.Graphics.Color;
using Paint = Android.Graphics.Paint;
using RectF = Android.Graphics.RectF;
namespace MauiCamera2.Platforms.Droid.Controls
{
    public class TextureViewEx : TextureView
    {
        private int mRatioWidth = 0;
        private int mRatioHeight = 0;
        private int mRealWidth = 0;
        private int mRealHeight = 0;
        public TextureViewEx(Context context) : base(context, null)
        {
            Init();
        }

        public TextureViewEx(Context context, IAttributeSet attrs) : base(context, attrs, 0)
        {
            Init();
        }

        public TextureViewEx(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Init();
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
        public int GetmRealWidth()
        {
            return mRealWidth;
        }

        public int GetmRealHeight()
        {
            return mRealHeight;
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
                    mRealWidth = width;
                    mRealHeight = width * mRatioHeight / mRatioWidth;
                    SetMeasuredDimension(width, mRealHeight);
                }
                else
                {
                    mRealHeight = height;
                    mRealWidth = height * mRatioWidth / mRatioHeight;
                    SetMeasuredDimension(mRealWidth, height);
                }
            }

        }

        Paint? mPaint;
        private string mCorlor = "#42ed45";
        private List<RectF>? mFaces = null;
        private void Init()
        {
            mPaint = new Paint();
            mPaint.Color = Color.ParseColor(mCorlor);
            mPaint.SetStyle(Paint.Style.Stroke);
            mPaint.StrokeWidth = TypedValue.ApplyDimension(ComplexUnitType.Dip, 1f, Context?.Resources?.DisplayMetrics);
            mPaint.AntiAlias = true;
        }
        public void SetFaces(List<RectF> faces)
        { 
             try
             {
                 var canvas = LockCanvas();
                 if (canvas != null)
                 { 
                     canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
                     foreach (var rect in faces)
                     {
                         canvas.Save();
                         canvas.DrawRect(rect.Left, rect.Top, rect.Right, rect.Bottom, mPaint);
                         canvas.Restore();
                     }
                     UnlockCanvasAndPost(canvas);
                 }
             }
             catch
             {
             
             }
        } 
         
    }
}
