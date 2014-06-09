using System;
using System.Linq.Expressions;
using Caliburn.Micro;

namespace SoLeap.Visualizer
{
    public static class PropertyChangedBaseHelper
    {
        public static string GetPropertyName<TProperty>(this PropertyChangedBase obj, Expression<Func<TProperty>> property)
        {
            return property.GetMemberInfo().Name;
        }
    }
}
