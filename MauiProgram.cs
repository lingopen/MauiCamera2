using CommunityToolkit.Maui;
using MauiCamera2.Controls;
using MauiCamera2.Pages;
using MauiCamera2.Platforms.Droid.Handlers;
using MauiCamera2.Platforms.Droid.Services;
using MauiCamera2.Services;
using Microsoft.Extensions.Logging;
using Plugin.BLE.Abstractions.Contracts;
using SqlSugar;

namespace MauiCamera2
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            SnowFlakeSingle.WorkId = 14;
            Yitter.IdGenerator.YitIdHelper.SetIdGenerator(new Yitter.IdGenerator.IdGeneratorOptions()
            {
                WorkerId = 1,
                WorkerIdBitLength = 1,
                SeqBitLength = 6
            });

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                 .ConfigureMauiHandlers(handlers =>
                 {
#if ANDROID
                     handlers.AddHandler(typeof(PdfWebView), typeof(PdfWebViewHandler)); 
                     handlers.AddHandler(typeof(TextureView), typeof(TextureViewHandler));
#endif
                 })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("iconfont.ttf", "IconFont");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<IHttpService, HttpService>();
            builder.Services.AddSingleton<IDbService, SqliteDbService>();//采用sqlite
            builder.Services.AddSingleton<ILogService, LogService>();
#if ANDROID
            builder.Services.AddSingleton<IPlatformService, PlatformService>();
            builder.Services.AddSingleton<ISoundService, SoundService>();
            builder.Services.AddTransient<ICamera2Service, Camera2Service>();
#endif
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainPageModel>();
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));


            builder.Services.AddTransient<Camera2Page>();
            builder.Services.AddTransient<Camera2PageModel>();
            Routing.RegisterRoute(nameof(Camera2Page), typeof(Camera2Page));

            builder.Services.AddTransient<Camera2FaceDetectorPage>();
            builder.Services.AddTransient<Camera2FaceDetectorPageModel>();
            Routing.RegisterRoute(nameof(Camera2FaceDetectorPage), typeof(Camera2FaceDetectorPage));

            builder.Services.AddTransient<BLEPage>();
            builder.Services.AddTransient<BLEPageModel>();
            Routing.RegisterRoute(nameof(BLEPage), typeof(BLEPage));
            return builder.Build();
        }
    }
}
