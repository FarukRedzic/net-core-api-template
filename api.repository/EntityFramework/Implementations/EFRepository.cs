using api.repository.EntityFramework.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace api.repository.EntityFramework.Implementations
{
    public class EFRepository<TContext> : IEFRepository<TContext> where TContext : DbContext
    {
        private readonly TContext _context;

        public EFRepository(TContext context)
        {
            _context = context;
        }

        protected virtual IQueryable<TEntity> GetQuery<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool trackEntities = true,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> thenInclude = null,
            params Expression<Func<TEntity, object>>[] includes) where TEntity : class, new()
        {
            var query = trackEntities ?
                _context.Set<TEntity>() :
                _context.Set<TEntity>().AsNoTracking(); // Set AsNoTracking for queries which only have SELECT purpose. When fetching readonly data.

            if (filter != null)
                query = query.Where(filter);

            if (includes != null)
                query = includes.Aggregate(query, (entity, include) => entity.Include(include));

            if (thenInclude != null)
                query = thenInclude(query);

            if (orderBy != null)
                query = orderBy(query);

            if (skip != null)
                query = query.Skip(skip.Value);

            if (take != null)
                query = query.Take(take.Value);

            return query;
        }

        public void Create<TEntity>(TEntity entity) where TEntity : class, new()
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Set<TEntity>().Add(entity);
        }

        public async Task CreateAsync<TEntity>(TEntity entity) where TEntity : class, new()
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _context.Set<TEntity>().AddAsync(entity);
        }

        public void CreateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, new()
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _context.Set<TEntity>().AddRange(entities);
        }

        public async Task CreateRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, new()
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await _context.Set<TEntity>().AddRangeAsync(entities);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class, new()
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (_context.Entry(entity).State == EntityState.Detached)
                _context.Entry(entity).State = EntityState.Deleted;

            _context.Set<TEntity>().Remove(entity);
        }

        public void Delete<TEntity>(object id) where TEntity : class, new()
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var entity = this.GetById<TEntity>(id);

            this.Delete(entity);
        }

        public void DeleteRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, new()
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                if (_context.Entry(entity).State == EntityState.Detached)
                    _context.Entry(entity).State = EntityState.Deleted;
            }

            _context.Set<TEntity>().RemoveRange(entities);
        }

        public IEnumerable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int? skip = null, int? take = null, bool trackEntities = false, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> thenInclude = null, params Expression<Func<TEntity, object>>[] includes) where TEntity : class, new()
        {
            return this.GetQuery(filter, orderBy, skip, take, trackEntities, thenInclude, includes).ToList();
        }

        public IEnumerable<TEntity> GetAll<TEntity>(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int? skip = null, int? take = null, bool trackEntities = false, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> thenInclude = null, params Expression<Func<TEntity, object>>[] includes) where TEntity : class, new()
        {
            return this.GetQuery(null, orderBy, skip, take, trackEntities, thenInclude, includes).ToList();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int? skip = null, int? take = null, bool trackEntities = false, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> thenInclude = null, params Expression<Func<TEntity, object>>[] includes) where TEntity : class, new()
        {
            return await this.GetQuery(null, orderBy, skip, take, trackEntities, thenInclude, includes).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int? skip = null, int? take = null, bool trackEntities = false, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> thenInclude = null, params Expression<Func<TEntity, object>>[] includes) where TEntity : class, new()
        {
            return await this.GetQuery(filter, orderBy, skip, take, trackEntities, thenInclude, includes).ToListAsync();
        }

        public TEntity GetById<TEntity>(params object[] id) where TEntity : class, new()
        {
            return _context.Set<TEntity>().Find(id);
        }

        public async Task<TEntity> GetByIdAsync<TEntity>(params object[] id) where TEntity : class, new()
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public int GetCount<TEntity>(Expression<Func<TEntity, bool>> filter = null) where TEntity : class, new()
        {
            return this.GetQuery(filter).Count(filter);
        }

        public async Task<int> GetCountAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null) where TEntity : class, new()
        {
            return await this.GetQuery(filter).CountAsync();
        }

        public bool GetExists<TEntity>(Expression<Func<TEntity, bool>> filter = null) where TEntity : class, new()
        {
            return this.GetQuery(filter).Any();
        }

        public async Task<bool> GetExistsAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null) where TEntity : class, new()
        {
            return await this.GetQuery(filter).AnyAsync();
        }

        public TEntity GetFirst<TEntity>(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool trackEntities = false, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> thenInclude = null, params Expression<Func<TEntity, object>>[] includes) where TEntity : class, new()
        {
            return this.GetQuery(filter, orderBy, null, null, trackEntities, thenInclude, includes).FirstOrDefault();
        }

        public async Task<TEntity> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool trackEntities = false, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> thenInclude = null, params Expression<Func<TEntity, object>>[] includes) where TEntity : class, new()
        {
            return await this.GetQuery(filter, orderBy, null, null, trackEntities, thenInclude, includes).FirstOrDefaultAsync();
        }

        public long GetLongCount<TEntity>(Expression<Func<TEntity, bool>> filter = null) where TEntity : class, new()
        {
            return this.GetQuery(filter).LongCount();
        }

        public async Task<long> GetLongCountAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null) where TEntity : class, new()
        {
            return await this.GetQuery(filter).LongCountAsync();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class, new()
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (_context.Entry(entity).State == EntityState.Detached)
                _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        //public void SoftDelete<TEntity>(TEntity entity) where TEntity : class, new()
        //{
        //    if (entity == null)
        //        throw new ArgumentNullException(nameof(entity));

        //    (entity as ISoftDeleteableEntity).IsDeleted = true; // ISoftDeleteableEntity is interface which can have IsDeleted prop. All entities should implement it. To enable soft delete and soft restore

        //    this.Update(entity);
        //}

        //public void SoftDelete<TEntity>(object id) where TEntity : class, new()
        //{
        //    if (id == null)
        //        return;

        //    var entity = this.GetById<TEntity>(id);

        //    this.SoftDelete(entity);
        //}

        //public void Restore<TEntity>(TEntity entity) where TEntity : class, new()
        //{
        //    if (entity == null)
        //        throw new ArgumentNullException(nameof(entity));

        //    (entity as ISoftDeleteableEntity).IsDeleted = false; // ISoftDeleteableEntity is interface which can have IsDeleted prop. All entities should implement it. To enable soft delete and soft restore

        //    this.Update(entity);
        //}

        //public void Restore<TEntity>(object id) where TEntity : class, new()
        //{
        //    if (id == null)
        //        return;

        //    var entity = this.GetById<TEntity>(id);

        //    this.Restore(entity);
        //}
    }
}
