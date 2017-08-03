namespace Core.Common
{
    using System;
    using System.Linq.Expressions;

    public static class Caster<TSource, TTarget>
    {
        public static readonly Func<TSource, TTarget> Cast = UncheckedCast();

        private static Func<TSource, TTarget> UncheckedCast()
        {
            return ((Expression<Func<TSource, TTarget>>) (source => (TTarget) Convert.ChangeType(source, typeof(TTarget)))).Compile();
        }
    }
}