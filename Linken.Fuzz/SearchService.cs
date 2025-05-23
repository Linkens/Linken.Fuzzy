using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linken.Fuzzy
{
    [Obsolete("This class is obsolete. Use LinkFuzzyHelpers instead.")]
    public class QuerySearchService
    {
        public IQueryable<T> QueryFilter<T>(IQueryable<T> query, string? searchText, Func<T,string> f) {
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
    public static class LinkFuzzyHelpers {
        /// <summary>
        /// The FuzzyFilter extension method performs a fuzzy search on an IQueryable<T> collection using a provided string selector function and a search text. It uses the FuzzySharp.Fuzz.PartialRatio algorithm to group items by their similarity score to the search text. Only groups with a score above 15 are considered, and results are ordered by descending similarity. The method returns items from the best-matching group(s), prioritizing the highest score and any group with a score above 95. If the search text is null or whitespace, the original query is returned unfiltered.
        /// </summary>
        public static IQueryable<T> FuzzyFilter<T>(this IQueryable<T> query, string? searchText, Func<T, string> f) {
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
        /// <summary>
        /// Fuzzy filter for multiple fields, this will return the best match priotizing the first functions.
        /// </summary>
        public static IQueryable<T> FuzzyFilter<T>(this IQueryable<T> query, string? searchText, params Func<T,string>[] Funcs) {
            if (string.IsNullOrWhiteSpace(searchText)) {
                return query;
            }
            var Filtered = new List<T>();
            foreach (var f in Funcs) {
                var g = query.GroupBy(c => FuzzySharp.Fuzz.PartialRatio(searchText, f(c))).Where(g => g.Key > 15).OrderByDescending(g => g.Key);
                var BestMatch = g.Where(g => g.Key > 95).FirstOrDefault();
                if (BestMatch != null && BestMatch.Count() == 1) return BestMatch.AsQueryable();
                var first = true;
                foreach (var item in g) {
                    if (first || item.Key > 95)
                        Filtered.AddRange(item.AsEnumerable());
                    first = false;
                }
            }
            return Filtered.AsQueryable();
        }
    }
}
