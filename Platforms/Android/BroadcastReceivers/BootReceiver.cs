using Android.App;
using Android.Content;

namespace MauiCamera2.Platforms.Droid.BroadcastReceivers
{
    /// <summary>
    /// 开启自启
    /// </summary>
    [BroadcastReceiver(Label = "BootReceiver", DirectBootAware = true, Enabled = true, Exported = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted }, Priority = (int)IntentFilterPriority.HighPriority)]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            var launch_intent = Platform.CurrentActivity?.PackageManager?.GetLaunchIntentForPackage(Platform.CurrentActivity?.PackageName ?? "MauiCamera2");
            if (launch_intent != null)
            {
                launch_intent.AddFlags(ActivityFlags.ReorderToFront);
                launch_intent.AddFlags(ActivityFlags.NewTask);
                launch_intent.AddFlags(ActivityFlags.ResetTaskIfNeeded);
                Platform.CurrentActivity?.StartActivity(launch_intent);
            }

        }
    }
}
