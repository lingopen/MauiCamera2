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
        /// <param name="view">预览视图</param>
        /// <param name="faceView">人脸视图</param>
        void CreateCamera(object? view,object? faceView);
        /// <summary>
        /// 切换前置或后置相机
        /// </summary>
        void ChangeCamera();
        
        /// <summary>
        /// 拍照
        /// </summary>
        void TakePicture();
        /// <summary>
        /// 关闭相机
        /// </summary>
        void CloseCamera();
        /// <summary>
        /// 一帧照片数据
        /// </summary>
        event EventHandler<byte[]?> CallBack;
    }
}
