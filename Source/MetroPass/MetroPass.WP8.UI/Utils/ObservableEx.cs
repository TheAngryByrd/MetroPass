using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using ReactiveUI;

namespace MetroPass.WP8.UI.Utils
{
    public static class ObservableEx
    {
        public static IObservable<IObservedChange<TSender, TValue>> ObservableForPropertyNotNull<TSender, TValue>(this TSender This, Expression<Func<TSender, TValue>> property, Boolean beforeChange = false, Boolean skipInitial = true)
        {
            if (This == null)
            {
                throw new ArgumentNullException("Sender");
            }
            String[] propertyNames = Reflection.ExpressionToPropertyNames<TSender, TValue>(property);
            return This.SubscribeToExpressionChain<TSender, TValue>(propertyNames, beforeChange, skipInitial).Where(o => o.Value != null);
        }
    }
}