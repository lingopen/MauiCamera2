using Android.Content;
using Android.Util;
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
                catch (Exception)
                {
                     
                }
                
            } 
        } 
    }
}
