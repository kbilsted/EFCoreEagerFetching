using System;

namespace EFCoreEagerFetching
{
    public class TableRelation
    {
        public readonly int MaxRecursionDepth;
        public readonly Type FromType, ToType;
        public readonly string FieldName;

        public TableRelation(int maxRecursionDepth, Type fromType, Type toType, string fieldName)
        {
            MaxRecursionDepth = maxRecursionDepth;
            FromType = fromType;
            ToType = toType;
            FieldName = fieldName;
        }
    }
}