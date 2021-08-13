using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu.Node
{
    /// <summary>
    /// 筛选条件
    /// </summary>
    /// <typeparam name="T">筛选条件所在表对应的class类型</typeparam>
    /// <typeparam name="P">筛选条件针对的列的数据类型--为了解决批量编辑和修改场景下，从实例数据中获取该属性对应的参数值，所以保留这个参数</typeparam>
    public class WhereNode<T,P> : WhereNode
    {
        Column<T, P> column;
        /// <summary>
        /// 筛选条件的参数值列表
        /// </summary>
        object value;
        object value2;
        List<object> lstValue;
        /// <summary>
        /// 关系运算符
        /// </summary>
        WH opt;



        public WhereNode(Column<T,P> column,  WH opt, object value)
        {
            this.opt = opt;
            this.value = value;
            this.column = column;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="opt"></param>
        /// <param name="value"></param>
        public WhereNode(Column<T, P> column, WH opt, System.Collections.ICollection value)
        {
            this.column = column;
            this.opt = opt;
            this.lstValue = new List<object>();
            foreach (var tmp in value)
            {
                this.lstValue.Add(tmp);
            }     
        }

        public WhereNode(Column<T, P> column, WH opt,object min,object max)
        {
            this.column = column;
            this.opt = opt;
            this.value = min;
            this.value2 = max;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override string Parse(ParseSqlContext context)
        {
            switch (this.opt)
            {
                case WH.IN:
                    return this.ToSQLWithIN(context);
                case ~WH.IN:
                    return this.ToSQLWithIN(context);
                case WH.BT:
                    return this.ToSQLWithBetween(context);
                case ~WH.BT:
                    return this.ToSQLWithBetween(context);
                case WH.NoParameter:
                    return this.ToSQLWithNoParameter(context);
                case WH.NULL:
                    return this.ToSQLWithNULL(context);
                case ~WH.NULL:
                    return this.ToSQLWithNULL(context);
                case WH.Exists:
                    return this.ToSQLWithExists(context);
                case ~WH.Exists:
                    return this.ToSQLWithExists(context);
                default:
                    break;
            }
            DbParameter parameter = context.CreateParameter(this.column.name, value);
            string strwhere = string.Format("{0} {1} {2}", this.column.Parse(context), this.opt.ToSQL(), parameter.ParameterName); //表别名.列1=参数1
            context.DbParameters.Add(parameter);
            return strwhere;
        }

        private string ToSQLWithNoParameter(ParseSqlContext context)
        {
            throw new NotImplementedException();
        }

        private string ToSQLWithExists(ParseSqlContext context)
        {
            var tmp= this.value as IParseSql;
            string strwhere = string.Format("{0} {1} ({2})", this.column.Parse(context), this.opt.ToSQL(),tmp.Parse(context));
            return strwhere;
        }

        private string ToSQLWithNULL(ParseSqlContext context)
        {
            string strwhere = string.Format("{0} {1}", this.column.Parse(context), this.opt.ToSQL()); 
            return strwhere;
        }

        /// <summary>
        /// Between和NOT Between的情况
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string ToSQLWithBetween(ParseSqlContext context)
        {
            System.Data.Common.DbParameter parameter = context.CreateParameter(this.column.name, value);
            context.DbParameters.Add(parameter);
            System.Data.Common.DbParameter parameter2 = context.CreateParameter(this.column.name, value2);
            context.DbParameters.Add(parameter2);
            return string.Format("{0} {3} {1} AND {2}", this.column.Parse(context), parameter.ParameterName, parameter2.ParameterName, this.opt.ToSQL());
        }

        /// <summary>
        /// IN和NOT IN的情况
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string ToSQLWithIN(ParseSqlContext context)
        {
            var lstParameter= lstValue.Select(x => context.CreateParameter(this.column.name, x)).ToList();
            context.DbParameters.AddRange(lstParameter);
            var lstParameterName = lstParameter.Select(x => x.ParameterName).ToList();
            return string.Format("{0} {1} ({2})", this.column.Parse(context), this.opt.ToSQL(), string.Join(",", lstParameterName));
        }



        public static WhereNode<T,P> operator!(WhereNode<T,P> node)
        {
            node.opt = ~node.opt;
            return node;
        }
    }
}
