namespace MauiCamera2.Services
{
    /// <summary>
    /// Camera2人脸检测服务
    /// </summary>
    public interface ICamera2FaceDetectorService : ICamera2Service
    {
        /// <summary>
        /// 初始化人脸检测
        /// </summary>
        void InitFaceDetect(object? view);
    }
}
