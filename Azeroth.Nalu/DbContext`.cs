using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// 数据库上下文，对应于连接的数据库
    /// </summary>
    /// <typeparam name="H"></typeparam>
    public abstract  class DbContext<H>:IDbContext where H : System.Data.Common.DbConnection, new()
    {
        protected string Cnnstr { get; set; }

        public virtual int SaveChange(params ICud[] dbsets)
        {
            int rst = 0;
            using (H cnn = new H())
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

        public virtual Container CreateContainer() 
        {
            return new Container(this);
        }

        ResolveContext IDbContext.GetResolveContext()
        {
            return this.GetResolveContext();
        }

        protected virtual ResolveContext GetResolveContext() 
        {
            return new ResolveContext("@",()=>new System.Data.SqlClient.SqlParameter());
        }

        List<T> IDbContext.ToList<T>(IContainer container, Func<object[], T> transfer)
        {
            return container.ToList<H, T>(transfer,this.Cnnstr);
        }




        public DbSet<T> Set<T>(IContainer container)
        {
            return new DbSet<T>(container);

            //DbSet<B> tmp = new DbSet<B>(this);
            //this.lstDbSet.Add(tmp);
            //return tmp;
        }
    }
}
