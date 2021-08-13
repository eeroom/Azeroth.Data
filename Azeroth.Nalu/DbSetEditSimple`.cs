using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Azeroth.Nalu.Node;

namespace Azeroth.Nalu
{
    public class DbSetEditSimple<T>:Table<T>,IExecuteNonQuery
    {
        WhereNode whereNode { set; get; }
      
        Dictionary<string,object> dictSetColumn { set; get; }
        internal DbSetEditSimple()
        {
            this.dictSetColumn = new Dictionary<string, object>();
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

        public DbSetEditSimple<T> SetColumn<S>(Expression<Func<T,S>> colexp,S value)
        {
            var colmem = colexp.Body as MemberExpression;
            if (colmem == null)
                throw new ArgumentException("不支持的表达式");
            this.dictSetColumn.Add(colmem.Member.Name, value);
            return this;
        }

        int IExecuteNonQuery.ExecuteNonQuery(System.Data.Common.DbCommand cmd, ParseSqlContext context)
        {
            var lstParameter= this.dictSetColumn.Select(x => context.CreateParameter(x.Key, x.Value, false)).ToArray();
            var lstsetcol = this.dictSetColumn.Zip(lstParameter, (x, y) => $"{x.Key}={y.ParameterName}").ToList();
            var strset = string.Join(",", lstsetcol);
            context.DbParameters.Clear();
            string strwhere = string.Empty;
            if (this.whereNode != null)
                strwhere = " where " + this.whereNode.Parse(context);
            cmd.CommandText = $"update {this.Name} set {strset} {strwhere}";
            cmd.Parameters.Clear();
            cmd.Parameters.AddRange(lstParameter);
            cmd.Parameters.AddRange(context.DbParameters.ToArray());
            return cmd.ExecuteNonQuery();
        }
    }
}
