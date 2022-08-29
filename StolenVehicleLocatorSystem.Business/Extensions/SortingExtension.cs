using System.Linq.Expressions;
using System.Reflection;

namespace StolenVehicleLocatorSystem.Business.Extensions
{
    public static class SortingExtension
    {
        /// <summary>
        /// Sort a IQueryable element to desc or asc 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">A data source</param>
        /// <param name="ordering"> property to order by </param>
        /// <param name="desc"> descrese or not</param>
        /// <returns>a IQueryable data source are sorted by order property</returns>
        public static IQueryable<T> OrderByPropertyName<T>(this IQueryable<T> source, string ordering, bool desc)
        {
            var type = typeof(T);
            var property = type.GetProperty(ordering, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), 
                desc ? "OrderByDescending" : "OrderBy", 
                new Type[] { type, property.PropertyType }, 
                source.Expression, Expression.Quote(orderByExp));

            return source.Provider.CreateQuery<T>(resultExp);
        }
    }
}
