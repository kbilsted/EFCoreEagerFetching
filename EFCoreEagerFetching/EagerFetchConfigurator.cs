using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EFCoreEagerFetching
{
    /// <summary>
    /// This class ensures a cache pr. root type such that the configuration is only build once pr. type in a thread-safe manner.
    /// </summary>
    public class EagerFetchConfigurator
    {
        private readonly AggregateRootConfiguration configuration = new AggregateRootConfiguration();
        private List<string> cacheConstructedIncludePaths = null;

        public void AddOneToManyRelation<TFrom, TTo>(Expression<Func<TFrom, IEnumerable<TTo>>> navigationExpression, int maxRecursionDepth)
        {
            configuration.AddRelation(navigationExpression, maxRecursionDepth);
        }

        public void AddOneToOneRelation<TFrom, TTo>(Expression<Func<TFrom, TTo>> navigationExpression, int maxRecursionDepth)
        {
            configuration.AddRelation(navigationExpression, maxRecursionDepth);
        }

        public IQueryable<T> ApplyEagerConfiguration<T>(IQueryable<T> query) where T : class 
        {
            lock (configuration)
            {
                if (cacheConstructedIncludePaths == null)
                {
                    cacheConstructedIncludePaths = configuration.BuildIncludePaths(typeof(T));
                }
            }

            var result = query;
            foreach (var path in cacheConstructedIncludePaths)
            {
                result = result.Include(path);
            }

            return result;
        }
    }
}
