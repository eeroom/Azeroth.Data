using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Azeroth.Nalu.Node;

namespace Azeroth.Nalu
{
    public class DbSetDelSimple<T>:Table<T>,IExecuteNonQuery
    {
        WhereNode whereNode { set; get; }
     
        internal DbSetDelSimple()
        {
            
        }

        public DbSetDelSimple<T> Where(Func<DbSetDelSimple<T>,WhereNode> predicate)
        {
            this.whereNode = predicate(this);
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

        int IExecuteNonQuery.ExecuteNonQuery(System.Data.Common.DbCommand cmd, ParseSqlContext context)
        {
            context.DbParameters.Clear();
            string strwhere = string.Empty;
            if (this.whereNode != null)
                strwhere = " where " + this.whereNode.Parse(context);
            cmd.CommandText = $"DELETE FROM {this.Name} {strwhere}";
            cmd.Parameters.Clear();
            cmd.Parameters.AddRange(context.DbParameters.ToArray());
            return cmd.ExecuteNonQuery();
        }
    }
}
