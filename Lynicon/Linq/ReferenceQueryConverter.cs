using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Utility;

namespace Lynicon.Linq
{
    public static class ReferenceQueryConverterX
    {
        public static Func<IQueryable<TContainer>, IQueryable<TContainer>>
            ApplyToContainer<T, TContainer>(this Func<IQueryable<T>, IQueryable<T>> queryBody, IQueryable<TContainer> containerSource)
        {
            var contentExpression = queryBody(new List<T>().AsQueryable()).Expression;
            return (IQueryable<TContainer> iq) =>
                {
                    var containerExp = new ReferenceQueryConverter(iq.Expression, typeof(T), typeof(TContainer)).Visit(contentExpression);
                    return iq.Provider.CreateQuery<TContainer>(containerExp);
                };
        }
    }

    public class ReferenceQueryConverter : ExpressionVisitor
    {
        Expression innerExpression = null;
        Type from, to;
        public ReferenceQueryConverter(Expression innerExpression, Type contentType, Type containerType)
        {
            this.innerExpression = innerExpression;
            this.from = contentType;
            this.to = containerType;
        }
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (typeof(IQueryable).IsAssignableFrom(node.Type))
                return ((IQueryable)node.Value).Expression;

            return base.VisitConstant(node);
        }

        private Dictionary<ParameterExpression, ParameterExpression> parameterMappings = new Dictionary<ParameterExpression, ParameterExpression>();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            Type changedType = ReflectionX.SubstituteType(node.Type, from, to);
            if (changedType != null)
            {
                if (!parameterMappings.ContainsKey(node))
                    parameterMappings.Add(node, Expression.Parameter(changedType, node.Name));
                return parameterMappings[node];
            }

            return base.VisitParameter(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            Type changedType = ReflectionX.SubstituteType(typeof(T), from, to);
            if (changedType != null)
                return Expression.Lambda(changedType, Visit(node.Body),
                    node.Parameters.Select(p => (ParameterExpression)Visit(p)));

            return base.VisitLambda<T>(node);
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            //if (node.Member.DeclaringType.IsAssignableFrom(from))
            //{
            //    string queryRef = node.Member.Name + "=" + id.ToString();
            //    return ci => ci.References.Contains(queryRef);
            //}

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var mi = node.Method;
            if (mi.IsGenericMethod)
            {
                Type[] changedTypes = ReflectionX.SubstituteTypes(mi.GetGenericArguments(), from, to);
                if (changedTypes != null)
                {
                    MethodInfo newMi = mi.GetGenericMethodDefinition().MakeGenericMethod(changedTypes);
                    if (node.Object == null)
                        return Expression.Call(newMi,
                            node.Arguments.Select(a => Visit(a)));
                    else
                        return Expression.Call(node.Object, newMi,
                            node.Arguments.Select(a => Visit(a)));
                }
            }
            return base.VisitMethodCall(node);
        }
    }
}
