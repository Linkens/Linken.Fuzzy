using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linken.Fuzzy
{
    public class QuerySearchService
    {
        public IQueryable<T> QueryFilter<T>(IQueryable<T> query, string searchText, Func<T,string> f) {
            if (string.IsNullOrWhiteSpace(searchText)) {
                return query;
            }
            var Filtered = new List<T>();
            var g = query.GroupBy(c => FuzzySharp.Fuzz.PartialRatio(searchText, f(c))).Where(g => g.Key > 15).OrderByDescending(g => g.Key);
            var first = true;
            foreach (var item in g) {
                if (first || item.Key > 95)
                    Filtered.AddRange(item.AsEnumerable());
                first = false;
            }
            return Filtered.AsQueryable();
        }
    }
}
