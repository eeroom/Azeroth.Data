using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public class Column:IColumn
    {

        public Column(Container contianer, string columnName)
        {
            this.container = contianer;
            this.columnName = columnName;
        }

        protected IComponentSELECT mapColumn;

        public Column(Container contianer, string columnName, IComponentSELECT mapColumn)
        {
            this.container = contianer;
            this.columnName = columnName;
            this.mapColumn = mapColumn;
        }

        protected Function functionCode;

        protected Func<Column, string> functionHandler;
        protected IContainer container;

        /// <summary>
        /// 名称
        /// </summary>
        protected string columnName;

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.container.NameNick))
                return this.columnName;
            if (mapColumn != null)
                return this.container.NameNick + "." + mapColumn.ColumnNameNick;
            return this.container.NameNick + "." + this.columnName;
        }

        IContainer IColumn.Container
        {
            get { return this.container; }
        }

        string IColumn.ColumnName
        {
            get { return this.columnName; }
        }

        public string ToSQL(ResovleContext context)
        {
            if (functionHandler != null)
                return this.functionHandler(this);
            string rst = string.Empty;
            switch (this.functionCode)
            {
                case Azeroth.Nalu.Function.NONE:
                    rst = this.ToString();
                    break;
                case Azeroth.Nalu.Function.Sum:
                    rst = string.Format("SUM({0})", this.ToString());
                    break;
                case Azeroth.Nalu.Function.Avg:
                    rst = string.Format("AVG({0})", this.ToString());
                    break;
                case Azeroth.Nalu.Function.Count:
                    rst = string.Format("COUNT({0})", this.ToString());
                    break;
                case Azeroth.Nalu.Function.Max:
                    rst = string.Format("MAX({0})", this.ToString());
                    break;
                case Azeroth.Nalu.Function.Min:
                    rst = string.Format("MIN({0})", this.ToString());
                    break;
                case Azeroth.Nalu.Function.Lower:
                    rst = string.Format("LOWER({0})", this.ToString());
                    break;
                case Azeroth.Nalu.Function.Upper:
                    rst = string.Format("UPPER({0})", this.ToString());
                    break;
                default:
                    throw new ArgumentException("未识别的函数");
            }
            return rst;
        }

        public Column Function(Function value)
        {
            this.functionCode = value;
            return this;
        }

        public Column Function(Func<Column,string> value)
        {
            this.functionHandler = value;
            return this;
        }

        Function IColumn.FunctionCode
        {
            get { return this.functionCode; }
        }

        Func<Column, string> IColumn.FunctionHandler
        {
            get { return this.functionHandler; }
        }

        /// <summary>
        /// 获取表达式对应的列的名称
        /// </summary>
        /// <param name="expBody"></param>
        /// <returns></returns>
        public static string GetColumnName(Expression expBody)
        {
            MemberExpression memExp = expBody as MemberExpression;
            if (memExp != null)
                return memExp.Member.Name;
            UnaryExpression unaExp = expBody as UnaryExpression;
            if (unaExp != null)
                return GetColumnName(unaExp.Operand);
            throw new ArgumentException("必须使用返回单个属性值的表达式，例如：x=>x.Name");
        }

        /// <summary>
        /// 获取表达式对应的列的名称
        /// </summary>
        /// <param name="expBody"></param>
        /// <returns></returns>
        public static List<string> GetColumnNames(Expression expBody)
        {
            NewExpression newExp = expBody as NewExpression;
            if (newExp != null)
                return newExp.Members.Select(x => x.Name).ToList();
            string tmp = GetColumnName(expBody);
            return new List<string>() { tmp };
        }
    }
}
