using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiCamera2.Platforms.Droid.Handlers
{
    public class PdfWebViewHandler : WebViewHandler
    {
        protected override void ConnectHandler(Android.Webkit.WebView platformView)
        {
            base.ConnectHandler(platformView);
            platformView.Settings.AllowFileAccess = true;
            platformView.Settings.AllowFileAccessFromFileURLs = true;
            platformView.Settings.AllowUniversalAccessFromFileURLs = true;
        }

    }
}
