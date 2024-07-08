using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MauiCamera2.Services;
using static Android.Views.View;
using Button = Android.Widget.Button;

namespace MauiCamera2.Platforms.Droid.Services
{
    public class FloatService : IFloatService
    {
        public WindowManagerLayoutParams WMLParams { get; set; }
        public IWindowManager? WM { get; set; }
        public Button? FloatButton { get; set; }
        public void Show()
        {
            FloatButton = new Button(Platform.AppContext);
            FloatButton.SetText("悬浮窗", TextView.BufferType.Normal);

            WM = Platform.CurrentActivity?.GetSystemService(Context.WindowService) as IWindowManager;
            WMLParams = new WindowManagerLayoutParams();

            // 设置window type 

            //检查版本，注意当type为TYPE_APPLICATION_OVERLAY时，铺满活动窗口，但在关键的系统窗口下面，如状态栏或IME
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                WMLParams.Type = WindowManagerTypes.ApplicationOverlay;
            }
            else
            {
                WMLParams.Type = WindowManagerTypes.SystemAlert;
            }
            /*
             * 如果设置为parms.type = WindowManager.Layoutparms.TYPE_PHONE; 那么优先级会降低一些,
             * 即拉下通知栏不可见
             */

            WMLParams.Format = Android.Graphics.Format.Rgba8888; // 设置图片格式，效果为背景透明

            // 设置Window flag
            WMLParams.Flags = WindowManagerFlags.NotTouchModal | WindowManagerFlags.NotFocusable;
            /*
             * 下面的flags属性的效果形同“锁定”。 悬浮窗不可触摸，不接受任何事件,同时不影响后面的事件响应。
             * wmparms.flags=Layoutparms.FLAG_NOT_TOUCH_MODAL |
             * Layoutparms.FLAG_NOT_FOCUSABLE | Layoutparms.FLAG_NOT_TOUCHABLE;
             */

            // 设置悬浮窗的长得宽
            WMLParams.Width = 150;
            WMLParams.Height = 150;

            // 设置悬浮窗的Touch监听
            FloatButton.SetOnTouchListener(new OnTouchListener(this));
            FloatButton.Background?.SetAlpha(100);
            WM?.AddView(FloatButton, WMLParams);
        }
    }

    class OnTouchListener : Java.Lang.Object, IOnTouchListener
    {
        FloatService parent;
        int lastX, lastY;
        int paramX, paramY;
        public OnTouchListener(FloatService parent)
        {
            this.parent = parent;
        }
        public bool OnTouch(Android.Views.View? v, MotionEvent? e)
        {
            if (e == null) return true;
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    lastX = (int)e.RawX;
                    lastY = (int)e.RawY;
                    paramX = parent.WMLParams.X;
                    paramY = parent.WMLParams.Y;
                    break;
                case MotionEventActions.Move:
                    int dx = (int)e.RawX - lastX;
                    int dy = (int)e.RawY - lastY;
                    parent.WMLParams.X = paramX + dx;
                    parent.WMLParams.Y = paramY + dy;
                    // 更新悬浮窗位置
                    parent?.WM?.UpdateViewLayout(parent.FloatButton, parent.WMLParams);
                    break;
            }
            return true;
        }
    }
}
