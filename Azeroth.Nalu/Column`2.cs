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

        public Column(Container db, System.Linq.Expressions.Expression<Func<T, S>> exp,IComponentSELECT mapcolumn)
            : base(db, Column.GetColumnName(exp.Body),mapcolumn)
        {
            this.exp = exp;
        }

        public ComponentWHERE<T, S> In(System.Collections.ICollection value)
        {
            return new ComponentWHERE<T, S>(this, WH.IN,value);
        }

        public ComponentWHERE<T, S> In(params S[] value)
        {
            return new ComponentWHERE<T, S>(this, WH.IN, value);
        }

        public ComponentWHERE<T, S> Range(object min,object max)
        {
            return new ComponentWHERE<T, S>(this, WH.BT, min,max);
        }

        public ComponentWHERE<T, S> BatchEdit(WH opt)
        {
            return new ComponentWHERE<T, S>(this,this.exp,opt);
        }

        public ComponentWHERE<T, S> Null()
        {
            
            return new ComponentWHERE<T, S>(this, WH.NULL,string.Empty);
        }

        public ComponentWHERE<T, S> Exists(DbSetContainer value)
        {

            return new ComponentWHERE<T, S>(this, WH.Exists, value);
        }

        public ComponentWHERE<T, S> Like(string value)
        {
            return new ComponentWHERE<T, S>(this, WH.LIKE, value);
        }

        public ComponentWHERE<T, S> NoParameter(Func<Column, string> handler)
        {
            this.functionHandler = handler;
            return new ComponentWHERE<T, S>(this, WH.NoParameter, string.Empty);
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

        public static ComponentWHERE<T,S> operator >=(Column<T,S> col,S value)
        {
            return new ComponentWHERE<T, S>(col, WH.GTE,value);
        }

        public static ComponentWHERE<T, S> operator >(Column<T, S> col, S value)
        {
            return new ComponentWHERE<T, S>(col, WH.GT, value);
        }

        public static ComponentWHERE<T, S> operator <=(Column<T, S> col, S value)
        {
            return new ComponentWHERE<T, S>(col, WH.LTE, value);
        }

        public static ComponentWHERE<T, S> operator <(Column<T, S> col, S value)
        {
            return new ComponentWHERE<T, S>(col, WH.LT, value);
        }

        public static ComponentWHERE<T, S> operator ==(Column<T, S> col, S value)
        {
            return new ComponentWHERE<T, S>(col, WH.EQ, value);
        }

        public static ComponentWHERE<T, S> operator !=(Column<T, S> col, S value)
        {
            return new ComponentWHERE<T, S>(col, ~WH.EQ, value);
        }

        public static ComponentWHERE<T, S> operator >=(S value,Column<T, S> col)
        {
            return new ComponentWHERE<T, S>(col, WH.LTE, value);
        }

        public static ComponentWHERE<T, S> operator >( S value,Column<T, S> col)
        {
            return new ComponentWHERE<T, S>(col, WH.LT, value);
        }

        public static ComponentWHERE<T, S> operator <=(S value,Column<T, S> col )
        {
            return new ComponentWHERE<T, S>(col, WH.GTE, value);
        }

        public static ComponentWHERE<T, S> operator <(S value,Column<T, S> col)
        {
            return new ComponentWHERE<T, S>(col, WH.GT, value);
        }

        public static ComponentWHERE<T, S> operator ==(S value,Column<T, S> col)
        {
            return new ComponentWHERE<T, S>(col, WH.EQ, value);
        }

        public static ComponentWHERE<T, S> operator !=(S value, Column<T, S> col)
        {
            return new ComponentWHERE<T, S>(col, ~WH.EQ, value);
        }

        public static ComponentON operator !=(Column<T, S> col, IColumn col2)
        {
            throw new ArgumentException("不支持的连接条件");
        }

        public static ComponentON operator ==(Column<T, S> col, IColumn col2)
        {
            return new ComponentON(col,col2);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public ComponentWHERE<T, S> Equals(S value)
        {
            return new ComponentWHERE<T, S>(this, WH.EQ, value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
