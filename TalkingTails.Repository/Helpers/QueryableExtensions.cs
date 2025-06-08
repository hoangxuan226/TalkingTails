using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TalkingTails.Repository.Data;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Repository.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                var idProperty = typeof(T).GetProperty("Id",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                return idProperty != null ? source.OrderBy("Id") : source;
            }

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var orderQueryBuilder = new StringBuilder();
            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;
                var propertyFromQueryName = param.Split(" ")[0];
                var objectProperty = propertyInfos.FirstOrDefault(pi =>
                    pi.Name.Equals(propertyFromQueryName, StringComparison.OrdinalIgnoreCase));
                if (objectProperty == null)
                    continue;
                var sortingOrder = param.EndsWith(" desc") ? "descending" : "ascending";
                orderQueryBuilder.Append($"{objectProperty.Name} {sortingOrder}, ");
            }

            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
            return source.OrderBy(orderQuery);
        }

        public static IQueryable<T> ApplyIncludes<T>(this IQueryable<T> source, string? includeProperties)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(includeProperties))
                return source;
            foreach (var includeProperty in includeProperties.Split([','], StringSplitOptions.RemoveEmptyEntries))
            {
                source = source.Include(includeProperty.Trim());
            }

            return source;
        }

        public static IQueryable<T> ApplyVietnameseSearch<T>(
            this IQueryable<T> source,
            (string searchTerm, string columnName)? search
        ) where T : class
        {
            if (search == null) return source;
            var (searchTerm, columnName) = search.Value;

            var standardizedSearch = searchTerm.Standardizing();
            var parameter = Expression.Parameter(typeof(T), "p");
            var property = Expression.Property(parameter, columnName); // Ví dụ: PetName
            var normalizeMethod =
                typeof(ApplicationDbContext).GetMethod(nameof(ApplicationDbContext.NormalizeVietnamese));
            var normalizedProperty = Expression.Call(null, normalizeMethod, property);
            var likePattern = Expression.Constant($"%{standardizedSearch}%");
            var likeMethod = typeof(DbFunctionsExtensions).GetMethod(
                nameof(DbFunctionsExtensions.Like),
                [typeof(DbFunctions), typeof(string), typeof(string)]
            );
            var efFunctions = Expression.Constant(EF.Functions);
            var body = Expression.Call(null, likeMethod, efFunctions, normalizedProperty, likePattern);
            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return source.Where(lambda);
        }

        #region Select fields using strings

        //public static IQueryable<dynamic> SelectFields<T>(this IQueryable<T> source, string? fields)
        //{
        //    if (string.IsNullOrWhiteSpace(fields))
        //    {
        //        return source.Cast<dynamic>();
        //    }

        //    // Get valid properties of type T
        //    var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
        //        .ToDictionary(pi => pi.Name.ToLower(), pi => pi.Name);

        //    // Get allowed fields from the query parameter
        //    var requestedFields = fields.Split(',', StringSplitOptions.RemoveEmptyEntries)
        //        .Select(f => f.Trim().ToLower())
        //        .Where(f => propertyInfos.ContainsKey(f))
        //        .Select(f => propertyInfos[f])
        //        .ToList();

        //    // If no valid fields are requested, return source as dynamic
        //    if (!requestedFields.Any())
        //    {
        //        return source.Cast<dynamic>();
        //    }

        //    // Create a dynamic select expression
        //    var selectQuery = $"new {{ {string.Join(",", requestedFields)} }}";
        //    return source.Select(selectQuery).Cast<dynamic>();
        //}

        #endregion

        public static IQueryable<TTarget> SelectTo<T, TTarget>(this IQueryable<T> source)
            where TTarget : class
        {
            // Get valid properties of type T
            var entityProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(pi => pi.Name.ToLower(), pi => pi.Name);

            // Get properties of TTarget
            var targetProperties = typeof(TTarget).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(pi => pi.Name.ToLower(), pi => pi.Name);

            // Get properties that exist in both T and TTarget
            var commonFields = targetProperties.Keys
                .Where(f => entityProperties.ContainsKey(f))
                .Select(f => new { TargetName = targetProperties[f], EntityName = entityProperties[f] })
                .ToList();

            // Create select expression: new TTarget { Prop1 = source.Prop1, Prop2 = source.Prop2 }
            var parameter = Expression.Parameter(typeof(T), "x");

            // Automatic field bindings
            var bindings = commonFields.Select(f =>
                Expression.Bind(
                    typeof(TTarget).GetProperty(f.TargetName)!,
                    Expression.Property(parameter, f.EntityName)
                )
            ).ToList();

            // Check for IMappable<T> and process special mappings
            if (typeof(IMappable<T>).IsAssignableFrom(typeof(TTarget)))
            {
                var mappingsProp = typeof(TTarget).GetProperty("Mappings", BindingFlags.Public | BindingFlags.Static);
                if (mappingsProp != null)
                {
                    var mappings = (Dictionary<string, Expression<Func<T, object>>>)mappingsProp.GetValue(null)!;
                    foreach (var targetProp in typeof(TTarget).GetProperties(
                                 BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (mappings.TryGetValue(targetProp.Name, out var sourceSelector))
                        {
                            var visitor = new ParameterReplaceVisitor(sourceSelector.Parameters[0], parameter);
                            var sourceExpression = visitor.Visit(sourceSelector.Body);
                            if (sourceExpression is UnaryExpression unary)
                            {
                                sourceExpression = unary.Operand;
                            }

                            var finalExpression = sourceExpression.Type != targetProp.PropertyType
                                ? Expression.Convert(sourceExpression, targetProp.PropertyType)
                                : sourceExpression;
                            bindings.Add(Expression.Bind(targetProp, finalExpression));
                        }
                    }
                }
            }

            if (!bindings.Any())
            {
                return (IQueryable<TTarget>)source;
            }

            // Create select expression
            var newExpression = Expression.New(typeof(TTarget));
            var memberInit = Expression.MemberInit(newExpression, bindings);
            var lambda = Expression.Lambda<Func<T, TTarget>>(memberInit, parameter);

            return source.Select(lambda);
        }

        public static async Task<Pagination<T>> ToPaginatedListAsync<T>(
            this IQueryable<T> source,
            int pageIndex,
            int pageSize
        ) where T : class
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new Pagination<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = count,
                Items = items
            };
        }

        #region Helper to replace parameters in expressions

        private class ParameterReplaceVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            : ExpressionVisitor
        {
            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == oldParameter ? newParameter : base.VisitParameter(node);
            }
        }

        #endregion
    }
}