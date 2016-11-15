using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Lynicon.Utility
{
    public class OperatorDefinition
    {
        public string Op { get; set; }
        public int Precedence { get; set; }
        public bool IsUnary { get; set; }
        public Func<List<Expression>, Expression> Build { get; set; }

        public OperatorDefinition()
        {
            Precedence = 0;
            Build = null;
        }
    }

    public class ParseResult
    {
        public Expression Expression { get; set; }
        public Dictionary<string, ParameterExpression> Parameters { get; set; }
    }

    public class ExpressionParser
    {
        public List<OperatorDefinition> Ops { get; set; }
        public string OpenBrackets { get; set; }
        public string CloseBrackets { get; set; }
        public string Quotes { get; set; }
        public Dictionary<string, Type> Variables { get; set; }

        public ExpressionParser()
        {
            Ops = new List<OperatorDefinition>();
            Ops.Add(new OperatorDefinition { Op = ",", Precedence = 0 });
            OpenBrackets = "(";
            CloseBrackets = ")";
            Variables = new Dictionary<string, Type>();
        }

        public ParseResult Parse(string s)
        {
            return Parse(s.Tokenise(Quotes[0]).ToList());
        }
        public ParseResult Parse(List<Token> tokens)
        {
            var parameters = new Dictionary<string, ParameterExpression>();
            var res = new ParseResult();
            res.Expression = Parse(parameters, tokens);
            res.Parameters = parameters;
            return res;
        }
        protected Expression Parse(Dictionary<string, ParameterExpression> parameters, List<Token> tokens)
        {
            if (tokens == null || tokens.Count == 0)
                return null;
            if (tokens.Count == 1)
            {
                if (tokens[0] is QuotedLiteralToken)
                {
                    return Expression.Constant(((QuotedLiteralToken)tokens[0]).Value);
                }
                else if (tokens[0] is WordToken)
                {
                    return BuildReference(parameters, tokens[0].ToString().ToLower());
                }
                else if (tokens[0] is NumberToken)
                {
                    return Expression.Constant((int)((NumberToken)tokens[0]).Value);
                }
                else
                    throw new ArgumentException("Unexpected token: " + tokens[0].ToString());
            }
            else if (tokens.MatchPattern("'guid'Q"))
                return Expression.Constant(new Guid(tokens[1].ToString()));
            else if (tokens.MatchPattern("'datetime'Q"))
                return Expression.Constant(XmlConvert.ToDateTime(tokens[1].ToString()));
            else if (tokens.MatchPattern("W'('"))
            {
                var funcOp = Ops.FirstOrDefault(op => op.Op == tokens[0].ToString());
                if (funcOp == null)
                    throw new ArgumentException("Unrecognised operator " + tokens[0].ToString());
                List<Expression> parms = new List<Expression>();
                OperatorDefinition opFound = new OperatorDefinition();
                OperatorDefinition opComma = new OperatorDefinition { Op = "," };
                while (opFound != null)
                {
                    var exp = Parse(parameters, GetHead(tokens, new List<OperatorDefinition> { opComma }, opFound));
                    parms.Add(exp);
                }
                return funcOp.Build(parms);
            }
            else
            {
                var topOp = GetTopOperator(tokens, Ops);
                if (topOp.Item1 == null)
                    throw new Exception("Cannot find operator");
                return topOp.Item1.Build(new List<Expression> {
                    Parse(parameters, tokens.Take(topOp.Item2).ToList()),
                    Parse(parameters, tokens.Skip(topOp.Item2 + 1).ToList())
                });
            }
        }

        public List<Token> GetHead(IEnumerable<Token> tokens, List<OperatorDefinition> ops, OperatorDefinition foundOp)
        {
            var topOp = GetTopOperator(tokens, ops);
            if (topOp.Item1 != null)
            {
                foundOp = topOp.Item1;
                return tokens.Take(topOp.Item2 - 1).ToList();
            }
            else
            {
                foundOp = null;
                return tokens.ToList();
            }
        }

        public Tuple<OperatorDefinition, int> GetTopOperator(IEnumerable<Token> tokens, List<OperatorDefinition> ops)
        {
            int pos = 0;
            int winningPos = -1;
            int bracketDepth = 0;
            OperatorDefinition winningOpDef = null;
            OperatorDefinition opDef = null;
            foreach (Token t in tokens)
            {
                opDef = ops.FirstOrDefault(op => op.Op == t.ToString().ToLower());
                if (OpenBrackets.Contains(t.ToString()))
                    bracketDepth++;
                else if (CloseBrackets.Contains(t.ToString()))
                    bracketDepth--;
                if (bracketDepth < 0)
                    break;
                if (opDef != null && (winningOpDef == null || opDef.Precedence >= winningOpDef.Precedence))
                {
                    winningOpDef = opDef;
                    winningPos = pos;
                }
                pos++;
            }

            return new Tuple<OperatorDefinition, int>(winningOpDef, winningPos);
        }

        public virtual Expression BuildReference(Dictionary<string, ParameterExpression> parameters, string name)
        {
            if (!parameters.ContainsKey(name))
            {
                if (!Variables.ContainsKey(name))
                    throw new ArgumentException("Unidentified variable: " + name);
                parameters.Add(name, Expression.Parameter(Variables[name], name));
            }
            return parameters[name];
        }
    }
}
