using SqlSugar;

namespace MauiCamera2.Sqlite.Entitys
{
    /// <summary>
    /// 参数配置表
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    [SugarTable(null, "参数配置表")]
    public class SysConfig : EntityBase
    {
        /// <summary>
        /// 登录用户ID
        /// </summary>
        [SugarColumn(ColumnDescription = "登录用户ID", IsNullable = true)]
        public long UserId { get; set; }
        [SugarColumn(ColumnDescription = "头像", Length = 1024, IsNullable = true)]
        public string? Avatar { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [SugarColumn(ColumnDescription = "账号", Length = 32, IsNullable = true)]
        public string? Account { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [SugarColumn(ColumnDescription = "姓名", Length = 32, IsNullable = true)]
        public string? RealName { get; set; }
        /// <summary>
        /// 未加密过的密码
        /// </summary>
        [SugarColumn(ColumnDescription = "未加密过的密码", Length = 32, IsNullable = true)]
        public string? Password { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [SugarColumn(ColumnDescription = "电话", IsNullable = true)]
        public string? Phone { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [SugarColumn(ColumnDescription = "邮箱", Length = 128, IsNullable = true)]
        public string? Email { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        [SugarColumn(ColumnDescription = "机构名称", Length = 128, IsNullable = true)]
        public string? OrgName { get; set; }
        /// <summary>
        /// 是否 记住密码
        /// </summary>
        [SugarColumn(ColumnDescription = "记住密码")]
        public bool IsRemenber { get; set; }
        /// <summary>
        /// 服务器api地址
        /// </summary>
        [SugarColumn(ColumnDescription = "服务器api地址", Length = 128, IsNullable = true)]
        public string? ServerUrl { get; set; }
        /// <summary>
        /// 上次登录时间
        /// </summary>
        [SugarColumn(ColumnDescription = "上次登录时间", IsNullable = true)]
        public DateTime? LastLoginTime { get; set; }
        /// <summary>
        /// 选中的语言序号
        /// </summary>
        [SugarColumn(ColumnDescription = "选中的语言序号", IsNullable = false)]
        public int LangIndex { get; set; } = 0;
       
    }
}
