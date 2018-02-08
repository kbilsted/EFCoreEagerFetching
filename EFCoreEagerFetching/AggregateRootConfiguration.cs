using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EFCoreEagerFetching
{
    class AggregateRootConfiguration
    {
        public readonly List<TableRelation> Relations = new List<TableRelation>();

        public void AddRelation<TFrom, TTo>(Expression<Func<TFrom, IEnumerable<TTo>>> navigationExpression, int maxRecursionDepth)
        {
            if (navigationExpression == null)
                throw new ArgumentNullException(nameof(navigationExpression));
            if (maxRecursionDepth < 1)
                throw new ArgumentOutOfRangeException(nameof(maxRecursionDepth), "Must be 1 or above");
            if (maxRecursionDepth > 1 && typeof(TFrom) != typeof(TTo))
                throw new ArgumentOutOfRangeException(nameof(maxRecursionDepth), "Recursion depth > 1 can only be used on recursive relations");

            var lambda = navigationExpression;
            if (lambda.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentOutOfRangeException($"{nameof(navigationExpression)} must be a MemberAccess expression.");

            var memberAccess = (MemberExpression) lambda.Body;
            var toType = memberAccess.Type.GenericTypeArguments[0];

            var fieldName = memberAccess.Member.Name;

            Relations.Add(new TableRelation(maxRecursionDepth, typeof(TFrom), toType, fieldName));
        }

        public void AddRelation<T, TRelated>(Expression<Func<T, TRelated>> navigationExpression, int maxRecursionDepth)
        {
            if (navigationExpression == null)
                throw new ArgumentNullException(nameof(navigationExpression));
            if (maxRecursionDepth < 1)
                throw new ArgumentOutOfRangeException(nameof(maxRecursionDepth), "Must be 1 or above");
            if (maxRecursionDepth > 1 && typeof(T) != typeof(TRelated))
                throw new ArgumentOutOfRangeException(nameof(maxRecursionDepth), "Recursion depth > 1 can only be used on recursive relations");

            var lambda = navigationExpression;
            if (lambda.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentOutOfRangeException($"{nameof(navigationExpression)} must be a MemberAccess expression.");

            var memberAccess = (MemberExpression)lambda.Body;
            var toType = memberAccess.Type;

            var fieldName = memberAccess.Member.Name;

            Relations.Add(new TableRelation(maxRecursionDepth, typeof(T), toType, fieldName));
        }

        public List<string> BuildIncludePaths(Type rootType)
        {
            var result = new List<string>();
            BuildIncludePaths(rootType, "", result);
            return result;
        }

        void BuildIncludePaths(Type current, string parentPath, List<string> result)
        {
            var children = Relations.Where(x => x.FromType == current).ToArray();

            foreach (var child in children)
            {
                var path = Join(parentPath, child.FieldName);

                var recursivePath = path;
                for (int i = 0; i < child.MaxRecursionDepth; i++)
                {
                    result.Add(recursivePath);
                    recursivePath = Join(recursivePath, child.FieldName);
                }

                if (current != child.ToType)
                    BuildIncludePaths(child.ToType, path, result);
            }
        }

        private static string Join(string path1, string path2)
        {
            if (path1 == "")
                return path2;
            return $"{path1}.{path2}";
        }
    }
}