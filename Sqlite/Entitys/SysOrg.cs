﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiCamera2.Sqlite.Entitys
{
    /// <summary>
    /// 系统机构表
    /// </summary>
    [SugarTable(null, "系统机构表")]
    public class SysOrg : EntityBase
    {
        /// <summary>
        /// 父Id
        /// </summary>
        [SugarColumn(ColumnDescription = "父Id")]
        public long Pid { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnDescription = "名称", Length = 64)]
        [Required, MaxLength(64)]
        public virtual string? Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [SugarColumn(ColumnDescription = "编码", Length = 64, IsNullable = true)]
        [MaxLength(64)]
        public string? Code { get; set; }



        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(ColumnDescription = "排序")]
        public int OrderNo { get; set; } = 100;

        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(ColumnDescription = "状态")]
        public StatusEnum Status { get; set; } = StatusEnum.Enable;

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnDescription = "备注", Length = 128, IsNullable = true)]
        [MaxLength(128)]
        public string? Remark { get; set; }

        /// <summary>
        /// 机构子项
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<SysOrg>? Children { get; set; }

    }
}
