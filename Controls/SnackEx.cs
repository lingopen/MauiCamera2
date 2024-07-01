using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;

namespace MauiCamera2.Controls
{
    public class SnackEx
    {
        /// <summary>
        /// 显示告警消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="duration"></param>
        /// <param name="actionText"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task ShowAlarmAsync(string msg, int duration = 5, string actionText = "", Action action = null)
        {
            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Colors.Red,
                TextColor = Colors.White,
                ActionButtonTextColor = Color.FromArgb("FFE5B9"),
                CornerRadius = new CornerRadius(10),
                Font = Font.SystemFontOfSize(14),
                ActionButtonFont = Font.SystemFontOfSize(14),

                //CharacterSpacing = 0.5,
            };
            TimeSpan ts = TimeSpan.FromSeconds(duration);
            var snackbar = Snackbar.Make(msg, action, actionText, ts, snackbarOptions);
            await snackbar.Show();
        }
        /// <summary>
        /// 底部的计时警报
        /// </summary>
        public static void SnackShowErrorShort(string msg, Action action = null, string butText = "确定")
        {
            App.Current?.Dispatcher.Dispatch(async () =>
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                var snackbarOptions = new SnackbarOptions
                {
                    BackgroundColor = Colors.Red,
                    TextColor = Colors.Green,
                    ActionButtonTextColor = Colors.Yellow,
                    CornerRadius = new CornerRadius(10),
                    Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                    ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(14),
                    CharacterSpacing = 0.5
                };

                var snackbar = Snackbar.Make(msg, action, butText, TimeSpan.FromSeconds(3), snackbarOptions);
                await snackbar.Show(cancellationTokenSource.Token);

            });



        }
        /// <summary>
        /// 底部的计时警报
        /// </summary>
        public static void SnackShow(string msg, TimeSpan duration, Action action = null, string butText = "确定")
        {

            App.Current?.Dispatcher.Dispatch(async () =>
            {


                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                var snackbarOptions = new SnackbarOptions
                {
                    BackgroundColor = Colors.Red,
                    TextColor = Colors.Green,
                    ActionButtonTextColor = Colors.Yellow,
                    CornerRadius = new CornerRadius(10),
                    Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                    ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(14),
                    CharacterSpacing = 0.5
                };

                var snackbar = Snackbar.Make(msg, action, butText, duration, snackbarOptions);
                await snackbar.Show(cancellationTokenSource.Token);
            });



        }
        /// <summary>
        /// 底部的计时警报
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="butText"></param>
        /// <param name="action"></param>
        public static void SnackShowTips(string msg, TimeSpan duration, Action action = null, string butText = "确定")
        {
            App.Current?.Dispatcher.Dispatch(async () =>
            {

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                var snackbarOptions = new SnackbarOptions
                {
                    BackgroundColor = Colors.Blue,
                    TextColor = Colors.White,
                    ActionButtonTextColor = Colors.White,
                    CornerRadius = new CornerRadius(10),
                    Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                    ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(14),
                    CharacterSpacing = 0.5
                };

                var snackbar = Snackbar.Make(msg, action, butText, duration, snackbarOptions);
                await snackbar.Show(cancellationTokenSource.Token);
            });
        }
        /// <summary>
        /// 底部的计时警报
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="butText"></param>
        /// <param name="action"></param>
        public static void SnackShowSuceess(string msg, TimeSpan duration, Action action = null, string butText = "确定")
        {
            App.Current?.Dispatcher.Dispatch(async () =>
            {

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                var snackbarOptions = new SnackbarOptions
                {
                    BackgroundColor = Colors.Green,
                    TextColor = Colors.White,
                    ActionButtonTextColor = Colors.White,
                    CornerRadius = new CornerRadius(10),
                    Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                    ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(14),
                    CharacterSpacing = 0.5
                };

                var snackbar = Snackbar.Make(msg, action, butText, duration, snackbarOptions);
                await snackbar.Show(cancellationTokenSource.Token);
            });
        }
    }
}
