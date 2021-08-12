using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Azeroth.Nalu.Node;

namespace Azeroth.Nalu
{
    public class DbSetEditSimple<T>:Table<T>,ICud
    {
        WhereNode whereNode { set; get; }
      
        List<WhereNode> lstSetNode { set; get; }
        internal DbSetEditSimple()
        {
          
        }

        public DbSetEditSimple<T> Where(Func<DbSetEditSimple<T>,WhereNode> predicate)
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

        public DbSetEditSimple<T> SetColumn<S>(Func<DbSetEditSimple<T>, WhereNode> setColValue)
        {
            this.lstSetNode.Add(setColValue(this));
            return this;
        }

        int ICud.Execute(System.Data.Common.DbCommand cmd, ParseSqlContext context)
        {
            var lstsetcol = this.lstSetNode.Select(x => x.Parse(context)).ToList();
            var strset = string.Join(",", lstsetcol);

            context.DbParameters.Clear();
            string strwhere = string.Empty;
            if (this.whereNode != null)
                strwhere = " where " + this.whereNode.Parse(context);
            cmd.CommandText = $"update {this.Name} set {strset} where {strwhere}";
            cmd.Parameters.Clear();
            cmd.Parameters.AddRange(context.DbParameters.ToArray());
            return cmd.ExecuteNonQuery();
        }
    }
}
