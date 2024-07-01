using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiCamera2.Services
{
    public interface IPlatformService
    {
        /// <summary>
        /// 确认权限
        /// </summary>
        /// <returns></returns>
        Task CheckPermissions();
        /// <summary>
        /// 获取app使用的SD卡路径
        /// </summary>
        /// <returns></returns>
        string GetRootPath();
        /// <summary>
        /// 打开系统时间设置
        /// </summary>
        void OpenTimeSetting();
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="dFile"></param>
        /// <param name="filePath"></param>
        void SaveFile(byte[] dFile, string filePath);
        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        byte[]? GetFile(string filePath);
    }
}
