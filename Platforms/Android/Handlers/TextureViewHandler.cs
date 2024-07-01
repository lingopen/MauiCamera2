using MauiCamera2.Controls;
using MauiCamera2.Platforms.Droid.Controls;
using Microsoft.Maui.Handlers;

namespace MauiCamera2.Platforms.Droid.Handlers
{
    public class TextureViewHandler : ViewHandler<TextureView, TextureViewEx>
    {
        public TextureViewHandler() : base(PropertyMapper)
        {
        }
        public static PropertyMapper<TextureView, TextureViewHandler> PropertyMapper = new PropertyMapper<TextureView, TextureViewHandler>(ViewHandler.ViewMapper)
        {
             
        };

        protected override TextureViewEx CreatePlatformView()
        {
            return new TextureViewEx(Context);
        }
    }
}
