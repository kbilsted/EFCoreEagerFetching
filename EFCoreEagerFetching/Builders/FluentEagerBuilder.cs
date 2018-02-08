using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCoreEagerFetching.Builders
{
    public class FluentEagerBuilder<T> where T : class
    {
        private readonly EntityTypeBuilder<T> builder;
        private readonly List<EagerFetchConfigurator> configurations;

        public FluentEagerBuilder(EntityTypeBuilder<T> builder, EagerFetchConfigurator[] configurations)
        {
            this.builder = builder;
            this.configurations = configurations.ToList();
        }

        public EntityTypeBuilder<T> HasMany<TTo>(Expression<Func<T, IEnumerable<TTo>>> navigationExpression) where TTo : class
        {
            return HasMany(navigationExpression, 1);
        }
        
        public EntityTypeBuilder<T> HasMany<TTo>(Expression<Func<T, IEnumerable<TTo>>> navigationExpression, int maxRecursionDepth) where TTo : class
        {
            configurations.ForEach(x => x.AddOneToManyRelation(navigationExpression, maxRecursionDepth));

            builder.HasMany(navigationExpression);
            
            return builder;
        }

        public EntityTypeBuilder<T> HasOne<TTo>(Expression<Func<T, TTo>> navigationExpression) where TTo : class
        {
            return HasOne(navigationExpression, 1);
        }

        public EntityTypeBuilder<T> HasOne<TTo>(Expression<Func<T, TTo>> navigationExpression, int maxRecursionDepth) where TTo : class
        {
            configurations.ForEach(x => x.AddOneToOneRelation(navigationExpression, maxRecursionDepth));

            builder.HasOne(navigationExpression);

            return builder;
        }
    }
}