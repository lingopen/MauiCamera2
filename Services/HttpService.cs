using MauiCamera2.GMUtil;
using System.Net.Http.Headers;
using System.Text;

namespace MauiCamera2.Services
{
    public class HttpService : IHttpService
    {
        /// <summary>
        /// http客户端
        /// </summary>
        HttpClient _client;
        /// <summary>
        /// token 到期后 自动刷新 token
        /// </summary>
        public string Token { get; set; } = "";

        public HttpService()
        {
            HttpClientHandler handler = new HttpClientHandler() { UseDefaultCredentials = false };
            _client = new HttpClient(handler);
            _client.Timeout = TimeSpan.FromSeconds(30);
        }
        /// <summary>
        /// 如果请求返回401，则重新登录刷新token
        /// </summary>
        /// <returns></returns>
        async Task<bool> RefreshToken()
        {
            var parms = new LoginDto
            {
                Account = Constants.SysConfig?.Account,
                Password = CryptogramUtil.SM2Encrypt(Constants.SysConfig?.Password ?? "123456")//加密
            };
            var sevRes = await PostAsync<LoginOutput>("/api/app/appLogin", parms.ToJson(), 5);
            if (sevRes == null)
            {
                return false;
            }
            if (sevRes.code != 200)
            {
                return false;
            }
            Token = sevRes.result?.AccessToken??"";
            return true;
        }

        /// <summary>
        /// GET 请求
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="url">uri</param>
        /// <param name="timeOut">超时</param>
        /// <returns></returns>
        public async Task<RestDto<T>> GetAsync<T>(string url, int? timeOut)
        {
            Uri uri = new Uri(string.Format(Constants.ServerUrl + url, string.Empty));
            try
            {
            refreshToken:
                //if (timeOut.HasValue)
                //{
                //    HttpClientHandler handler = new HttpClientHandler() { UseDefaultCredentials = false };
                //    _client = new HttpClient(handler);
                //    _client.Timeout = TimeSpan.FromSeconds(timeOut.Value);
                //}
                if (Token.IsNotNull())
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var res = content.FromJson<RestDto<T>>();
                    if (res != null && res.code == 401)
                    {
                        if (await RefreshToken())
                            goto refreshToken;
                    }
                    return res;
                }
                else return new RestDto<T>() { code = -1, message = response.ReasonPhrase };
            }
            catch (Exception ex)
            {
                return new RestDto<T>() { code = -1, message = ex.Message.Contains("Timeout") ? "连接服务器超时,请重试!" : ex.Message };
            }
        }
        /// <summary>
        /// 上传多张照片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="files"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public async Task<RestDto<T>> PostFiles<T>(string url, List<Stream> files, int? timeOut)
        {
            Uri uri = new Uri(string.Format(Constants.ServerUrl + url, string.Empty));
            try
            {
            refreshToken:
                if (timeOut.HasValue)
                {
                    HttpClientHandler handler = new HttpClientHandler() { UseDefaultCredentials = false };
                    _client = new HttpClient(handler);
                    _client.Timeout = TimeSpan.FromSeconds(timeOut.Value);
                }
                if (Token.IsNotNull())
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

                using (var formData = new MultipartFormDataContent())
                {
                    foreach (var file in files) //携带文件列表
                    {
                        if (file == null) continue;
                        HttpContent fileStreamContent = new StreamContent(file);
                        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                        formData.Add(fileStreamContent, "files", DateTime.Now.ToString("yyyyMMddHHMmmssfff"));
                    }
                    var response = await _client.PostAsync(uri, formData);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var res = content.FromJson<RestDto<T>>();
                        if (res != null && res.code == 401)
                        {
                            if (await RefreshToken())
                                goto refreshToken;
                        }
                        return res;
                    }
                    else return new RestDto<T>() { code = -1, message = response.ReasonPhrase };
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Socket closed"))
                {
                    return new RestDto<T>() { code = -1, message = ex.Message.Contains("Timeout") ? "服务器任务处理超时,请重试!" : ex.Message };
                }
                else
                    return new RestDto<T>() { code = -1, message = ex.Message.Contains("Timeout") ? "连接服务器超时,请重试!" : ex.Message };
            }
        }

        /// <summary>
        /// POST 请求
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="url">uri</param>
        /// <param name="timeOut">超时</param>
        /// <returns></returns>
        public async Task<RestDto<T>> PostAsync<T>(string url, string json, int? timeOut)
        {
            Uri uri = new Uri(string.Format(Constants.ServerUrl + url, string.Empty));
            try
            {
            refreshToken:
                //if (timeOut.HasValue)
                //{
                //    HttpClientHandler handler = new HttpClientHandler() { UseDefaultCredentials = false };
                //    _client = new HttpClient(handler);
                //    _client.Timeout = TimeSpan.FromSeconds(timeOut.Value);
                //}
                if (Token.IsNotNull())
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

                HttpResponseMessage response;
                if (json.IsNotNull())
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await _client.PostAsync(uri, content);
                }
                else
                    response = await _client.PostAsync(uri, null);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var res = content.FromJson<RestDto<T>>();
                    if (res != null && res.code == 401)
                    {
                        if (await RefreshToken())
                            goto refreshToken;
                    }
                    return res;
                }
                else return new RestDto<T>() { code = -1, message = response.ReasonPhrase };
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Connection failure"))
                {
                    return new RestDto<T>() { code = -1, message = "WIFI网络尚未连接" };
                }
                else if (ex.Message.Contains("Socket closed"))
                {
                    return new RestDto<T>() { code = -1, message = ex.Message.Contains("Timeout") ? "服务器任务处理超时,请重试!" : ex.Message };
                }
                else
                    return new RestDto<T>() { code = -1, message = ex.Message.Contains("Timeout") ? "连接服务器超时,请重试!" : ex.Message };
            }
        }
    }
}
