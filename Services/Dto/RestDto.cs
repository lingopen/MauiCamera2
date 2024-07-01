namespace MauiCamera2.Services
{
    /// <summary>
    /// RESTful风格---返回格式
    /// </summary>
    public class RestDto
    {
        /// <summary>
        /// 执行成功
        /// </summary>
        public string? type { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        public int? code { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string? message { get; set; }
        /// <summary>
        /// 蓝牙地址
        /// </summary>
        public string? mac { get; set; }
        /// <summary>
        /// 附加数据
        /// </summary>
        public string? extras { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime time { get; set; }
    }
    /// <summary>
    /// RESTful风格---返回格式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RestDto<T> : RestDto
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T? result { get; set; }
        public string? typeName { get; set; }
    }
}
