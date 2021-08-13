using Azeroth.Nalu.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class Column<T,S>:Column
    {


        public Column(Table db, string colName):base(db,colName)
        {
           
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

        public WhereNode<T, S> Null()
        {
            return new WhereNode<T, S>(this, WH.NULL,string.Empty);
        }


        public WhereNode<T, S> Like(string value)
        {
            return new WhereNode<T, S>(this, WH.LIKE, value);
        }



        public Column<T, S> Max()
        {
            return new ColumnByFunction<T, S>(this.table, this.name, Nalu.Function.Max);
        }

        public Column<T, S> Min()
        {
            return new ColumnByFunction<T, S>(this.table, this.name, Nalu.Function.Min);
        }

        public Column<T, S> Sum()
        {
            return new ColumnByFunction<T, S>(this.table, this.name, Nalu.Function.Sum);
        }

        public Column<T, S> Avg()
        {
            return new ColumnByFunction<T, S>(this.table, this.name, Nalu.Function.Avg);
        }

        public Column<T,S> UserFunction(Func<Column<T, S>, ParseSqlContext, string> handler)
        {
            return new ColumnByUserFunction<T,S>(this.table, this.name, handler);
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

        public static WhereJoinOnNode operator !=(Column<T, S> col, Column col2)
        {
            throw new ArgumentException("不支持的连接条件");
        }

        public static WhereJoinOnNode operator ==(Column<T, S> col, Column col2)
        {
            return new WhereJoinOnNode(col,col2);
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
