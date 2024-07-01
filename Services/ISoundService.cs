namespace MauiCamera2.Services
{
    /// <summary>
    /// 声音模块
    /// </summary>
    public interface ISoundService
    {
        /// <summary>
        /// 播放成功声音
        /// </summary>
        void PlaySuccess();
        /// <summary>
        /// 播放失败声音
        /// </summary>
        void PlayFail();
    }
}
