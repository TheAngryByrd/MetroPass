using System.Linq.Expressions;
using System.Reflection;

namespace MetroPass.WP8.UI.Utils
{
    public static class PropertySupport
    {
        public static PropertyInfo GetMemberInfo(this Expression expression)
        {
            var lambda = (LambdaExpression)expression;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            return memberExpression.Member as PropertyInfo;
        }
    }
}