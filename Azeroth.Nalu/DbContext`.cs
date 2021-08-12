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

        protected List<ICud> lstCud = new List<ICud>();
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
            this.lstCud.Add(dbset);
            return dbset;
        }

        public DbSetAdd<T> Add<T>(IEnumerable<T> lst)
        {
            var dbset = new DbSetAdd<T>(lst);
            this.lstCud.Add(dbset);
            return dbset;
        }

        public DbSetEdit<T> Edit<T>(T entity)
        {
            var dbset = new DbSetEdit<T>(new List<T>() { entity });
            this.lstCud.Add(dbset);
            return dbset;
        }

        public DbSetEdit<T> Edit<T>(IEnumerable<T> lst)
        {
            var dbset = new DbSetEdit<T>(lst);
            this.lstCud.Add(dbset);
            return dbset;
        }

        public DbSetDel<T> Delete<T>(T entity)
        {
            var dbset = new DbSetDel<T>(new List<T>() { entity });
            this.lstCud.Add(dbset);
            return dbset;
        }

        public DbSetDel<T> Delete<T>(IEnumerable<T> lst)
        {
            var dbset = new DbSetDel<T>(lst);
            this.lstCud.Add(dbset);
            return dbset;
        }

        List<T> IDbContext.ToList<T>(Func<DbDataReader, T> map, Action<ParseSqlContext, bool> initParseSqlContext)
        {
            using (var cnn = this.CreateConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var parseSqlContext = new ParseSqlContext(this.GetDbParameterNamePrefix(), cmd.CreateParameter);
                    initParseSqlContext(parseSqlContext, true);
                    cmd.CommandText = this.Parse(parseSqlContext);
                    cnn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        List<T> lst = new List<T>();
                        while (reader.Read())
                        {
                            lst.Add(map(reader));
                        }
                        return lst;
                    }
                }
            }
        }

        protected virtual string Parse(ParseSqlContext context)
        {
            var lstselect = context.SelectNode.Select(x => x.Parse(context)).ToList();
            string selectstr = $"select {string.Join(",", lstselect)}";
            string fromstr = $"from {context.FromTable.Name} as {context.FromTable.NameNick}";
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
            string cmdstr = $"{selectstr} \r\n{fromstr}\r\n{joinstr}\r\n{wherestr}\r\n{groupbystr}\r\n{havingstr}\r\n{orderbystr}";
            return cmdstr;

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
                        this.lstCud.ForEach(x => effectrows += x.Execute(cmd, context));
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
