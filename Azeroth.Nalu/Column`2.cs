﻿using Azeroth.Nalu.Node;
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
            return new WhereNode<T, S>(this, WhereOpt.IN,value);
        }

        public WhereNode<T, S> In(params S[] value)
        {
            return new WhereNode<T, S>(this, WhereOpt.IN, value);
        }

        public WhereNode<T, S> Range(object min,object max)
        {
            return new WhereNode<T, S>(this, WhereOpt.BT, min,max);
        }

        public WhereNode<T, S> Null()
        {
            return new WhereNode<T, S>(this, WhereOpt.NULL,string.Empty);
        }


        public WhereNode<T, S> Like(string value)
        {
            return new WhereNode<T, S>(this, WhereOpt.LIKE, value);
        }



        public Column<T, S> Max()
        {
            return new ColumnByFunction<T, S>(this.table, this.name, Nalu.SqlFunction.Max);
        }

        public Column<T, S> Min()
        {
            return new ColumnByFunction<T, S>(this.table, this.name, Nalu.SqlFunction.Min);
        }

        public Column<T, S> Sum()
        {
            return new ColumnByFunction<T, S>(this.table, this.name, Nalu.SqlFunction.Sum);
        }

        public Column<T, S> Avg()
        {
            return new ColumnByFunction<T, S>(this.table, this.name, Nalu.SqlFunction.Avg);
        }

        public Column<T,S> UserFunction(Func<Column<T, S>, ParseSqlContext, string> handler)
        {
            return new ColumnByUserFunction<T,S>(this.table, this.name, handler);
        }


        public static WhereNode<T,S> operator >=(Column<T,S> col,S value)
        {
            return new WhereNode<T, S>(col, WhereOpt.GTE,value);
        }

        public static WhereNode<T, S> operator >(Column<T, S> col, S value)
        {
            return new WhereNode<T, S>(col, WhereOpt.GT, value);
        }

        public static WhereNode<T, S> operator <=(Column<T, S> col, S value)
        {
            return new WhereNode<T, S>(col, WhereOpt.LTE, value);
        }

        public static WhereNode<T, S> operator <(Column<T, S> col, S value)
        {
            return new WhereNode<T, S>(col, WhereOpt.LT, value);
        }

        public static WhereNode<T, S> operator ==(Column<T, S> col, S value)
        {
            return new WhereNode<T, S>(col, WhereOpt.EQ, value);
        }

        public static WhereNode<T, S> operator !=(Column<T, S> col, S value)
        {
            return new WhereNode<T, S>(col, ~WhereOpt.EQ, value);
        }

        public static WhereNode<T, S> operator >=(S value,Column<T, S> col)
        {
            return new WhereNode<T, S>(col, WhereOpt.LTE, value);
        }

        public static WhereNode<T, S> operator >( S value,Column<T, S> col)
        {
            return new WhereNode<T, S>(col, WhereOpt.LT, value);
        }

        public static WhereNode<T, S> operator <=(S value,Column<T, S> col )
        {
            return new WhereNode<T, S>(col, WhereOpt.GTE, value);
        }

        public static WhereNode<T, S> operator <(S value,Column<T, S> col)
        {
            return new WhereNode<T, S>(col, WhereOpt.GT, value);
        }

        public static WhereNode<T, S> operator ==(S value,Column<T, S> col)
        {
            return new WhereNode<T, S>(col, WhereOpt.EQ, value);
        }

        public static WhereNode<T, S> operator !=(S value, Column<T, S> col)
        {
            return new WhereNode<T, S>(col, ~WhereOpt.EQ, value);
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
            return new WhereNode<T, S>(this, WhereOpt.EQ, value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
