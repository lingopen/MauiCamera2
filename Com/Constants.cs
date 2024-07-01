using MauiCamera2.Services;
using MauiCamera2.Sqlite.Entitys;
using System.Net.NetworkInformation;
namespace MauiCamera2
{
    /// <summary>
    /// 公共内存
    /// </summary>
    public static class Constants
    {
        public static string ServerUrl { get; set; }

        /// <summary>
        /// 系统配置含登录信息
        /// </summary>
        public static SysConfig SysConfig { get; set; }
        /// <summary>
        /// 服务器请求服务
        /// </summary>
        public static IHttpService Api { get; set; }
        /// <summary>
        /// 本地数据库服务
        /// </summary>
        public static IDbService LocalDb { get; set; }
        /// <summary>
        /// 调用平台操作服务
        /// </summary>
        public static IPlatformService Plateform { get; set; }

        /// <summary>
        /// 是否联网
        /// </summary>
        public static bool HasNetWork
        {
            get
            {
                return NetworkInterface.GetIsNetworkAvailable();
            }
        }
    }
}
