using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Utility
{
    public class ODataExpressionParser : ExpressionParser
    {
        public ODataExpressionParser()
            : base()
        {
            this.Quotes = "'";
            this.Ops =
                new List<OperatorDefinition>
                {
                    new OperatorDefinition
                    {
                         Op = "+",
                         Build = exps => Expression.Add(exps[0], exps[1]),
                         Precedence = 10
                    },
                    new OperatorDefinition
                    {
                        Op = "eq",
                        Build = exps => Expression.Equal(exps[0], exps[1]),
                        Precedence = 20
                    },
                    new OperatorDefinition
                    {
                        Op = "substringof",
                        Build = exps => Expression.Call(exps[1], typeof(string).GetMethod("Contains"), exps[0]),
                        Precedence = 20
                    },
                    new OperatorDefinition
                    {
                        Op = "or",
                        Build = exps => Expression.OrElse(exps[0], exps[1]),
                        Precedence = 30
                    },
                    new OperatorDefinition
                    {
                        Op = "and",
                        Build = exps => Expression.AndAlso(exps[0], exps[1]),
                        Precedence = 28
                    }
                };
        }

        public Expression<Func<Dictionary<string, object>, bool>> ParseToLambda(string s)
        {
            ParameterExpression dict = Expression.Parameter(typeof(Dictionary<string, object>), "_recordDict");
            Dictionary<string, ParameterExpression> paramDict = new Dictionary<string, ParameterExpression> { { "_recordDict", dict } };
            var lambda = Expression.Lambda<Func<Dictionary<string, object>, bool>>(
                Parse(paramDict, s.Tokenise(Quotes[0]).ToList()),
                dict);
            return lambda;
        }

        //public override Expression BuildReference(Dictionary<string, ParameterExpression> parameters, string name)
        //{
        //    var exp = Expression.MakeIndex(
        //        parameters["_recordDict"],
        //        typeof(Dictionary<string, object>).GetProperty("Item"),
        //        new [] { Expression.Constant(name) });
        //    return exp;
        //}
    }
}
