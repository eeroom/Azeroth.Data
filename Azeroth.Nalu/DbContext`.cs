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
    /// <typeparam name="H"></typeparam>
    public abstract  class DbContext:IDbContext
    {
        public string Cnnstr { get; set; }

        public virtual int SaveChange(params ICud[] dbsets)
        {
            int rst = 0;
            using (DbConnection cnn = this.GetDbProviderFactory().CreateConnection())
            {
                cnn.ConnectionString = this.Cnnstr;
                cnn.Open();
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.Transaction = cnn.BeginTransaction();
                    var context = this.GetResolveContext();
                    foreach (ICud dbset in dbsets)
                    {
                        rst += dbset.Execute(cmd, context);
                    }
                    cmd.Transaction.Commit();
                }
            }
            return rst;
        }

        public virtual DbCud<T> Cud<T>() where T:class
        {
            return new DbCud<T>();
        }

        public virtual Query Query() 
        {
            return new Query(this);
        }

        protected virtual ResolveContext GetResolveContext() 
        {
            return new ResolveContext("@",()=>new System.Data.SqlClient.SqlParameter());
        }


        public DbSet<T> Set<T>(IQuery container)
        {
            return new DbSet<T>(container);

            //DbSet<B> tmp = new DbSet<B>(this);
            //this.lstDbSet.Add(tmp);
            //return tmp;
        }

        public abstract DbProviderFactory GetDbProviderFactory();

        ResolveContext IDbContext.GetResolveContext()
        {
            return this.GetResolveContext();
        }
    }
}
