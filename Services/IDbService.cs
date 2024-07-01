using MauiCamera2.Sqlite;
using SqlSugar;
using System.Linq.Expressions;

namespace MauiCamera2.Services
{
    /// <summary>
    /// 数据库服务
    /// </summary>
    public interface IDbService
    {
        /// <summary>
        /// SqlSugarScope对象
        /// </summary>
        SqlSugarScope Db { get; set; }
        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns></returns>
        Task Connect();

        /// <summary>
        /// 获取数据版本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<long> GetVersion<T>() where T : EntityBase, new();
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predExpr"></param>
        /// <returns></returns>
        Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> predExpr) where T : EntityBase, new();
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predExpr"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<List<T>> GetPageListAsync<T>(Expression<Func<T, bool>> predExpr, int pageNumber, int pageSize) where T : EntityBase, new();
        /// <summary>
        /// 详情查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predExpr"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(Expression<Func<T, bool>> predExpr) where T : EntityBase, new();
        /// <summary>
        /// 更新或新增
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<int> SaveAsync<T>(T item) where T : EntityBase, new();

        /// <summary>
        /// 快速新增或修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<int> FastSaveAsync<T>(List<T> list) where T : EntityBase, new();
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predExpr"></param>
        /// <returns></returns>
        Task<int> DeleteItemAsync<T>(Expression<Func<T, bool>> predExpr) where T : EntityBase, new();
    }
}
