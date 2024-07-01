using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiCamera2.Services
{
    /// <summary>
    /// 登录结构
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class LoginDto
    {
        /// <summary>
        /// 登陆账号
        /// </summary>
        public string? Account { get; set; }
        /// <summary>
        /// 登陆加密的密码
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// 是否 记住密码
        /// </summary>
        public bool IsRemenber { get; set; }

    }
}
