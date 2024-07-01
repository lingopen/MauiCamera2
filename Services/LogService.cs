using System.Text;

namespace MauiCamera2.Services
{
    public class LogService : ILogService
    {
        IPlatformService _platformService;
        public LogService(IPlatformService platformService)
        {
            _platformService = platformService;
        }
        private string writeLock = "";
        /// <summary>
        /// 记录本地日志文件
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="state">状态</param>
        public void WriteLog(string content, bool? state = false)
        {
            lock (writeLock)
            {
                var time = DateTime.Now;
                var dirPath = Path.Combine(_platformService.GetRootPath(), "Logs", time.ToString("yyyyMM"));
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                //1小时一个日志文件
                string filePath = Path.Combine(dirPath, time.ToString("yyyyMMddHH") + ".log");
                using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    sw.WriteLine($"{time.ToString("yyyy-MM-dd HH:mm:ss")}->{(state == null ? "【IN】" : state.Value ? "【OK】" : "【NG】")}->{content}");
                }
            }
        }

        /// <summary>
        /// 清理指定路径的文件夹
        /// </summary> 
        /// <param name="days">30天以前</param>
        /// <param name="path">绝对路径</param>
        public void CleanLog(int days = -30, string? path = null)
        {
            if (path == null)
                path = Path.Combine(_platformService.GetRootPath(), "Logs");
            if (!Directory.Exists(path)) return;
            DirectoryInfo dir = new DirectoryInfo(path);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.LastWriteTime < DateTime.Now.AddDays(days))
                {
                    file.Delete();
                }
            }
            var dirs = dir.GetDirectories();
            foreach (var waitDelte in dirs)
            {
                CleanLog(days, waitDelte.FullName);
            }
        }
    }
}
