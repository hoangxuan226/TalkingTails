using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TalkingTails.Repository.Data;
using TalkingTails.Repository.Helpers;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Repository.Repositories
{
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

        public IQueryable<T> GetAll()
        {
            return DbContext.Set<T>().AsQueryable();
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter,
            string? includeProperties = null)
        {
            IQueryable<T> query = DbSet;
            query = query.Where(filter);
            if (includeProperties != null)
            {
                foreach (
                    var item in includeProperties.Split(
                        [','],
                        StringSplitOptions.RemoveEmptyEntries
                    )
                )
                {
                    query = query.Include(item);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>> filter,
            string? includeProperties = null
        )
        {
            IQueryable<T> query = DbSet;
            query = query.Where(filter);
            if (includeProperties != null)
            {
                foreach (
                    var item in includeProperties.Split(
                        [','],
                        StringSplitOptions.RemoveEmptyEntries
                    )
                )
                {
                    query = query.AsNoTracking().Include(item);
                }
            }
            return await query.ToListAsync();
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

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public async Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            string? includeProperties = null
        )
        {
            IQueryable<T> query = DbSet;
            query = query.Where(predicate);
            if (includeProperties != null)
            {
                foreach (
                    var item in includeProperties.Split(
                        [','],
                        StringSplitOptions.RemoveEmptyEntries
                    )
                )
                {
                    query = query.Include(item);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<Pagination<T>> GetPaginationAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            string? includeProperties = null,
            Expression<Func<T, object>>? orderBy = null,
            bool isDescending = false
        )
        {
            IQueryable<T> query = DbSet;
            if (predicate != null)
                query = query.Where(predicate);
            if (includeProperties != null)
            {
                foreach (
                    var item in includeProperties.Split(
                        [','],
                        StringSplitOptions.RemoveEmptyEntries
                    )
                )
                {
                    query = query.Include(item);
                }
            }
            if (orderBy != null)
            {
                query = isDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            }
            var itemCount = await query.CountAsync();
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
            var result = new Pagination<T>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = itemCount,
                Items = items,
            };

            return result;
        }
    }
}
