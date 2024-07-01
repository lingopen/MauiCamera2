using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiCamera2.Services
{
    /// <summary>
    /// 用户登录结果
    /// </summary>
    public class LoginOutput
    {
        /// <summary>
        /// 令牌Token
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// 刷新Token
        /// </summary>
        public string? RefreshToken { get; set; }
    }
}
