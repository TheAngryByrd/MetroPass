using System;
using System.Linq.Expressions;
using ReactiveUI;

namespace MetroPass.WP8.UI.Utils
{
    public static class ReactiveUINavigateExtentions
    {
        public static void Navigate<T>(this INavigateCommand command)
        {
            var VM = RxApp.DependencyResolver.GetService<T>();
            command.Execute(VM);
        }

        public static void Navigate<T, TParam>(this INavigateCommand command, Expression<Func<T, TParam>> paramExpression, TParam value)
        {
            var VM = RxApp.DependencyResolver.GetService<T>();
            paramExpression.GetMemberInfo().SetValue(VM, value);
            command.Execute(VM);
        }
    }
}