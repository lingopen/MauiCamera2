using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace MauiCamera2.Sqlite.Entitys
{
    /// <summary>
    /// 系统用户表
    /// </summary>
    [SugarTable(null, "系统用户表")]
    [SugarIndex("index_{table}_A", nameof(Account), OrderByType.Asc)]
    [SugarIndex("index_{table}_P", nameof(Phone), OrderByType.Asc)]
    public class SysUser : EntityBase
    {
        /// <summary>
        /// 账号
        /// </summary>
        [SugarColumn(ColumnDescription = "账号", Length = 32)]
        [Required, MaxLength(32)]
        public virtual string? Account { get; set; }



        /// <summary>
        /// 真实姓名
        /// </summary>
        [SugarColumn(ColumnDescription = "真实姓名", Length = 32)]
        [MaxLength(32)]
        public virtual string? RealName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [SugarColumn(ColumnDescription = "头像", Length = 512, IsNullable = true)]
        [MaxLength(512)]
        public string? Avatar { get; set; }

        /// <summary>
        /// 性别-男_1、女_2
        /// </summary>
        [SugarColumn(ColumnDescription = "性别")]
        public GenderEnum Sex { get; set; } = GenderEnum.Male;

        /// <summary>
        /// 年龄
        /// </summary>
        [SugarColumn(ColumnDescription = "年龄")]
        public int Age { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [SugarColumn(ColumnDescription = "出生日期", IsNullable = true)]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        [SugarColumn(ColumnDescription = "民族", Length = 32, IsNullable = true)]
        [MaxLength(32)]
        public string? Nation { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [SugarColumn(ColumnDescription = "手机号码", Length = 32, IsNullable = true)]
        [MaxLength(32)]
        public string? Phone { get; set; }



        /// <summary>
        /// 邮箱
        /// </summary>
        [SugarColumn(ColumnDescription = "邮箱", Length = 64, IsNullable = true)]
        [MaxLength(64)]
        public string? Email { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [SugarColumn(ColumnDescription = "地址", Length = 256, IsNullable = true)]
        [MaxLength(256)]
        public string? Address { get; set; }


        /// <summary>
        /// 直属机构Id
        /// </summary>
        [SugarColumn(ColumnDescription = "直属机构Id")]
        public long OrgId { get; set; }

        /// <summary>
        /// 直属机构
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(OrgId))]
        public SysOrg? SysOrg { get; set; }

        /// <summary>
        /// 直属主管Id
        /// </summary>
        [SugarColumn(ColumnDescription = "直属主管Id", IsNullable = true)]
        public long? ManagerUserId { get; set; }


        /// <summary>
        /// 职位Id
        /// </summary>
        [SugarColumn(ColumnDescription = "职位Id")]
        public long PosId { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Navigate(NavigateType.OneToOne, nameof(PosId))]
        public SysPos? SysPos { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [SugarColumn(ColumnDescription = "工号", Length = 32, IsNullable = true)]
        [MaxLength(32)]
        public string? JobNum { get; set; }

        /// <summary>
        /// 入职日期
        /// </summary>
        [SugarColumn(ColumnDescription = "入职日期", IsNullable = true)]
        public DateTime? JoinDate { get; set; }


        /// <summary>
        /// 电子签名
        /// </summary>
        [SugarColumn(ColumnDescription = "电子签名", Length = 512, IsNullable = true)]
        [MaxLength(512)]
        public string? Signature { get; set; }
    }
}
