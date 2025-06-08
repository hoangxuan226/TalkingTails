using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using TalkingTails.Repository.Helpers;

namespace TalkingTails.Repository.Interfaces
{
    public interface IGenericRepository<T>
    {
        IQueryable<T> GetQueryable();
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);
        Task<TTarget?> GetAsync<TTarget>(Expression<Func<T, bool>> filter) where TTarget : class;
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        Task<List<TTarget>> GetAllAsync<TTarget>(Expression<Func<T, bool>>? filter = null, string? sort = null)
            where TTarget : class;

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task InsertAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);

        Task<int> ExecuteUpdateAsync(
            Expression<Func<T, bool>> predicate,
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls);

        void Delete(T entity);

        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null);

        Task<Pagination<T>> GetPaginationAsync(int pageIndex, int pageSize,
            (string searchTerm, string columnName)? vietnameseSearch = null,
            Expression<Func<T, bool>>? predicate = null,
            string? includeProperties = null, string? sort = null);

        Task<Pagination<TTarget>> GetPaginationAsync<TTarget>(
            int pageIndex,
            int pageSize,
            (string searchTerm, string columnName)? vietnameseSearch = null,
            Expression<Func<T, bool>>? predicate = null,
            string? sort = null
        ) where TTarget : class;
    }
}