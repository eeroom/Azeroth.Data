using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// 数据库上下文，对应于连接的数据库
    /// </summary>
    public abstract class DbContext : IDbContext
    {
        protected string Cnnstr { get; set; }

        protected List<IExecuteNonQuery> executeNonQueryDbSetCollection = new List<IExecuteNonQuery>();
        protected virtual string GetDbParameterNamePrefix()
        {
            return "@";
        }

        protected virtual System.Data.Common.DbConnection CreateConnection()
        {
            return new System.Data.SqlClient.SqlConnection();
        }

        public DbSet<T> Set<T>()
        {
            return new DbSet<T>(this);
        }

        public DbSetAdd<T> Add<T>(T entity)
        {
            var dbset = new DbSetAdd<T>(new List<T>() { entity });
            this.executeNonQueryDbSetCollection.Add(dbset);
            return dbset;
        }

        public DbSetAdd<T> AddRange<T>(IEnumerable<T> lst)
        {
            var dbset = new DbSetAdd<T>(lst);
            this.executeNonQueryDbSetCollection.Add(dbset);
            return dbset;
        }

        public DbSetEdit<T> Edit<T>(T entity)
        {
            var dbset = new DbSetEdit<T>(new List<T>() { entity });
            this.executeNonQueryDbSetCollection.Add(dbset);
            return dbset;
        }

        public DbSetEdit<T> EditRange<T>(IEnumerable<T> lst)
        {
            var dbset = new DbSetEdit<T>(lst);
            this.executeNonQueryDbSetCollection.Add(dbset);
            return dbset;
        }

        public DbSetEditSimple<T> Edit<T>()
        {
            var dbset = new DbSetEditSimple<T>();
            this.executeNonQueryDbSetCollection.Add(dbset);
            return dbset;
        }

        public DbSetDel<T> Delete<T>(T entity)
        {
            var dbset = new DbSetDel<T>(new List<T>() { entity });
            this.executeNonQueryDbSetCollection.Add(dbset);
            return dbset;
        }

        public DbSetDel<T> DeleteRange<T>(IEnumerable<T> lst)
        {
            var dbset = new DbSetDel<T>(lst);
            this.executeNonQueryDbSetCollection.Add(dbset);
            return dbset;
        }

        public DbSetDelSimple<T> Delete<T>()
        {
            var dbset = new DbSetDelSimple<T>();
            this.executeNonQueryDbSetCollection.Add(dbset);
            return dbset;
        }

        Tuple<List<T>,int> IDbContext.ToList<T>(Func<DbDataReader, T> map, Action<ParseSqlContext, bool> initParseSqlContext)
        {
            using (var cnn = this.CreateConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var parseSqlContext = new ParseSqlContext(this.GetDbParameterNamePrefix(), cmd.CreateParameter);
                    initParseSqlContext(parseSqlContext, true);
                    cmd.CommandText = this.Parse(parseSqlContext);
                    cnn.Open();
                    using (var reader = cmd.ExecuteReader()) {
                        List<T> lst = new List<T>();
                        int rowcounts = 0;
                        if (parseSqlContext.SkipTake) {
                            if (!reader.Read())
                                return Tuple.Create(lst,rowcounts);
                            lst.Add(map(reader));
                            rowcounts = (int)reader[this.rowCountFiledName];
                        }
                        while (reader.Read()) {
                            lst.Add(map(reader));
                        }
                        return Tuple.Create(lst, rowcounts);
                    }
                }
            }
        }

        protected string rowCountFiledName = "_rowsCount";
        protected virtual string Parse(ParseSqlContext context)
        {
            var lstselect = context.SelectNode.Select(x => x.Parse(context)).ToList();
            string selectstr = $"{string.Join(",", lstselect)}";
            string fromstr = $"{context.FromTable.Name} as {context.FromTable.NameNick}";
            var lstjoin = context.JoinNode.Select(x => x.Parse(context)).ToList();
            var joinstr = string.Empty;
            if (lstjoin.Count > 0)
                joinstr = string.Join("\r\n", lstjoin);
            string wherestr = string.Empty;
            if (context.WhereNode != null)
                wherestr = $"where {context.WhereNode.Parse(context)}";
            List<string> lstgroupby = context.GroupbyNode.Select(x => x.Parse(context)).ToList();
            string groupbystr = string.Empty;
            if (lstgroupby.Count > 0)
                groupbystr = $"group by {string.Join(",", lstgroupby)}";
            string havingstr = string.Empty;
            if (context.Having != null)
                havingstr = context.Having.Parse(context);
            var lstorderby = context.OrderbyNode.Select(x => x.Parse(context)).ToList();
            string orderbystr = string.Empty;
            if (lstorderby.Count > 0)
                orderbystr = $"order by {string.Join(",", lstorderby)}";
            
            if (!context.SkipTake)
                return $"select {selectstr} \r\n from {fromstr}\r\n{joinstr}\r\n{wherestr}\r\n{groupbystr}\r\n{havingstr}\r\n{orderbystr}";
            string tmpRowIndex = "_theRowIndex";
            string cmdstr = $"select {selectstr},ROW_NUMBER() OVER({orderbystr}) AS {tmpRowIndex} \r\n from {fromstr}\r\n{joinstr}\r\n{wherestr}\r\n{groupbystr}\r\n{havingstr}";
            string cmdstrskitak = $"with htt AS(\r\n{cmdstr}),\r\n hbb AS(\r\n select COUNT(0) AS {this.rowCountFiledName} from htt)\r\n select htt.*,hbb.* from htt,hbb WHERE htt.{tmpRowIndex} BETWEEN {context.Skip} AND {context.Skip+context.Take-1}";
            return cmdstrskitak;
        }

        public virtual int SaveChange(System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.Unspecified)
        {
            using (var cnn = this.CreateConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    var context = new ParseSqlContext(this.GetDbParameterNamePrefix(), cmd.CreateParameter);
                    cmd.Transaction = cnn.BeginTransaction(isolationLevel);
                    try
                    {
                        int effectrows = 0;
                        this.executeNonQueryDbSetCollection.ForEach(x => effectrows += x.ExecuteNonQuery(cmd, context));
                        cmd.Transaction.Commit();
                        return effectrows;
                    }
                    catch (Exception)
                    {
                        cmd.Transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
