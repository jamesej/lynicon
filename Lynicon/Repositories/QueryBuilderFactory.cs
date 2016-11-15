using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Repositories
{
    public abstract class QueryBuilderFactory
    {
        static readonly QueryBuilderFactory instance =
            new QueryBuilderFactory<QueryBuilder<SqlConnection, SqlCommand, SqlParameter, SqlDataReader>>();
        public static QueryBuilderFactory Instance { get { return instance; } }

        public abstract IQueryBuilder Create();
    }

    public class QueryBuilderFactory<T> : QueryBuilderFactory where T : IQueryBuilder
    {
        public override IQueryBuilder Create()
        {
            return Activator.CreateInstance(typeof(T), ConfigurationManager.ConnectionStrings["LyniconContent"].ConnectionString) as IQueryBuilder;
        }
    }
}
