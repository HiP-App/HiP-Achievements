using System;
using System.Linq;
using System.Linq.Expressions;

namespace PaderbornUniversity.SILab.Hip.Achievements.Utility
{
    public static class QueryHelper
    {
        /// <summary>
        /// Applies a WHERE filter to a query if the specified condition is true.
        /// </summary>
        public static IQueryable<T> FilterIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }
    }
}
