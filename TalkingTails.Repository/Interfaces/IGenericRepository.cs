using System.Linq.Expressions;
using TalkingTails.Repository.Helpers;

namespace TalkingTails.Repository.Interfaces
{
    public interface IGenericRepository<T>
    {
        IQueryable<T> GetAll();
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);
        Task InsertAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            string? includeProperties = null
        );

        Task<Pagination<T>> GetPaginationAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            string? includeProperties = null,
            Expression<Func<T, object>>? orderBy = null,
            bool isDescending = false
        );
    }
}
