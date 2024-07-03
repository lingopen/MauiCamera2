using Android.Content;
using Android.Util;
using Java.Lang;
using Color = Android.Graphics.Color;
using Paint = Android.Graphics.Paint;
using RectF = Android.Graphics.RectF;

namespace MauiCamera2.Platforms.Droid.Controls
{
    public class FaceViewEx : Android.Views.View
    {
        Paint? mPaint;
        private string mCorlor = "#42ed45";
        private List<RectF>? mFaces = null;
        private int mRatioWidth = 0;
        private int mRatioHeight = 0;
        public FaceViewEx(Context context) : base(context, null)
        {
            Init();
        }

        public FaceViewEx(Context context, IAttributeSet attrs) : base(context, attrs, 0)
        {
            Init();
        }

        public FaceViewEx(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Init();
        }
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
            mFaces = faces;
            Invalidate();
        }
        public void Clear()
        {
            Invalidate();
        }
        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            base.OnDraw(canvas);
            //绘制矩形框
            if (mPaint != null && mFaces != null && mFaces.Any())
            {
                try
                {
                    foreach (var face in mFaces)
                    {
                        canvas.DrawRect(face, mPaint);
                        //canvas.DrawText($"left:{face.Left}, top:{face.Top},right:{face.Right},bottom:{face.Bottom}", 50, 50, mPaint);
                    }
                }
                catch (Java.Lang.Exception)
                {
                     
                }
                
            } 
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
