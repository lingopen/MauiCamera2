namespace MauiCamera2.Services
{
    /// <summary>
    /// http服务
    /// </summary>
    public interface IHttpService
    {
        /// <summary>
        /// token
        /// </summary>
        string Token { get; set; }
        /// <summary>
        /// GET 请求
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="url">uri</param>
        /// <param name="timeOut">超时</param>
        /// <returns></returns>
        Task<RestDto<T>> GetAsync<T>(string url, int? timeOut = null);
        /// <summary>
        /// POST 请求
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="url">uri</param>
        /// <param name="timeOut">超时</param>
        /// <returns></returns>
        Task<RestDto<T>> PostAsync<T>(string url, string json, int? timeOut = null);
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="files"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        Task<RestDto<T>> PostFiles<T>(string url, List<Stream> files, int? timeOut);
    }
}
