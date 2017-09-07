/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EDUGraphAPI
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return items == null || items.Count() == 0;
        }

        public static bool IsNotNullAndEmpty<T>(this IEnumerable<T> items)
        {
            return items != null && items.Any();
        }

        public static bool In<T>(this T t, params T[] c)
        {
            return c.Any(i => EqualityComparer<T>.Default.Equals(t, i));
        }

        public static bool IgnoreCaseIn(this string s, params string[] items)
        {
            return items.Any(i => StringComparer.InvariantCultureIgnoreCase.Equals(s, i));
        }

        public static bool IgnoreCaseEquals(this string s, string other)
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals(s, other);
        }

        public static bool IgnoreCaseNotEquals(this string s, string other)
        {
            return !StringComparer.InvariantCultureIgnoreCase.Equals(s, other);
        }

        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value = default(TValue))
        {
            dictionary.TryGetValue(key, out value);
            return value;
        }

        //public static async Task<TElement[]> ExecuteAllAsnyc<TElement>(this IReadOnlyQueryableSet<TElement> queryableSet)
        //{
        //    var pagedCollection = await queryableSet.ExecuteAsync();
        //    return await ExecuteAllAsnyc(pagedCollection);
        //}

        //public static async Task<IDirectoryObject[]> ExecuteAllAsnyc(this IDirectoryObjectCollection collection)
        //{
        //    var pagedCollection = await collection.ExecuteAsync();
        //    return await ExecuteAllAsnyc(pagedCollection);
        //}

        //public static async Task<TElement[]> ExecuteAllAsnyc<TElement>(this IPagedCollection<TElement> pagedCollection)
        //{
        //    var collection = pagedCollection;
        //    var list = new List<TElement>();

        //    while (true)
        //    {
        //        list.AddRange(collection.CurrentPage);
        //        if (!collection.MorePagesAvailable) break;
        //        collection = await collection.GetNextPageAsync();
        //    }
        //    return list.ToArray();
        //}

        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, Func<T, bool> predicate, bool condition)
        {
            return condition ? source.Where(predicate) : source;
        }

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, bool condition)
        {
            return condition ? source.Where(predicate) : source;
        }

        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            return dict.ContainsKey(key) ? dict[key] : defaultValue;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source) action(item);
            return source;
        }
    }
}