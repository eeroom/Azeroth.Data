using Azeroth.Nalu.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class Column<T,S>:Column,IColumn
    {
        System.Linq.Expressions.Expression<Func<T, S>> exp;

        public Column(Table db, System.Linq.Expressions.Expression<Func<T, S>> exp):base(db,Column.GetName(exp.Body))
        {
            this.exp = exp;
        }

        public Column(Table db, System.Linq.Expressions.Expression<Func<T, S>> exp,ISelectNode mapcolumn)
            : base(db, Column.GetName(exp.Body),mapcolumn)
        {
            this.exp = exp;
        }

        public WhereNode<T, S> In(System.Collections.ICollection value)
        {
            return new WhereNode<T, S>(this, WH.IN,value);
        }

        public WhereNode<T, S> In(params S[] value)
        {
            return new WhereNode<T, S>(this, WH.IN, value);
        }

        public WhereNode<T, S> Range(object min,object max)
        {
            return new WhereNode<T, S>(this, WH.BT, min,max);
        }

        public WhereNode<T, S> BatchEdit(WH opt)
        {
            return new WhereNode<T, S>(this,this.exp,opt);
        }

        public WhereNode<T, S> Null()
        {
            
            return new WhereNode<T, S>(this, WH.NULL,string.Empty);
        }

        public WhereNode<T, S> Exists(Query value)
        {

            return new WhereNode<T, S>(this, WH.Exists, value);
        }

        public WhereNode<T, S> Like(string value)
        {
            return new WhereNode<T, S>(this, WH.LIKE, value);
        }

        public WhereNode<T, S> NoParameter(Func<Column, string> handler)
        {
            this.functionHandler = handler;
            return new WhereNode<T, S>(this, WH.NoParameter, string.Empty);
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

        public static WhereNode<T,S> operator >=(Column<T,S> col,S value)
        {
            return new WhereNode<T, S>(col, WH.GTE,value);
        }

        public static WhereNode<T, S> operator >(Column<T, S> col, S value)
        {
            return new WhereNode<T, S>(col, WH.GT, value);
        }

        public static WhereNode<T, S> operator <=(Column<T, S> col, S value)
        {
            return new WhereNode<T, S>(col, WH.LTE, value);
        }

        public static WhereNode<T, S> operator <(Column<T, S> col, S value)
        {
            return new WhereNode<T, S>(col, WH.LT, value);
        }

        public static WhereNode<T, S> operator ==(Column<T, S> col, S value)
        {
            return new WhereNode<T, S>(col, WH.EQ, value);
        }

        public static WhereNode<T, S> operator !=(Column<T, S> col, S value)
        {
            return new WhereNode<T, S>(col, ~WH.EQ, value);
        }

        public static WhereNode<T, S> operator >=(S value,Column<T, S> col)
        {
            return new WhereNode<T, S>(col, WH.LTE, value);
        }

        public static WhereNode<T, S> operator >( S value,Column<T, S> col)
        {
            return new WhereNode<T, S>(col, WH.LT, value);
        }

        public static WhereNode<T, S> operator <=(S value,Column<T, S> col )
        {
            return new WhereNode<T, S>(col, WH.GTE, value);
        }

        public static WhereNode<T, S> operator <(S value,Column<T, S> col)
        {
            return new WhereNode<T, S>(col, WH.GT, value);
        }

        public static WhereNode<T, S> operator ==(S value,Column<T, S> col)
        {
            return new WhereNode<T, S>(col, WH.EQ, value);
        }

        public static WhereNode<T, S> operator !=(S value, Column<T, S> col)
        {
            return new WhereNode<T, S>(col, ~WH.EQ, value);
        }

        public static JoinOnNode operator !=(Column<T, S> col, IColumn col2)
        {
            throw new ArgumentException("不支持的连接条件");
        }

        public static JoinOnNode operator ==(Column<T, S> col, IColumn col2)
        {
            return new JoinOnNode(col,col2);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public WhereNode<T, S> Equals(S value)
        {
            return new WhereNode<T, S>(this, WH.EQ, value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
