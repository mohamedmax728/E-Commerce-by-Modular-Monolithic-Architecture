using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PagedList;
using Shared.Infrastructre.Context;
using Shared.Utilities.Models;
using Shared.Utilities.Models.Models;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace EasyRepository.EFCore.Generic
{
    /// <summary>
    /// This class contains implementations of repository functions
    /// </summary>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        private readonly AppDbContext _dbContext;
        public Repository(AppDbContext context)
        {
            _dbContext = context;
        }

        public virtual IQueryable<TEntity> Where(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            if (include != null)
            {
                return include(_dbContext.Set<TEntity>())
                .Where(predicate)!;
            }

            return _dbContext.Set<TEntity>().Where(predicate)!;
        }

        public virtual async Task<IEnumerable<TEntity>> WhereAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            if (include != null)
            {
                return await include(_dbContext.Set<TEntity>())
                .Where(predicate).ToListAsync()!;
            }

            return await _dbContext.Set<TEntity>().Where(predicate).ToListAsync()!;
        }

        public virtual TEntity Get(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            if (include != null)
            {
                return include(_dbContext.Set<TEntity>())
                .FirstOrDefault(predicate)!;
            }

            return _dbContext.Set<TEntity>().FirstOrDefault(predicate)!;
        }

        public virtual TEntity Get(
            string predicate) => _dbContext.Set<TEntity>().FirstOrDefault(predicate: predicate)!;

        public virtual TEntity Get(
            string predicate,
            List<string> includes)
        {
            if (includes != null)
            {
                IQueryable<TEntity> res = _dbContext.Set<TEntity>();
                includes.ForEach(e =>
                {
                    res = res.Include(e);
                });
                return res
                .FirstOrDefault(predicate)!;
            }

            return _dbContext.Set<TEntity>().FirstOrDefault(predicate)!;
        }

        public virtual TEntity Get(
            string predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include)
        {
            if (include != null)
            {
                return include(_dbContext.Set<TEntity>())
                .FirstOrDefault(predicate)!;
            }

            return _dbContext.Set<TEntity>().FirstOrDefault(predicate)!;
        }

        public virtual async Task<TEntity> GetAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            if (include != null)
            {
                return await include(_dbContext.Set<TEntity>())
                .FirstOrDefaultAsync(predicate)!;
            }

            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate)!;
        }

        public virtual async Task<TEntity> GetAsync(
            string predicate) => await _dbContext.Set<TEntity>().FirstOrDefault(predicate)!;

        public virtual async Task<TEntity> GetAsync(
            string predicate,
            List<string> includes)
        {
            if (includes != null)
            {
                IQueryable<TEntity> res = _dbContext.Set<TEntity>();
                includes.ForEach(e =>
                {
                    res = res.Include(e);
                });
                return await res
                .FirstOrDefault(predicate)!;
            }

            return await _dbContext.Set<TEntity>().FirstOrDefault(predicate)!;
        }

        public virtual async Task<TEntity> GetAsync(
            string predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include)
        {
            if (include != null)
            {
                return await include(_dbContext.Set<TEntity>())
                .FirstOrDefault(predicate)!;
            }

            return await _dbContext.Set<TEntity>().FirstOrDefault(predicate)!;
        }

        public virtual IPagedList<TResult> GetPagedList<TResult>(
            IMapper mapper,
            Expression<Func<TEntity, bool>> predicate = null,
            SearchModel searchModel = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
            where TResult : class, new()
        {
            if (mapper == null)
            {
                throw new NullReferenceException
                    ($"{nameof(mapper)} parameter Is Null In {this.GetType().Name}.{MethodBase.GetCurrentMethod()!.Name}()");
            }

            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
                query = orderBy(query);

            IQueryable<TResult> result = query.ProjectTo<TResult>(mapper.ConfigurationProvider).ToList().AsQueryable();

            int pageNumber = 1;
            int pageSize = int.MaxValue;

            if (searchModel != null)
            {
                pageNumber = searchModel.PageNumber;
                pageSize = searchModel.PageSize;
            }
            var entities = result.ToPagedList(pageNumber, pageSize);

            return entities;
        }

        public virtual IPagedList<TResult> GetPagedList<TResult>(
            IMapper mapper,
            List<string> include,
            Expression<Func<TEntity, bool>> predicate = null,
            SearchModel searchModel = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
            where TResult : class, new()
        {
            if (mapper == null)
            {
                throw new NullReferenceException
                    ($"{nameof(mapper)} parameter Is Null In {this.GetType().Name}.{MethodBase.GetCurrentMethod()!.Name}()");
            }

            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (include != null && include.Any())
            {
                include.ForEach(item =>
                {
                    query = query.Include(item);
                });
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
                query = orderBy(query);

            IQueryable<TResult> result = query.ProjectTo<TResult>(mapper.ConfigurationProvider).ToList().AsQueryable();

            int pageNumber = 1;
            int pageSize = int.MaxValue;
            if (searchModel != null)
            {
                pageNumber = searchModel.PageNumber;
                pageSize = searchModel.PageSize;
            }
            var entities = result.ToPagedList(pageNumber, pageSize);

            return entities;
        }

        public virtual async Task<IPagedList<TResult>> GetPagedListAsync<TResult>(
            IMapper mapper,
            Expression<Func<TEntity, bool>> predicate = null,
            SearchModel searchModel = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
            where TResult : class, new()
        {
            if (mapper == null)
            {
                throw new NullReferenceException
                    ($"{nameof(mapper)} parameter Is Null In {this.GetType().Name}.{MethodBase.GetCurrentMethod()!.Name}()");
            }

            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
                query = orderBy(query);

            var res = await query.ToListAsync();

            IQueryable<TResult> result = res.AsQueryable().ProjectTo<TResult>(mapper.ConfigurationProvider);

            int pageNumber = 1;
            int pageSize = int.MaxValue;

            if (searchModel != null)
            {
                pageNumber = searchModel.PageNumber;
                pageSize = searchModel.PageSize;
            }
            var entities = result.ToPagedList(pageNumber, pageSize);

            return entities;
        }

        public virtual async Task<IPagedList<TResult>> GetPagedListAsync<TResult>(
            IMapper mapper,
            List<string> include,
            Expression<Func<TEntity, bool>> predicate = null,
            SearchModel searchModel = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
            where TResult : class, new()
        {
            if (mapper == null)
            {
                throw new NullReferenceException
                    ($"{nameof(mapper)} parameter Is Null In {this.GetType().Name}.{MethodBase.GetCurrentMethod()!.Name}()");
            }

            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (include != null && include.Any())
            {
                include.ForEach(item =>
                {
                    query = query.Include(item);
                });
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
                query = orderBy(query);

            var res = await query.ToListAsync();

            IQueryable<TResult> result = res.AsQueryable().ProjectTo<TResult>(mapper.ConfigurationProvider).ToList().AsQueryable();

            int pageNumber = 1;
            int pageSize = int.MaxValue;
            if (searchModel != null)
            {
                pageNumber = searchModel.PageNumber;
                pageSize = searchModel.PageSize;
            }
            var entities = result.ToPagedList(pageNumber, pageSize);

            return entities;
        }
        public virtual void Add(TEntity entity)
        {
            var codeProp = typeof(TEntity).GetProperty("Code");
            if (codeProp != null)
            {
                var count = _dbContext.Set<TEntity>().Count() + 1;
                codeProp.SetValue(entity, count.ToString($@"D{6}"));
            }
            _dbContext.Add(entity);
        }
        public virtual async Task Update(TEntity entity)
        {
            _dbContext.Update(entity);
        }
        public virtual async Task Delete(TEntity record)
        {
            _dbContext.Set<TEntity>().Remove(record);
        }
        public virtual async Task AddAsync(TEntity entity)
        {
            var codeProp = typeof(TEntity).GetProperty("Code");
            if (codeProp != null)
            {
                var count = _dbContext.Set<TEntity>().Count() + 1;
                codeProp.SetValue(entity, count.ToString($@"D{6}"));
            }
            await _dbContext.AddAsync(entity);
        }
        public virtual bool Exists(Expression<Func<TEntity, bool>> selector = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (include != null)
            {
                query = include(query);
            }
            if (selector == null)
            {
                return query.AsNoTracking().Any();
            }
            else
            {
                return query.AsNoTracking().Any(selector);
            }
        }
        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> selector = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (include != null)
            {
                query = include(query);
            }
            if (selector == null)
            {
                return await query.AsNoTracking().AnyAsync();
            }
            else
            {
                return await query.AsNoTracking().AnyAsync(selector);
            }
        }
    }
}
