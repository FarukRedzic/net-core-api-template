using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace api.repository.EntityFramework.Interfaces
{
    public interface IEFRepository<TContext> where TContext : DbContext
    {
        IEnumerable<TEntity> GetAll<TEntity>(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool trackEntities = false,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> thenInclude = null,
            params Expression<Func<TEntity, object>>[] includes) where TEntity : class, new();

        Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool trackEntities = false,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> thenInclude = null,
            params Expression<Func<TEntity, object>>[] includes) where TEntity : class, new();

        IEnumerable<TEntity> Get<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool trackEntities = false,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> thenInclude = null,
            params Expression<Func<TEntity, object>>[] includes) where TEntity : class, new();

        Task<IEnumerable<TEntity>> GetAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool trackEntities = false,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> thenInclude = null,
            params Expression<Func<TEntity, object>>[] includes) where TEntity : class, new();

        TEntity GetFirst<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool trackEntities = false,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> thenInclude = null,
            params Expression<Func<TEntity, object>>[] includes) where TEntity : class, new();

        Task<TEntity> GetFirstAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool trackEntities = false,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> thenInclude = null,
            params Expression<Func<TEntity, object>>[] includes) where TEntity : class, new();

        TEntity GetById<TEntity>(params object[] id) where TEntity : class, new();

        Task<TEntity> GetByIdAsync<TEntity>(params object[] id) where TEntity : class, new();

        int GetCount<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, new();

        Task<int> GetCountAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, new();

        long GetLongCount<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, new();

        Task<long> GetLongCountAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, new();

        bool GetExists<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, new();

        Task<bool> GetExistsAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, new();

        void Create<TEntity>(TEntity entity)
            where TEntity : class, new();

        Task CreateAsync<TEntity>(TEntity entity)
            where TEntity : class, new();

        void CreateRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class, new();

        Task CreateRangeAsync<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class, new();

        void Update<TEntity>(TEntity entity)
            where TEntity : class, new();

        void Delete<TEntity>(TEntity entity)
            where TEntity : class, new();

        void Delete<TEntity>(object id)
            where TEntity : class, new();

        void DeleteRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class, new();

        void SaveChanges();

        Task SaveChangesAsync();

        //void SoftDelete<TEntity>(TEntity entity)
        //    where TEntity : class, new();

        //void SoftDelete<TEntity>(object id)
        //    where TEntity : class, new();

        //void Restore<TEntity>(TEntity entity)
        //    where TEntity : class, new();

        //void Restore<TEntity>(object id)
        //    where TEntity : class, new();
    }
}
