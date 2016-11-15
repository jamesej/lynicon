using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Extensibility;
using Lynicon.Utility;

namespace Lynicon.Repositories
{
    public class QueryBuilder<TConn, TCmd, TParam, TReader> : IQueryBuilder
        where TReader : DbDataReader
        where TCmd : DbCommand, new()
        where TParam : DbParameter, new()
        where TConn: DbConnection, new()
    {
        public List<string> SqlFields { get; set; }
        public List<string> SqlConditionals { get; set; }
        public Dictionary<string, object> SqlParameters { get; set; }
        public List<string> OrderBys { get; set; }
        public Dictionary<string, object> SqlSets { get; set; }
        public List<string> PreservedFields { get; set; }
        public string SqlTable { get; set; }
        public string ConnectionString { get; set; }
        public string SelectModifier { get; set; }
        public string ParamUniqueKey { get; set; }
        public object Transaction { get; set; }
        private bool StartingTransaction { get; set; }
        public bool ShouldInsert { get; set; }

        public QueryBuilder(string connString)
        {
            SqlFields = new List<string>();
            SqlConditionals = new List<string>();
            SqlParameters = new Dictionary<string, object>();
            OrderBys = new List<string>();
            SqlSets = new Dictionary<string, object>();
            PreservedFields = new List<string>();
            ConnectionString = connString;
            ParamUniqueKey = "";
            Transaction = null;
            StartingTransaction = false;
            ShouldInsert = false;
        }

        private TConn GetConn()
        {
            if (Transaction == null)
            {
                TConn conn = new TConn();
                conn.ConnectionString = ConnectionString;
                return conn;
            }
            else
                return (Transaction as DbTransaction).Connection as TConn;
        }

        protected virtual string BuildSelectText()
        {
            return "SELECT "
                + SelectModifier + " "
                + GetFieldList()
                + " FROM " + SqlTable
                + (SqlConditionals.Count == 0 ? ""
                    : " WHERE " + SqlConditionals.Join(" AND "))
                + (OrderBys.Count == 0 ? ""
                    : " ORDER BY " + OrderBys.Join(","));
        }

        protected virtual string BuildCountText()
        {
            return "SELECT Count(*)  FROM " + SqlTable
                + (SqlConditionals.Count == 0 ? ""
                    : " WHERE " + SqlConditionals.Join(" AND "));
        }

        protected virtual TCmd BuildSelect()
        {
            TCmd cmd = new TCmd();
            cmd.CommandText = BuildSelectText();
            SetParameters(cmd);

            return cmd;
        }

        protected virtual TCmd BuildCount()
        {
            TCmd cmd = new TCmd();
            cmd.CommandText = BuildCountText();
            SetParameters(cmd);

            return cmd;
        }

        protected virtual TCmd BuildPagedSelect(int skip, int take, string idField, Type idType,
            string sortField, Type sortType)
        {
            var subBuilder = new QueryBuilder<TConn, TCmd, TParam, TReader>(ConnectionString);
            subBuilder.SqlFields = this.SqlFields.ToList();
            subBuilder.SqlTable = this.SqlTable;
            subBuilder.SqlConditionals = this.SqlConditionals.ToList();
            subBuilder.OrderBys = this.OrderBys.ToList();

            string wherePart = (SqlConditionals.Count == 0 ? ""
                    : " WHERE " + SqlConditionals.Join(" AND "));

            if (skip == 0)
            {
                subBuilder.SelectModifier = "TOP " + take.ToString();
                var selCmd = subBuilder.BuildSelect();
                selCmd.CommandText += string.Format("; SELECT Count(*) FROM {0}{1};", subBuilder.SqlTable, wherePart);
                SetParameters(selCmd);
                return selCmd;
            }

            string text;
            if (string.IsNullOrEmpty(sortField))
            {
                text = string.Format(@"DECLARE @firstId {0}, @skip int = {1}, @take int = {2};
                SET ROWCOUNT @skip;
                SELECT @firstId = {3} FROM {4}{6} ORDER BY {3};

                -- Now, set the row count to MaximumRows and get
                -- all records >= @first_id
                SET ROWCOUNT @take;

                SELECT @firstId;

                SELECT {5}
                FROM {4}
                WHERE {3} >= @firstId {7}
                ORDER BY {3};

                SET ROWCOUNT 0;

                SELECT Count(*) FROM {4}{6};", DbTypeName(idType), skip + 1, take,
                idField, subBuilder.SqlTable, GetFieldList(),
                wherePart, wherePart.Replace(" WHERE ", " AND "));
            }
            else
                text = string.Format(@"DECLARE @firstValue {0}, @firstId {1}, @skip int = {2}, @take int = {3};
                SET ROWCOUNT @skip;
                SELECT @firstValue = {4}, @firstId = {5} FROM {6}{8} ORDER BY {4}, {5};

                -- Now, set the row count to MaximumRows and get
                -- all records >= @first_id
                SET ROWCOUNT @take;

                SELECT @firstValue, @firstId;

                SELECT {7}
                FROM {6}
                WHERE {5} >= @firstId AND {4} >= @firstValue{9}
                ORDER BY {4}, {5};

                SET ROWCOUNT 0;

                SELECT Count(*) FROM {6}{8};", DbTypeName(sortType), DbTypeName(idType), skip + 1, take, sortField,
                idField, subBuilder.SqlTable, GetFieldList(),
                wherePart, wherePart.Replace(" WHERE ", " AND "));

            TCmd cmd = new TCmd();
            cmd.CommandText = text;
            SetParameters(cmd);

            return cmd;
        }

        public virtual List<PropertyStore> RunPagedSelect(int skip, int take,
            string idField, Type idType, string sortField, Type sortType,
            out int count)
        {
            TCmd cmd = BuildPagedSelect(skip, take, idField, idType, sortField, sortType);
            List<PropertyStore> values = new List<PropertyStore>();
            using (TConn conn = GetConn())
            {
                conn.Open();
                cmd.Connection = conn;
                using (TReader reader = cmd.ExecuteReader() as TReader)
                {
                    if (skip > 0) reader.NextResult();
                    while (reader.Read())
                        values.Add(new PropertyStore(reader));
                    reader.NextResult();
                    reader.Read();
                    count = reader.GetInt32(0);
                }
            }

            return values;
        }

        protected virtual string DbTypeName(Type type)
        {
            switch (type.Name)
            {
                case "Int32":
                    return "integer";
                case "Guid":
                    return "uniqueidentifier";
                case "String":
                    return "nvarchar(max)";
                default:
                    throw new ArgumentException("Can't convert " + type.FullName + " to database type");
            }
        }

        protected virtual void SetParameters(TCmd cmd)
        {
            foreach (KeyValuePair<string, object> kvp in SqlParameters)
            {
                TParam param = new TParam();
                param.ParameterName = kvp.Key.Replace("@", "@" + ParamUniqueKey);
                param.Value = kvp.Value ?? DBNull.Value;
                cmd.Parameters.Add(param);
            }
        }
        protected virtual void SetUpdateParameters(TCmd cmd, bool preserveFields)
        {
            foreach (KeyValuePair<string, object> kvp in SqlSets)
            {
                if (preserveFields && PreservedFields.Contains(kvp.Key))
                    continue;
                TParam param = new TParam();
                param.ParameterName = "@" + ParamUniqueKey + "S_" + kvp.Key;
                param.Value = kvp.Value ?? DBNull.Value;
                cmd.Parameters.Add(param);
            }
        }

        public virtual IEnumerable<PropertyStore> RunSelect()
        {
            TCmd cmd = BuildSelect();
            List<PropertyStore> values = new List<PropertyStore>();
            using (TConn conn = GetConn())
            {
                conn.Open();
                cmd.Connection = conn;
                using (TReader reader = cmd.ExecuteReader() as TReader)
                {
                    while (reader.Read())
                        yield return new PropertyStore(reader);
                }
            }
        }

        public virtual int RunCount()
        {
            TCmd cmd = BuildCount();
            using (TConn conn = GetConn())
            {
                conn.Open();
                cmd.Connection = conn;
                return (int)cmd.ExecuteScalar();
            }
        }

        protected virtual TCmd BuildUpdate()
        {
            if (SqlSets.Count == 0) return null;

            TCmd cmd = new TCmd();
            cmd.CommandText = "UPDATE " + SqlTable
                + " SET "
                + SqlSets.Where(kvp => !PreservedFields.Contains(kvp.Key))
                    .Select(kvp => QuoteName(kvp.Key) + " = @" + ParamUniqueKey + "S_" + kvp.Key)
                    .Join(", ")
                + (SqlConditionals.Count == 0 ? ""
                    : " WHERE " + SqlConditionals.Select(sc => sc.Replace("@", "@" + ParamUniqueKey)).Join(" AND "));
            SetParameters(cmd);
            SetUpdateParameters(cmd, true);

            return cmd;
        }

        protected virtual TCmd BuildInsert()
        {
            if (SqlSets.Count == 0) return null;

            TCmd cmd = new TCmd();
            cmd.CommandText = "INSERT INTO " + SqlTable
                + "(" + SqlSets.Select(kvp => QuoteName(kvp.Key)).Join(", ")
                + ") VALUES ("
                + SqlSets.Select(kvp => "@" + ParamUniqueKey + "S_" + kvp.Key).Join(", ")
                + ")";
            SetUpdateParameters(cmd, false);

            return cmd;
        }

        public virtual int RunUpdateInsert()
        {
            TCmd cmd = BuildUpdate();
            return ExecuteQuery(cmd,
                c => {
                    int affected = ShouldInsert ? 0 : cmd.ExecuteNonQuery();
                    if (affected == 0)
                    {
                        var cmdIns = BuildInsert();
                        cmdIns.Connection = cmd.Connection;
                        cmdIns.Transaction = cmd.Transaction;
                        cmdIns.ExecuteNonQuery();
                    }
                    return affected;
                },
                true);
        }

        protected virtual TCmd BuildDelete()
        {
            TCmd cmd = new TCmd();
            cmd.CommandText = "DELETE FROM " + SqlTable
                + (SqlConditionals.Count == 0 ? ""
                    : " WHERE " + SqlConditionals.Select(sc => sc.Replace("@", "@" + ParamUniqueKey)).Join(" AND "));
            SetParameters(cmd);

            return cmd;
        }

        public virtual int RunDelete()
        {
            TCmd cmd = BuildDelete();
            return ExecuteQuery(cmd,
                c => cmd.ExecuteNonQuery(),
                true);
        }

        public virtual void RunMultipleUpdate(List<IQueryBuilder> queries)
        {
            StringBuilder fullQ = new StringBuilder();
            TCmd multipleCmd = new TCmd();
            int idx = 0;
            foreach (var iquery in queries)
            {
                var query = iquery as QueryBuilder<TConn, TCmd, TParam, TReader>;
                if (query == null)
                    throw new Exception("Wrong query type in multiple: " + iquery.GetType().FullName);
                query.ParamUniqueKey = idx.ToString();
                var cmd = query.ShouldInsert ? query.BuildInsert() : query.BuildUpdate();
                AddQueryToMultiple(fullQ, cmd.CommandText);
                cmd.Parameters.Cast<DbParameter>()
                    .Do(p => multipleCmd.Parameters.Add(new TParam { ParameterName = p.ParameterName, Value = p.Value }));
            }
            multipleCmd.CommandText = fullQ.ToString();

            ExecuteQuery(multipleCmd, c => c.ExecuteNonQuery(), true);
        }

        private TResult ExecuteQuery<TResult>(TCmd cmd, Func<TCmd, TResult> execute, bool useTransaction)
        {
            TConn conn = GetConn();
            DbTransaction trans = null;
            if (Transaction == null)
            {
                conn.Open();
                if (StartingTransaction)
                    Transaction = conn.BeginTransaction();
                else if (useTransaction)
                    trans = conn.BeginTransaction();
            }
            cmd.Connection = conn;
            cmd.Transaction = trans ?? (Transaction as DbTransaction);
            TResult result = default(TResult);
            try
            {
                result = execute(cmd);
            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();
                if (Transaction != null)
                {
                    (Transaction as DbTransaction).Rollback();
                    Transaction = null;
                }

                throw ex;
            }
            if (trans != null)
                trans.Commit();
            if (!StartingTransaction && Transaction == null)
                conn.Close();

            return result;
        }

        public virtual void BeginTransaction()
        {
            StartingTransaction = true;
        }

        public virtual void EnsureCommitted()
        {
            if (Transaction != null)
            {
                (Transaction as DbTransaction).Commit();
                Transaction = null;
            }
        }

        protected virtual void AddQueryToMultiple(StringBuilder sql, string addSql)
        {
            sql.Append(addSql + ";");
        }

        public virtual List<string> GetFieldNames(string tableName)
        {
            TCmd cmd = new TCmd();
            cmd.CommandText = GetFieldNamesQuery();
            TParam param = new TParam();
            param.ParameterName = "@tableName";
            param.Value = tableName;
            cmd.Parameters.Add(param);
            List<string> fieldNames = new List<string>();
            using (TConn conn = GetConn())
            {
                conn.Open();
                cmd.Connection = conn;
                TReader reader = cmd.ExecuteReader() as TReader;
                while (reader.Read())
                    fieldNames.Add(reader.GetString(0));
            }

            return fieldNames;
        }

        public string GetFieldList()
        {
            if (SqlFields.Count == 0)
                return "*";
            else
                return SqlFields.Select(f => QuoteName(f)).Join(",");
        }

        public virtual string QuoteName(string name)
        {
            if (name.StartsWith("["))
                return name;
            else
                return "[" + name + "]";
        }

        protected virtual string GetFieldNamesQuery()
        {
            return "SELECT column_name FROM information_schema.columns WHERE table_name = @tableName ORDER BY ordinal_position";
        }

        public virtual string QuoteString(string s)
        {
            return "'" + s.Replace("'", "''") + "'";
        }

        public virtual string TranslateLikePattern(string s)
        {
            return s;
        }
    }
}
