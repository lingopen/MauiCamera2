namespace MauiCamera2.Services
{
    /// <summary>
    /// Camera2相机服务
    /// </summary>
    public interface ICamera2Service
    {
        /// <summary>
        /// 初始化相机
        /// </summary>
        /// <param name="view"></param>
        void InitCamera(object? view);
        /// <summary>
        /// 切换前置或后置相机
        /// </summary>
        void ChangeCamera();
        
        /// <summary>
        /// 拍照
        /// </summary>
        void TakePicture();
        /// <summary>
        /// 释放相机
        /// </summary>
        void ReleaseCamera();
        /// <summary>
        /// 一帧照片数据
        /// </summary>
        event EventHandler<byte[]?> CallBack;
    }
}
