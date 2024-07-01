using MauiCamera2.Sqlite.Entitys;
using SqlSugar;

namespace MauiCamera2.Sqlite
{
    /// <summary>
    /// 框架实体基类Id
    /// </summary>
    public abstract class EntityBaseId
    {
        /// <summary>
        /// 雪花Id
        /// </summary>
        [SugarColumn(ColumnName = "Id", ColumnDescription = "主键Id", IsPrimaryKey = true, IsIdentity = false)]
        public virtual long Id { get; set; }
        /// <summary>
        /// 数据版本 客户端更新时需要+1，默认新增=0
        /// </summary>
        [SugarColumn(ColumnDescription = "数据版本", DefaultValue = "0")]
        public virtual long Version { get; set; } = 0;
    }

    /// <summary>
    /// 框架实体基类
    /// </summary>
    [SugarIndex("index_{table}_CT", nameof(CreateTime), OrderByType.Asc)]
    public abstract class EntityBase : EntityBaseId, IDeletedFilter
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDescription = "创建时间", IsOnlyIgnoreUpdate = true)]
        public virtual DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnDescription = "更新时间", IsNullable = false)]
        public virtual DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 创建者Id
        /// </summary>
        [SugarColumn(ColumnDescription = "创建者Id", IsOnlyIgnoreUpdate = true)]
        public virtual long? CreateUserId { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Navigate(NavigateType.OneToOne, nameof(CreateUserId))]
        public virtual SysUser? CreateUser { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        [SugarColumn(ColumnDescription = "创建者姓名", Length = 64, IsOnlyIgnoreUpdate = true)]
        public virtual string? CreateUserName { get; set; }

        /// <summary>
        /// 修改者Id
        /// </summary>
        [SugarColumn(ColumnDescription = "修改者Id")]
        public virtual long? UpdateUserId { get; set; }

        /// <summary>
        /// 修改者
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Navigate(NavigateType.OneToOne, nameof(UpdateUserId))]
        public virtual SysUser? UpdateUser { get; set; }

        /// <summary>
        /// 修改者姓名
        /// </summary>
        [SugarColumn(ColumnDescription = "修改者姓名", Length = 64)]
        public virtual string? UpdateUserName { get; set; }

        /// <summary>
        /// 软删除
        /// </summary>
        [SugarColumn(ColumnDescription = "软删除")]
        public virtual bool IsDelete { get; set; } = false;
    }

    /// <summary>
    /// 业务数据实体基类（数据权限）
    /// </summary>
    public abstract class EntityBaseData : EntityBase, IOrgIdFilter
    {
        /// <summary>
        /// 创建者部门Id
        /// </summary>
        [SugarColumn(ColumnDescription = "创建者部门Id", IsOnlyIgnoreUpdate = true)]
        public virtual long? CreateOrgId { get; set; }

        /// <summary>
        /// 创建者部门
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Navigate(NavigateType.OneToOne, nameof(CreateOrgId))]
        public virtual SysOrg? CreateOrg { get; set; }

        /// <summary>
        /// 创建者部门名称
        /// </summary>
        [SugarColumn(ColumnDescription = "创建者部门名称", Length = 64, IsOnlyIgnoreUpdate = true)]
        public virtual string? CreateOrgName { get; set; }
    }

    /// <summary>
    /// 租户实体基类
    /// </summary>
    public abstract class EntityTenant : EntityBase, ITenantIdFilter
    {
        /// <summary>
        /// 租户Id
        /// </summary>
        [SugarColumn(ColumnDescription = "租户Id", IsOnlyIgnoreUpdate = true)]
        public virtual long? TenantId { get; set; }
    }

    /// <summary>
    /// 租户实体基类Id
    /// </summary>
    public abstract class EntityTenantId : EntityBaseId, ITenantIdFilter
    {
        /// <summary>
        /// 租户Id
        /// </summary>
        [SugarColumn(ColumnDescription = "租户Id", IsOnlyIgnoreUpdate = true)]
        public virtual long? TenantId { get; set; }
    }

    /// <summary>
    /// 租户实体基类 + 业务数据（数据权限）
    /// </summary>
    public abstract class EntityTenantBaseData : EntityBaseData, ITenantIdFilter
    {
        /// <summary>
        /// 租户Id
        /// </summary>
        [SugarColumn(ColumnDescription = "租户Id", IsOnlyIgnoreUpdate = true)]
        public virtual long? TenantId { get; set; }
    }
}
