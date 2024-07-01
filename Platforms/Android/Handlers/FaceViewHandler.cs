using MauiCamera2.Controls;
using MauiCamera2.Platforms.Droid.Controls;
using Microsoft.Maui.Handlers;

namespace MauiCamera2.Platforms.Droid.Handlers
{
    public class FaceViewHandler : ViewHandler<FaceView, FaceViewEx>
    {
        public FaceViewHandler() : base(PropertyMapper)
        {
        }
        public static PropertyMapper<FaceView, FaceViewHandler> PropertyMapper = new PropertyMapper<FaceView, FaceViewHandler>(ViewHandler.ViewMapper)
        {
            
        };

        protected override FaceViewEx CreatePlatformView()
        {
            return new FaceViewEx(Context);
        }
    }
}
