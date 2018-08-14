using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class Column<T,S>:Column,IColumn
    {
        System.Linq.Expressions.Expression<Func<T, S>> exp;

        public Column(Container db, System.Linq.Expressions.Expression<Func<T, S>> exp):base(db,Column.GetColumnName(exp.Body))
        {
            this.exp = exp;
        }

        public Column(Container db, System.Linq.Expressions.Expression<Func<T, S>> exp,INodeSelect mapcolumn)
            : base(db, Column.GetColumnName(exp.Body),mapcolumn)
        {
            this.exp = exp;
        }

        public NodeWhere<T, S> In(System.Collections.ICollection value)
        {
            return new NodeWhere<T, S>(this, WH.IN,value);
        }

        public NodeWhere<T, S> In(params S[] value)
        {
            return new NodeWhere<T, S>(this, WH.IN, value);
        }

        public NodeWhere<T, S> Range(object min,object max)
        {
            return new NodeWhere<T, S>(this, WH.BT, min,max);
        }

        public NodeWhere<T, S> BatchEdit(WH opt)
        {
            return new NodeWhere<T, S>(this,this.exp,opt);
        }

        public NodeWhere<T, S> Null()
        {
            
            return new NodeWhere<T, S>(this, WH.NULL,string.Empty);
        }

        public NodeWhere<T, S> Exists(Query value)
        {

            return new NodeWhere<T, S>(this, WH.Exists, value);
        }

        public NodeWhere<T, S> Like(string value)
        {
            return new NodeWhere<T, S>(this, WH.LIKE, value);
        }

        public NodeWhere<T, S> NoParameter(Func<Column, string> handler)
        {
            this.functionHandler = handler;
            return new NodeWhere<T, S>(this, WH.NoParameter, string.Empty);
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

        public static NodeWhere<T,S> operator >=(Column<T,S> col,S value)
        {
            return new NodeWhere<T, S>(col, WH.GTE,value);
        }

        public static NodeWhere<T, S> operator >(Column<T, S> col, S value)
        {
            return new NodeWhere<T, S>(col, WH.GT, value);
        }

        public static NodeWhere<T, S> operator <=(Column<T, S> col, S value)
        {
            return new NodeWhere<T, S>(col, WH.LTE, value);
        }

        public static NodeWhere<T, S> operator <(Column<T, S> col, S value)
        {
            return new NodeWhere<T, S>(col, WH.LT, value);
        }

        public static NodeWhere<T, S> operator ==(Column<T, S> col, S value)
        {
            return new NodeWhere<T, S>(col, WH.EQ, value);
        }

        public static NodeWhere<T, S> operator !=(Column<T, S> col, S value)
        {
            return new NodeWhere<T, S>(col, ~WH.EQ, value);
        }

        public static NodeWhere<T, S> operator >=(S value,Column<T, S> col)
        {
            return new NodeWhere<T, S>(col, WH.LTE, value);
        }

        public static NodeWhere<T, S> operator >( S value,Column<T, S> col)
        {
            return new NodeWhere<T, S>(col, WH.LT, value);
        }

        public static NodeWhere<T, S> operator <=(S value,Column<T, S> col )
        {
            return new NodeWhere<T, S>(col, WH.GTE, value);
        }

        public static NodeWhere<T, S> operator <(S value,Column<T, S> col)
        {
            return new NodeWhere<T, S>(col, WH.GT, value);
        }

        public static NodeWhere<T, S> operator ==(S value,Column<T, S> col)
        {
            return new NodeWhere<T, S>(col, WH.EQ, value);
        }

        public static NodeWhere<T, S> operator !=(S value, Column<T, S> col)
        {
            return new NodeWhere<T, S>(col, ~WH.EQ, value);
        }

        public static NodeON operator !=(Column<T, S> col, IColumn col2)
        {
            throw new ArgumentException("不支持的连接条件");
        }

        public static NodeON operator ==(Column<T, S> col, IColumn col2)
        {
            return new NodeON(col,col2);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public NodeWhere<T, S> Equals(S value)
        {
            return new NodeWhere<T, S>(this, WH.EQ, value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
