using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Azeroth.Nalu.Node;

namespace Azeroth.Nalu
{
    public class DbSetDel<T>:Table<T>,ICud
    {
        IEnumerable<T> lstEntity { set; get; }

        Func<DbSetDel<T>, T, WhereNode> whereNodeHandler { set; get; }
     
        internal DbSetDel(IEnumerable<T> lst)
        {
            lstEntity = lst;
        }

        public DbSetDel<T> Where(Func<DbSetDel<T>,T,WhereNode> predicate)
        {
            this.whereNodeHandler = predicate;
            return this;
        }

        public Column<T, S> Col<S>(Expression<Func<T, S>> exp)
        {
            var colmem = exp.Body as MemberExpression;
            if (colmem == null)
                throw new ArgumentException("不支持的表达式");
            var col = new Column<T, S>(this, colmem.Member.Name);
            return col;
        }

        int ICud.Execute(System.Data.Common.DbCommand cmd, ParseSqlContext context)
        {
            if (this.whereNodeHandler == null)
                throw new ArgumentException("必须指定where条件");
            int rst = 0;
            foreach (var value in lstEntity)
            {
                context.DbParameters.Clear();
                string strwhere = this.whereNodeHandler(this, value).Parse(context);
                cmd.CommandText =$"DELETE FROM {this.Name} where {strwhere}";
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(context.DbParameters.ToArray());
                rst += cmd.ExecuteNonQuery();
            }
            return rst;
        }
    }
}
