using System.Net;
using System.Text.RegularExpressions;

namespace MauiCamera2
{
    /// <summary>
    /// 验证帮助类
    /// </summary>
    public static class ValidateHelper
    {
        /// <summary>
        /// 采用webclient下载图片
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static ImageSource FromUri(this string uri)
        {
#pragma warning disable SYSLIB0014 // 类型或成员已过时
            return ImageSource.FromStream(() => new MemoryStream(new WebClient().DownloadData(uri)));
#pragma warning restore SYSLIB0014 // 类型或成员已过时
        }

        /// <summary>
        /// 采用webclient下载
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] DownLoadImage(this string url)
        {

            try
            {
                if (url.Substring(0, 5) == "https")
                {
                    // 解决WebClient不能通过https下载内容问题
                    System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                        delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                 System.Security.Cryptography.X509Certificates.X509Chain chain,
                                 System.Net.Security.SslPolicyErrors sslPolicyErrors)
                        {
                            return true; // **** Always accept
                        };
                }
#pragma warning disable SYSLIB0014 // 类型或成员已过时
                var hl = new WebClient();
#pragma warning restore SYSLIB0014 // 类型或成员已过时
                var hltext = hl.DownloadData(url); //取网页源码
                return hltext;
            }
            catch (Exception)
            {
                return null;
            }

        }
        /// <summary>
        /// 手机号验证
        /// </summary>
        /// <param name="str_telephone"></param>
        /// <returns></returns>
        public static bool IsMatchTelephone(this string str_telephone)
        {

            //电信手机号规则
            string dianxin = @"^1[3578][01379]\d{8}$";
            Regex dReg = new Regex(dianxin);
            //连通手机规则
            string liantong = @"^1[34578][01256]\d{8}$";
            Regex tReg = new Regex(liantong);
            //移动手机规则
            string yidong = @"^(134[012345678]\d{7}|1[34578][0123456789]\d{8})$";
            Regex yReg = new Regex(yidong);
            if (dReg.IsMatch(str_telephone) || tReg.IsMatch(str_telephone) || yReg.IsMatch(str_telephone)) //不匹配
            {
                //xiaoxi("手机号格式不对，请重新输入！");    //弹框提示
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// 是否不为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static bool IsNotNull(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }
        [System.Diagnostics.DebuggerStepThrough]
        public static bool ListIsNull<T>(this List<T> list)
        {
            return list == null || list.Count < 1;
        }
        [System.Diagnostics.DebuggerStepThrough]
        public static bool IsNull(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
        [System.Diagnostics.DebuggerStepThrough]
        public static bool IsNotNull<T>(this IList<T> list)
        {
            return list != null && list.Count > 0;
        }

        /// <summary>
        /// 去掉头部的数字
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string DelHeaderNumber(this string str)
        {
            return Regex.Replace(str, @"^\d*", "");
        }
    }
}
