using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using PagedList;
using Shared.Utilities.Models.Models;
using System.Linq.Expressions;

namespace Shared.Utilities.Models
{
    /// <summary>
    /// This interface implemented base database operation with generic repository pattern
    /// </summary>
    public interface IRepository<TEntity> where TEntity : class, new()
    {
        IQueryable<TEntity> Where(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);
        Task<IEnumerable<TEntity>> WhereAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);
        TEntity Get(
                    Expression<Func<TEntity, bool>> predicate,
                    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);
        Task<TEntity> GetAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);
        TEntity Get(
            string predicate);
        Task<TEntity> GetAsync(
            string predicate);
        TEntity Get(
            string predicate,
            List<string> includes);
        Task<TEntity> GetAsync(
            string predicate,
            List<string> includes);
        TEntity Get(
            string predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include);
        Task<TEntity> GetAsync(
            string predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include);
        IPagedList<TResult> GetPagedList<TResult>(
            IMapper mapper,
            List<string> include,
            Expression<Func<TEntity, bool>> predicate = null,
            SearchModel searchModel = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
            where TResult : class, new();

        Task<IPagedList<TResult>> GetPagedListAsync<TResult>(
            IMapper mapper,
            List<string> include,
            Expression<Func<TEntity, bool>> predicate = null,
            SearchModel searchModel = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
            where TResult : class, new();

        IPagedList<TResult> GetPagedList<TResult>(
            IMapper mapper,
            Expression<Func<TEntity, bool>> predicate = null,
            SearchModel searchModel = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
            where TResult : class, new();

        Task<IPagedList<TResult>> GetPagedListAsync<TResult>(
            IMapper mapper,
            Expression<Func<TEntity, bool>> predicate = null,
            SearchModel searchModel = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
            where TResult : class, new();

        void Add(TEntity entity);
        Task AddAsync(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TEntity record);
        bool Exists(Expression<Func<TEntity, bool>> selector = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> selector = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);
    }
}
