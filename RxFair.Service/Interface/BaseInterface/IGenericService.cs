using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RxFair.Service.Interface.BaseInterface
{
    public interface IGenericService<T> where T : class
    {
        T GetSingle(Expression<Func<T, bool>> predicate, bool asNoTracking = false, params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false, params Expression<Func<T, object>>[] includeProperties);

        IEnumerable<T> GetAll(bool asNoTracking = false);
        IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, bool asNoTracking = false, params Expression<Func<T, object>>[] includePropertie);

        T GetById(object id);
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate, bool asNoTracking = false);

        int GetCount(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        void Add(T entity);
        void AddAsync(T entity);
        void AddRange(IEnumerable<T> entity);
        void AddRangeAsync(IEnumerable<T> entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entity);
        void Save();

        void Detach(T entity);

        Task<T> InsertAsync(T entity, IHttpContextAccessor accessor, long? userId = null);
        Task<T> UpdateAsync(T entity, IHttpContextAccessor accessor, long? userId = null);
        Task<IEnumerable<T>> InsertRangeAsync(IEnumerable<T> entity, IHttpContextAccessor accessor, long? userId = null);
        void DeleteRange(IEnumerable<T> entity, IHttpContextAccessor accessor, long? userId = null);
        Task<T> DeleteAsync(T entity, IHttpContextAccessor accessor, long? userId = null);
        Task SaveAsync();


    }
}
