using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// 筛选条件
    /// </summary>
    /// <typeparam name="T">筛选条件所在表对应的class类型</typeparam>
    /// <typeparam name="P">筛选条件针对的列的数据类型--为了解决批量编辑和修改场景下，从实例数据中获取该属性对应的参数值，所以保留这个参数</typeparam>
    public class PredicateNode<T,P> : PredicateNode
    {
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

        /// <summary>
        /// 批量修改和批量删除的的场景下，从要删除的实例中获取参数的值
        /// </summary>
        Func<T, P> getParameterValueFromResolverContext;

        bool qianTao;//是否嵌套查询

        public PredicateNode(Column<T,P> column,  WH opt, object value)
            : base(column)
        {
            this.opt = opt;
            if (this.column.Container.DictMapHandler[this.column.ColumnName].IsMapStringToEnum())
                this.value = value.ToString();
            else
                this.value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="opt"></param>
        /// <param name="value"></param>
        public PredicateNode(Column<T, P> column, WH opt, System.Collections.ICollection value)
            : base(column)
        {
            this.opt = opt;
            this.lstValue = new List<object>();
            if (this.column.Container.DictMapHandler[this.column.ColumnName].IsMapStringToEnum())
            {
                foreach (var tmp in value)
                    this.lstValue.Add(tmp.ToString());
            }
            else
            {
                foreach (var tmp in value)
                    this.lstValue.Add(tmp);
            }
        }

        public PredicateNode(Column<T, P> column, WH opt,object min,object max)
            : base(column)
        {
            this.opt = opt;
            if (this.column.Container.DictMapHandler[this.column.ColumnName].IsMapStringToEnum())
            {
                this.value = min.ToString();
                this.value2 = max.ToString();
            }
            else
            {
                this.value = min;
                this.value2 = max;
            }
        }

        public PredicateNode(IColumn col, Expression<Func<T, P>> exp, WH opt)
            : base(col)
        {
            this.opt = opt;
            this.getParameterValueFromResolverContext = exp.Compile();//针对批量编辑和批量删除的场景
            
        }

        public PredicateNode(Column<T, P> column, WH opt, IQuery handler)
            : base(column)
        {
            
            this.opt = opt;
            this.value = handler;
            this.qianTao = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override string ResolveSQL(ResovleContext context)
        {
            if (this.getParameterValueFromResolverContext != null && context.Tag != null)
                value= this.getParameterValueFromResolverContext((T)context.Tag);
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
            DbParameter parameter = context.CreateParameter();
            parameter.ParameterName = context.Symbol + this.column.ColumnName + context.NextIndex().ToString();//参数名称
            parameter.Value = value;//参数值
            string strwhere = string.Format("{0} {1} {2}", this.column.ResolveSQL(context), this.opt.Totxt(), parameter.ParameterName); //表别名.列1=参数1
            context.Parameters.Add(parameter);
            return strwhere;
        }

        private string ToSQLWithExists(ResovleContext context)
        {
            var tmp= this.value as IQuery;
            string strwhere = string.Format("{0} {1} ({2})", this.column.ResolveSQL(context), this.opt.Totxt(),tmp.GetCommandText(context));
            return strwhere;
        }

        private string ToSQLWithNULL(ResovleContext context)
        {
            string strwhere = string.Format("{0} {1}", this.column.ResolveSQL(context), this.opt.Totxt()); 
            return strwhere;
        }

        private string ToSQLWithNoParameter(ResovleContext context)
        {
            return base.ResolveSQL(context);
        }

        /// <summary>
        /// Between和NOT Between的情况
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string ToSQLWithBetween(ResovleContext context)
        {
            System.Data.Common.DbParameter parameter = context.CreateParameter();
            parameter.ParameterName = context.Symbol + this.column.ColumnName + context.NextIndex().ToString();
            parameter.Value = value;
            context.Parameters.Add(parameter);
            System.Data.Common.DbParameter parameter2 = context.CreateParameter();
            parameter2.ParameterName = context.Symbol + this.column.ColumnName + context.NextIndex().ToString();
            parameter2.Value = value2;
            context.Parameters.Add(parameter2);
            return string.Format("{0} {3} {1} AND {2}", this.column.ResolveSQL(context), parameter.ParameterName, parameter2.ParameterName, this.opt.Totxt());
        }

        /// <summary>
        /// IN和NOT IN的情况
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string ToSQLWithIN(ResovleContext context)
        {
            if (qianTao)
            {//这里是IN的嵌套查询
                var query = value as IQuery;
                return string.Format("{0} {1} ({2})", this.column.ResolveSQL(context), this.opt.Totxt(), query.GetCommandText(context));//where里面的子查询
            }
            List<string> lstName = new List<string>();
            System.Data.Common.DbParameter parameter;
            foreach (object val in lstValue)
            {//处理参数
                parameter = context.CreateParameter();
                parameter.ParameterName = context.Symbol + this.column.ColumnName + context.NextIndex().ToString();
                lstName.Add(parameter.ParameterName);
                parameter.Value = val;
                context.Parameters.Add(parameter);
            }
            return string.Format("{0} {1} ({2})", this.column.ResolveSQL(context), this.opt.Totxt(), string.Join(",", lstName));
        }

        public PredicateNode<T, P> SetPlaceholder(bool placeholder)
        {
            this.Placeholder = placeholder;
            return this;
        }

        public static PredicateNode<T,P> operator!(PredicateNode<T,P> node)
        {
            node.opt = ~node.opt;
            return node;
        }
    }
}
