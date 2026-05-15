using CR.DtoBase;
using System.Linq.Expressions;

namespace CR.ApplicationBase.Common
{
    public static class PagingExtension
    {
        /// <summary>
        /// Hàm phân trang 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        ///  <param name="query"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IQueryable<T> Paging<T>(this IQueryable<T> query, PagingRequestBaseDto input)
        {   
            // Nếu dùng phân trang theo pageNumber và pageSize,
            //  nhưng pageSize = -1 thì trả về toàn bộ dữ liệu (không phân trang)
            if (input.IsPagingByPage() && input.PageSize == -1)
            {
                return query; 
            }

            return query.Skip(input.GetSkip()).Take(input.GetTake());
        }


        /// <summary>
        /// Phân trang có sort
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IQueryable<T> PagingAndSorting<T>(this IQueryable<T> query, PagingRequestBaseDto input)
        where T : class
        {
            if(input.Sort?.Any() == true)
            {
                query = query.OrderDynamic(input.Sort); 
            }
            return query.Paging(input);
        }

        /// <summary>
        /// Sắp xếp động theo danh sách sort strings
        /// Format: "PropertyName" hoặc "PropertyName asc" hoặc "PropertyName desc"
        /// Ví dụ: ["Name asc", "Age desc"]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="sortList"></param>
        /// <returns></returns>
        public static IQueryable<T> OrderDynamic<T>(this IQueryable<T> query, List<string> sortList)
        where T : class
        {
            if (sortList == null || !sortList.Any())
                return query;

            IOrderedQueryable<T>? orderedQuery = null;

            foreach (var sortItem in sortList)
            {
                if (string.IsNullOrWhiteSpace(sortItem))
                    continue;

                var parts = sortItem.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var propertyName = parts[0];
                var isDescending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

                // Validate property exists
                var propertyInfo = typeof(T).GetProperty(propertyName, 
                    System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (propertyInfo == null)
                    continue;

                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, propertyInfo);
                var lambda = Expression.Lambda(property, parameter);

                if (orderedQuery == null)
                {
                    // First sort
                    var method = isDescending ? "OrderByDescending" : "OrderBy";
                    orderedQuery = (IOrderedQueryable<T>)query.Provider.CreateQuery(
                        Expression.Call(
                            typeof(Queryable),
                            method,
                            new[] { typeof(T), propertyInfo.PropertyType },
                            query.Expression,
                            lambda
                        )
                    );
                }
                else
                {
                    // Subsequent sorts
                    var method = isDescending ? "ThenByDescending" : "ThenBy";
                    orderedQuery = (IOrderedQueryable<T>)orderedQuery.Provider.CreateQuery(
                        Expression.Call(
                            typeof(Queryable),
                            method,
                            new[] { typeof(T), propertyInfo.PropertyType },
                            orderedQuery.Expression,
                            lambda
                        )
                    );
                }
            }

            return orderedQuery ?? query;
        }
    }
}