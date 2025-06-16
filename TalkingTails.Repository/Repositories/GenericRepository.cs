using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TalkingTails.Repository.Data;
using TalkingTails.Repository.Helpers;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Repository.Repositories
{
    /// <summary>
    ///     All getter methods use GetQueryable() to allow child classes to override the initial query.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        protected readonly ApplicationDbContext DbContext;
        internal DbSet<T> DbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            DbContext = context;
            DbSet = DbContext.Set<T>();
        }

        public virtual IQueryable<T> GetQueryable()
        {
            return DbSet.AsQueryable();
        }

        public Task<T?> GetAsync(Expression<Func<T, bool>> filter,
            string? includeProperties = null)
        {
            return GetQueryable().Where(filter).ApplyIncludes(includeProperties).FirstOrDefaultAsync();
        }

        public Task<TTarget?> GetAsync<TTarget>(Expression<Func<T, bool>> filter) where TTarget : class
        {
            return GetQueryable().Where(filter).SelectTo<T, TTarget>()
                .FirstOrDefaultAsync();
        }

        public Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null
        )
        {
            var query = GetQueryable();
            if (filter != null)
                query = query.Where(filter);
            query = query.ApplyIncludes(includeProperties);

            return query.ToListAsync();
        }

        public Task<List<TTarget>> GetAllAsync<TTarget>(Expression<Func<T, bool>>? filter = null, string? sort = null)
            where TTarget : class
        {
            var query = GetQueryable();
            if (filter != null)
                query = query.Where(filter);

            return query.ApplySort(sort).SelectTo<T, TTarget>().ToListAsync();
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return GetQueryable().AnyAsync(predicate);
        }

        public async Task InsertAsync(T entity)
        {
            await DbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await DbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            DbSet.Update(entity);
        }

        public Task<int> ExecuteUpdateAsync(
            Expression<Func<T, bool>> predicate,
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls)
        {
            return DbSet.Where(predicate).ExecuteUpdateAsync(setPropertyCalls);
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            string? includeProperties = null
        )
        {
            return GetQueryable().Where(predicate).ApplyIncludes(includeProperties).FirstOrDefaultAsync();
        }

        public Task<Pagination<T>> GetPaginationAsync(
            int pageIndex,
            int pageSize,
            (string searchTerm, string columnName)? vietnameseSearch = null,
            Expression<Func<T, bool>>? predicate = null,
            string? includeProperties = null,
            string? sort = null
        )
        {
            var query = GetQueryable();
            if (predicate != null)
                query = query.Where(predicate);
            return query
                .ApplyVietnameseSearch(vietnameseSearch)
                .ApplyIncludes(includeProperties)
                .ApplySort(sort)
                .ToPaginatedListAsync(pageIndex, pageSize);
        }

        public Task<Pagination<TTarget>> GetPaginationAsync<TTarget>(
            int pageIndex,
            int pageSize,
            (string searchTerm, string columnName)? vietnameseSearch = null,
            Expression<Func<T, bool>>? predicate = null,
            string? sort = null
        ) where TTarget : class
        {
            var query = GetQueryable();
            if (predicate != null)
                query = query.Where(predicate);
            return query
                .ApplyVietnameseSearch(vietnameseSearch)
                .ApplySort(sort)
                .SelectTo<T, TTarget>()
                .ToPaginatedListAsync(pageIndex, pageSize);
        }
    }
}