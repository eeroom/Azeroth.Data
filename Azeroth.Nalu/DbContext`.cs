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
    public abstract  class DbContext:IDbContext
    {
        protected string Cnnstr { get; set; }


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

        public DbCud<T> Add<T>(T entity)
        {
            return new DbCud<T>(entity, Cmd.Add);
        }

        public DbCud<T> Add<T>(IEnumerable<T> lst)
        {
            return new DbCud<T>(lst, Cmd.Add);
        }

        public DbCud<T> Edit<T>(T entity)
        {
            return new DbCud<T>(entity, Cmd.Edit);
        }

        public DbCud<T> Edit<T>(IEnumerable<T> lst)
        {
            return new DbCud<T>(lst, Cmd.Edit);
        }

        public DbCud<T> Delete<T>(T entity)
        {
            return new DbCud<T>(entity, Cmd.Del);
        }

        public DbCud<T> Delete<T>(IEnumerable<T> lst)
        {
            return new DbCud<T>(lst, Cmd.Del);
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
                    using (var reader=cmd.ExecuteReader())
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
            throw new NotImplementedException();
        }
    }
}
