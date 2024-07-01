using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiCamera2.Services
{
    /// <summary>
    /// 文本日志服务
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// 记录本地日志文件
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="state">状态</param>
        void WriteLog(string content, bool? state = false);
        /// <summary>
        /// 清理指定路径的日志文件夹
        /// </summary>
        /// <param name="days">30天以前</param>
        /// <param name="path">绝对路径</param>
        void CleanLog(int days = -30, string? path = null);
    }
}
