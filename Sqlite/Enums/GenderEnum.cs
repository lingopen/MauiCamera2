﻿using System.ComponentModel;

namespace MauiCamera2.Sqlite
{
    /// <summary>
    /// 性别枚举
    /// </summary>
    [Description("性别枚举")]
    public enum GenderEnum
    {
        /// <summary>
        /// 男
        /// </summary>
        [Description("男")]
        Male = 1,

        /// <summary>
        /// 女
        /// </summary>
        [Description("女")]
        Female = 2,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 3
    }
}
