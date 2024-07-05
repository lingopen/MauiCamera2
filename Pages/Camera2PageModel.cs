
using CommunityToolkit.Mvvm.Input;
using MauiCamera2.Controls;
using MauiCamera2.Services;

namespace MauiCamera2.Pages
{
    public partial class Camera2PageModel : BaseViewModel
    {
        private readonly ICamera2Service _camera2Service; 
        public Camera2PageModel(ICamera2Service camera2Service ) : base()
        {
            _camera2Service = camera2Service;
            _camera2Service.CallBack += _camera2Service_CallBack; 
        }

        private void _camera2Service_CallBack(object? sender, byte[]? dFile)
        {
            string path = System.IO.Path.Combine(Constants.Plateform.GetRootPath(), "Image", $"{Yitter.IdGenerator.YitIdHelper.NextId()}.jpeg");
            if (dFile != null)
            {
                Constants.Plateform.SaveFile(dFile, path);
            }
        }

        public void TextureView_HandlerChanged(object? obj, EventArgs e)
        {
            if (obj is TextureView view && view != null && view.Handler?.PlatformView != null)
            {
                _camera2Service.CreateCamera(view.Handler?.PlatformView, null);
            }
        }
        public override Task OnDisappearing()
        {
            _camera2Service.CloseCamera();
            return base.OnDisappearing();
        }
        /// <summary>
        /// 切换相机
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        void ChangeCamera()
        {
            _camera2Service.ChangeCamera();
        }
        /// <summary>
        /// 拍照
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        void TakePicture()
        {
            _camera2Service.TakePicture();
        }
   
    }
}
