using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class Column<T,S>:Column,IColumn
    {
        System.Linq.Expressions.Expression<Func<T, S>> exp;

        public Column(DbSet db, System.Linq.Expressions.Expression<Func<T, S>> exp):base(db,Column.GetColumnName(exp.Body))
        {
            this.exp = exp;
        }

        public Column(DbSet db, System.Linq.Expressions.Expression<Func<T, S>> exp,ISelectNode mapcolumn)
            : base(db, Column.GetColumnName(exp.Body),mapcolumn)
        {
            this.exp = exp;
        }

        public PredicateNode<T, S> Contains(System.Collections.ICollection value)
        {
            return new PredicateNode<T, S>(this, WH.IN,value);
        }

        public PredicateNode<T, S> Contains(params S[] value)
        {
            return new PredicateNode<T, S>(this, WH.IN, value);
        }

        public PredicateNode<T, S> Range(object min,object max)
        {
            return new PredicateNode<T, S>(this, WH.IN, min,max);
        }

        public PredicateNode<T, S> Batch(WH opt)
        {
            return new PredicateNode<T, S>(this,this.exp,opt);
        }

        public PredicateNode<T, S> Null()
        {
            
            return new PredicateNode<T, S>(this, WH.NULL,string.Empty);
        }

        public PredicateNode<T, S> Exists(Query value)
        {

            return new PredicateNode<T, S>(this, WH.Exists, value);
        }

        public PredicateNode<T, S> Like(string value)
        {
            return new PredicateNode<T, S>(this, WH.LIKE, value);
        }

        public PredicateNode<T, S> NoParameter(Func<Column, string> handler)
        {
            this.functionHandler = handler;
            return new PredicateNode<T, S>(this, WH.NoParameter, string.Empty);
        }

        public new Column<T, S> Function(Function value)
        {
            this.functionCode = value;
            return this;
        }

        public new  Column<T, S> Function(Func<Column, string> handler)
        {
            this.functionHandler = handler;
            return this;
        }

        public static PredicateNode<T,S> operator >=(Column<T,S> col,S value)
        {
            return new PredicateNode<T, S>(col, WH.GTE,value);
        }

        public static PredicateNode<T, S> operator >(Column<T, S> col, S value)
        {
            return new PredicateNode<T, S>(col, WH.GT, value);
        }

        public static PredicateNode<T, S> operator <=(Column<T, S> col, S value)
        {
            return new PredicateNode<T, S>(col, WH.LTE, value);
        }

        public static PredicateNode<T, S> operator <(Column<T, S> col, S value)
        {
            return new PredicateNode<T, S>(col, WH.LT, value);
        }

        public static PredicateNode<T, S> operator ==(Column<T, S> col, S value)
        {
            return new PredicateNode<T, S>(col, WH.EQ, value);
        }

        public static PredicateNode<T, S> operator !=(Column<T, S> col, S value)
        {
            return new PredicateNode<T, S>(col, ~WH.EQ, value);
        }

        public static PredicateNode<T, S> operator >=(S value,Column<T, S> col)
        {
            return new PredicateNode<T, S>(col, WH.LTE, value);
        }

        public static PredicateNode<T, S> operator >( S value,Column<T, S> col)
        {
            return new PredicateNode<T, S>(col, WH.LT, value);
        }

        public static PredicateNode<T, S> operator <=(S value,Column<T, S> col )
        {
            return new PredicateNode<T, S>(col, WH.GTE, value);
        }

        public static PredicateNode<T, S> operator <(S value,Column<T, S> col)
        {
            return new PredicateNode<T, S>(col, WH.GT, value);
        }

        public static PredicateNode<T, S> operator ==(S value,Column<T, S> col)
        {
            return new PredicateNode<T, S>(col, WH.EQ, value);
        }

        public static PredicateNode<T, S> operator !=(S value, Column<T, S> col)
        {
            return new PredicateNode<T, S>(col, ~WH.EQ, value);
        }

        public static PredicateNode2Join operator !=(Column<T, S> col, IColumn col2)
        {
            throw new ArgumentException("不支持的连接条件");
        }

        public static PredicateNode2Join operator ==(Column<T, S> col, IColumn col2)
        {
            return new PredicateNode2Join(col,col2);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public PredicateNode<T, S> Equals(S value)
        {
            return new PredicateNode<T, S>(this, WH.EQ, value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
