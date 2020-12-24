using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RxFair.Data.DbContext;
using RxFair.Service.Interface.BaseInterface;

namespace RxFair.Service.Implemetation.BaseService
{
    public class GenericRepository<T> : IGenericService<T> where T : class
    {

        private readonly RxFairDbContext _dbContext;
        private readonly DbSet<T> _table;

        protected GenericRepository(RxFairDbContext context)
        {
            _dbContext = context;
            _table = context.Set<T>();
            //_assessor = accessor;
        }

        public T GetSingle(Expression<Func<T, bool>> predicate, bool asNoTracking = false, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return asNoTracking ? query.AsNoTracking().SingleOrDefault(predicate) : query.SingleOrDefault(predicate);
        }
        public virtual IEnumerable<T> GetAll(bool asNoTracking = false)
        {
            return asNoTracking ? _table.AsNoTracking() : _table.AsEnumerable();
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            if (asNoTracking)
                return await query.AsNoTracking().SingleOrDefaultAsync(predicate);
            return await query.SingleOrDefaultAsync(predicate);
        }
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, bool asNoTracking = false, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return asNoTracking ? query.AsNoTracking().Where(predicate) : query.Where(predicate).AsEnumerable();
        }

        public T GetById(object id)
        {
            return _table.Find(id);
        }

        public int GetCount(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return query.Count(predicate);
        }

        public virtual IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate, bool asNoTracking = false)
        {
            return asNoTracking ? _table.AsNoTracking().Where(predicate) : _table.Where(predicate).AsEnumerable();
        }

        public virtual void Add(T entity)
        {
            _table.Add(entity);
        }

        public virtual void AddRange(IEnumerable<T> entity)
        {
            _table.AddRange(entity);
        }

        public virtual void Update(T entity)
        {
            _table.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public virtual void Delete(T entity)
        {
            _table.Remove(entity);
            Save();
        }

        public void DeleteRange(IEnumerable<T> entity)
        {
            _table.RemoveRange(entity);
            Save();
        }

        public virtual void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            var entities = _table.Where(predicate);

            foreach (var entity in entities)
            {
                _dbContext.Entry<T>(entity).State = EntityState.Deleted;
            }
            Save();
        }

        public virtual void Save()
        {
            _dbContext.SaveChanges();
        }

        public virtual void Detach(T entity)
        {
            EntityEntry dbEntityEntry = _dbContext.Entry<T>(entity);
            switch (dbEntityEntry.State)
            {
                case EntityState.Modified:
                    dbEntityEntry.State = EntityState.Unchanged;
                    break;
                case EntityState.Added:
                    dbEntityEntry.State = EntityState.Detached;
                    break;
                case EntityState.Deleted:
                    dbEntityEntry.Reload();
                    break;
            }
        }

        /* add With Ranage Async*/
        public virtual void AddAsync(T entity)
        {
            _table.AddAsync(entity);
        }

        public virtual void AddRangeAsync(IEnumerable<T> entity)
        {
            _table.AddRangeAsync(entity);
        }

        public async Task<IEnumerable<T>> InsertRangeAsync(IEnumerable<T> entity, IHttpContextAccessor accessor, long? userId)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var insertRangeAsync = entity.ToList();
            for (var i = 0; i < insertRangeAsync.Count; i++)
            {
                //Update default fields for addition and updation
                insertRangeAsync[i] = UpdateDefaultFieldsForAddAndUpdate(insertRangeAsync[i], userId);
            }
            _table.AddRange(insertRangeAsync);
            await _dbContext.SaveChangesAsync();

            return insertRangeAsync;
        }

        /// <summary>
        /// Insert Async Data and Save it to database
        /// </summary>
        /// <param name="entity">Entity to insert</param>
        /// <param name="accessor"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<T> InsertAsync(T entity, IHttpContextAccessor accessor, long? userId)
        {
            //Update default fields for addition and updation
            entity = UpdateDefaultFieldsForAddAndUpdate(entity, userId);

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            await Task.FromResult(_table.AddAsync(entity));
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        /* Update With Async*/
        public async Task<T> UpdateAsync(T entity, IHttpContextAccessor accessor, long? userId)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity = UpdateDefaultFieldsForAddAndUpdate(entity, userId, true);
            await Task.FromResult(_dbContext.Entry<T>(entity).State = EntityState.Modified);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        /* Delete Range*/
        public void DeleteRange(IEnumerable<T> entity, IHttpContextAccessor accessor, long? userId)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var enumerable = entity.ToList();
            _dbContext.Set<T>().RemoveRange(enumerable);
            _dbContext.SaveChanges();
        }

        /* Delete With Async*/
        public async Task<T> DeleteAsync(T entity, IHttpContextAccessor accessor, long? userId)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            //Update default fields for addition and updation
            entity = UpdateDefaultFieldsForAddAndUpdate(entity, userId, true);

            await Task.FromResult(_dbContext.Entry<T>(entity).State = EntityState.Deleted);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }


        public T UpdateDefaultFieldsForAddAndUpdate(T entity, long? userId, bool isEdit = false)
        {
            //Add createdBy and CreadetDate
            if (!isEdit)
            {
                if (entity.GetType().GetProperty("CreatedBy") != null)
                {
                    entity.GetType().GetProperty("CreatedBy")?.SetValue(entity, userId);
                }
                if (entity.GetType().GetProperty("CreatedDate") != null)
                {
                    entity.GetType().GetProperty("CreatedDate")?.SetValue(entity, DateTime.UtcNow);
                }
            }
            //Add updatedby and updatedDate
            else
            {
                if (entity.GetType().GetProperty("ModifiedBy") != null)
                {
                    entity.GetType().GetProperty("ModifiedBy")?.SetValue(entity, userId);
                }
                if (entity.GetType().GetProperty("ModifiedDate") != null)
                {
                    entity.GetType().GetProperty("ModifiedDate")?.SetValue(entity, DateTime.UtcNow);
                }
            }

            return entity;
        }
    }
}
