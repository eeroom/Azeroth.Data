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
        public string Cnnstr { get; set; }

        public virtual Result SaveChange(params ICud[] dbsets)
        {
            string msg;
            foreach (ICud db in dbsets)
            {
                if (!db.Validate(out msg))
                    return new Result(true, msg);
            }
            int rst = 0;
            using (H cnn = new H())
            {
                
                cnn.ConnectionString = this.Cnnstr;
                cnn.Open();
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.Transaction = cnn.BeginTransaction();
                    var context = this.GetResolvContext();
                    foreach (ICud dbset in dbsets)
                    {
                        rst += dbset.Execute(cmd, context);
                    }
                    cmd.Transaction.Commit();
                }
            }
            return new Result(rst);
        }

        public virtual DbCud<T> NoQuery<T>()
        {
            return new DbCud<T>();
        }

        public virtual DbSetContainer Query()
        {
            return new DbSetContainer(this);
        }

        protected abstract ResovleContext GetResolvContext();

        List<T> IDbContext.ExecuteQuery<T>(IDbSetContainer master, Func<object[], T> transfer)
        {
            return master.Execute<H, T>(transfer,this.Cnnstr);
        }

        ResovleContext IDbContext.GetResolvContext()
        {
            return this.GetResolvContext();
        }
    }
}
