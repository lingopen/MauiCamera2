using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using MauiCamera2.Platforms.Droid.BroadcastReceivers;

namespace MauiCamera2
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            //设置状态栏、导航栏色颜色
            Window?.SetStatusBarColor(Android.Graphics.Color.Argb(255, 0, 0, 0));
            base.OnCreate(savedInstanceState);
            try
            {
                // 注册开机启动广播接收器
                var bootReceiverIntent = new IntentFilter(Intent.ActionBootCompleted);
                var bootReceiver = new BootReceiver();
                RegisterReceiver(bootReceiver, bootReceiverIntent);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "初始化失败," + ex.Message, ToastLength.Long)?.Show();
            }
        }
    }
}
