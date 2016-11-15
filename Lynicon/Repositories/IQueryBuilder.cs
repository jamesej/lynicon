using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Extensibility;

namespace Lynicon.Repositories
{
    public interface IQueryBuilder
    {
        List<string> SqlFields { get; set; }
        List<string> SqlConditionals { get; set; }
        Dictionary<string, object> SqlParameters { get; set; }
        List<string> OrderBys { get; set; }
        Dictionary<string, object> SqlSets { get; set; }
        List<string> PreservedFields { get; set; }
        string SqlTable { get; set; }
        string ConnectionString { get; set; }
        IEnumerable<PropertyStore> RunSelect();
        int RunCount();
        int RunUpdateInsert();
        int RunDelete();
        List<string> GetFieldNames(string tableName);
        string GetFieldList();
        string QuoteName(string name);
        string QuoteString(string s);
        string SelectModifier { get; set; }
        string ParamUniqueKey { get; set; }
        void RunMultipleUpdate(List<IQueryBuilder> queries);
        bool ShouldInsert { get; set; }
        List<PropertyStore> RunPagedSelect(int skip, int take, string idField, Type idType, string sortField, Type sortType, out int count);
        string TranslateLikePattern(string queryKey);
    }
}
