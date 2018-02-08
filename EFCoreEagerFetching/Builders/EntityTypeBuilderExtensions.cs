using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCoreEagerFetching.Builders
{
    public static class EntityTypeBuilderExtensions
    {
        /// <summary>
        /// Configure for a number of configurations an eager fetch of a relation
        /// </summary>
        public static FluentEagerBuilder<T> Eager<T>(this EntityTypeBuilder<T> builder, params EagerFetchConfigurator[] configurations)
            where T : class
        {
            return new FluentEagerBuilder<T>(builder, configurations);
        }
    }
}