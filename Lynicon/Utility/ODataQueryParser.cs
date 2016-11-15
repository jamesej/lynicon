using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IQDict = System.Linq.IQueryable<System.Collections.Generic.Dictionary<string, object>>;

namespace Lynicon.Utility
{
    public class ODataQueryParser
    {
        public ODataQueryParser()
        {
        }

        public Expression<Func<IQDict, IQDict>>
            Parse(List<KeyValuePair<string, string>> queryStringParts)
        {
            if (queryStringParts.Count == 0)
                return null;
            if (queryStringParts.Count == 1)
            {
                string arg = queryStringParts[0].Value;
                switch (queryStringParts[0].Key)
                {
                    case "$skip":
                        return (IQDict qbl) => qbl.Skip(int.Parse(arg));
                    case "$take":
                        return (IQDict qbl) => qbl.Take(int.Parse(arg));
                    case "$filter":
                        var expParser = new ODataExpressionParser();
                        var cond = expParser.ParseToLambda(arg);
                        return (IQDict qbl) => qbl.Where(cond);
                    default:
                        return null;
                }
            }
            var partExpressions = queryStringParts
                .Select(qsp => new { Key = qsp.Key, Value = Parse(new List<KeyValuePair<string, string>> { qsp }) })
                .Where(pe => pe.Value != null)
                .ToDictionary(pe => pe.Key, pe => pe.Value);

            Expression<Func<IQDict, IQDict>> res = null;
            partExpressions
                .OrderBy(pe => pe.Key) // happens to work the right way
                .Do(pe =>
                    {
                        if (res == null)
                            res = pe.Value;
                        else
                        {
                            res = Expression.Lambda<Func<IQDict, IQDict>>(Expression.Invoke(pe.Value, res.Body), res.Parameters);
                        }
                    });

            return res;
        }
    }
}
