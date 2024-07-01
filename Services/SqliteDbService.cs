using Dm.filter;
using MauiCamera2.Sqlite.Entitys;
using MauiCamera2.Sqlite;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static AndroidX.Navigation.NavType;
using MauiCamera2.GMUtil;
using Yitter.IdGenerator;

namespace MauiCamera2.Services
{
    public class SqliteDbService : IDbService
    {
        /// <summary>
        /// 本地库
        /// </summary>
        public SqlSugarScope Db { get; set; }

        IPlatformService _platformService;

        public SqliteDbService(IPlatformService platformService)
        {
            _platformService = platformService;
        }

        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns></returns>
        public async Task Connect()
        {
            string path = Path.Combine(_platformService.GetRootPath(), "Data");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string dbName = Path.Combine(path, "app.sqlite");
            var dbConnection = $"DataSource={dbName}";
            var configConnection = new ConnectionConfig()
            {
                DbType = DbType.Sqlite,
                ConnectionString = dbConnection,
                IsAutoCloseConnection = true,
                MoreSettings = new ConnMoreSettings
                {
                    IsAutoDeleteQueryFilter = false,//启用删除查询过滤器  
                    IsAutoUpdateQueryFilter = false//启用更新查询过滤器 
                }
            };
            Db = new SqlSugarScope(configConnection,
                db =>
                {
                    // 配置实体假删除过滤器
                    db.QueryFilter.AddTableFilter<IDeletedFilter>(u => u.IsDelete == false);
                    //// 配置租户过滤器 
                    //if (Constants.LoginInfo != null && Constants.LoginInfo.TenantId.HasValue)
                    //    db.QueryFilter.AddTableFilter<ITenantIdFilter>(u => u.TenantId == Constants.LoginInfo.TenantId);
                    //单例参数配置，所有上下文生效
                    db.Aop.OnLogExecuting = (sql, pars) =>
                    {
                        string theSql = sql;
                        foreach (var par in pars)
                        {
                            theSql = theSql.Replace(par.ParameterName, par.Value == null ? "NULL" : par.Value.ToString());
                        }
                        System.Diagnostics.Trace.WriteLine(theSql);//输出sql
                    };
                    db.Aop.OnError = (ex) =>
                    {
                        System.Diagnostics.Trace.WriteLine(ex);
                    };

                });
            Db.CodeFirst.SetStringDefaultLength(128);
            //建库：如果不存在创建数据库存在不会重复创建
            Db.DbMaintenance.CreateDatabase();
            Db.CodeFirst.InitTables(typeof(SysConfig));
            Db.CodeFirst.InitTables(typeof(SysOrg));
            Db.CodeFirst.InitTables(typeof(SysPos));
            Db.CodeFirst.InitTables(typeof(SysUser));
            await Task.CompletedTask;
        }


        /// <summary>
        /// 获取数据版本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<long> GetVersion<T>() where T : EntityBase, new()
        {
            try
            {
                var query = Db.Queryable<T>();
                var lastVersion = await query.MaxAsync(p => p.Version);//查询版本  
                return lastVersion;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> predExpr) where T : EntityBase, new()
        {
            return await Db.Queryable<T>().Where(predExpr).ToListAsync();
        }
        public async Task<List<T>> GetPageListAsync<T>(Expression<Func<T, bool>> predExpr, int pageNumber, int pageSize) where T : EntityBase, new()
        {
            return await Db.Queryable<T>().Where(predExpr).ToPageListAsync(pageNumber, pageSize);
        }
        public async Task<T> GetAsync<T>(Expression<Func<T, bool>> predExpr) where T : EntityBase, new()
        {
            return await Db.Queryable<T>().Where(predExpr).FirstAsync();
        }
        public async Task<int> SaveAsync<T>(T item) where T : EntityBase, new()
        {
            var exist = await Db.Queryable<T>()
                .ClearFilter<ITenantIdFilter>()
                .Where(p => p.Id == item.Id).FirstAsync();
            if (exist != null)
            {
                item.UpdateUserId = Constants.SysConfig?.UserId ?? 0;
                item.UpdateUserName = Constants.SysConfig?.RealName ?? "超级管理员";
                item.UpdateTime = DateTime.Now;
                return await Db.Updateable<T>(item).ExecuteCommandAsync();
            }
            else
            {
                item.Id = item.Id == 0 ? YitIdHelper.NextId() : item.Id;
                item.CreateUserId = Constants.SysConfig?.UserId ?? 0;
                item.CreateUserName = Constants.SysConfig?.RealName ?? "";
                item.CreateTime = DateTime.Now;
                item.UpdateUserId = Constants.SysConfig?.UserId ?? 0;
                item.UpdateUserName = Constants.SysConfig?.RealName ?? "超级管理员";
                item.UpdateTime = DateTime.Now;
                item.IsDelete = false;
                item.Version = YitIdHelper.NextId();//新增时默认为1
                return await Db.Insertable<T>(item).ExecuteCommandAsync();
            }
        }
        /// <summary>
        /// 快速新增或修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<int> FastSaveAsync<T>(List<T> list) where T : EntityBase, new()
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                item.Id = item.Id == 0 ? YitIdHelper.NextId() : item.Id;
                item.CreateUserId = !item.CreateUserId.HasValue ? Constants.SysConfig?.UserId ?? 0 : item.CreateUserId.Value;
                item.CreateUserName = string.IsNullOrEmpty(item.CreateUserName) ? Constants.SysConfig?.RealName ?? "" : item.CreateUserName;
                item.CreateTime = !item.CreateTime.HasValue ? DateTime.Now : item.CreateTime.Value;
                item.UpdateUserId = !item.UpdateUserId.HasValue ? Constants.SysConfig?.UserId ?? 0 : item.UpdateUserId.Value;
                item.UpdateUserName = string.IsNullOrEmpty(item.UpdateUserName) ? Constants.SysConfig?.RealName ?? "" : item.UpdateUserName;
                item.UpdateTime = !item.UpdateTime.HasValue ? DateTime.Now : item.UpdateTime.Value;//从服务器下载时，这里的version应该等于服务器的值
                item.Version = item.Id == 0 ? YitIdHelper.NextId() : item.Version;
                item.IsDelete = item.IsDelete;
            }
            return await Db.Fastest<T>().PageSize(10000).BulkMergeAsync(list);
        }
        public async Task<int> DeleteItemAsync<T>(Expression<Func<T, bool>> predExpr) where T : EntityBase, new()
        {
            return await Db.Deleteable<T>(predExpr).ExecuteCommandAsync();
        }
    }
}
