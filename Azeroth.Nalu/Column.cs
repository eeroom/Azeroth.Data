﻿using Azeroth.Nalu.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public class Column:IColumn
    {

        public Column(Table table, string columnName)
        {
            this.table = table;
            this.columnName = columnName;
        }

        protected ISelectNode mapColumn;

        public Column(Table table, string columnName, ISelectNode mapColumn)
        {
            this.table = table;
            this.columnName = columnName;
            this.mapColumn = mapColumn;
        }

        protected Function functionCode;

        protected Func<Column, string> functionHandler;
        protected ITable table;

        /// <summary>
        /// 名称
        /// </summary>
        protected string columnName;

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.table.NameNick))
                return this.columnName;
            if (mapColumn != null)
                return this.table.NameNick + "." + mapColumn.ColumnNameNick;
            return this.table.NameNick + "." + this.columnName;
        }

        ITable IColumn.Table
        {
            get { return this.table; }
        }

        string IColumn.ColumnName
        {
            get { return this.columnName; }
        }

        public string ToSQL(ResolveContext context)
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
        public static string GetName(Expression expBody)
        {
            MemberExpression memExp = expBody as MemberExpression;
            if (memExp != null)
                return memExp.Member.Name;
            UnaryExpression unaExp = expBody as UnaryExpression;
            if (unaExp != null)
                return GetName(unaExp.Operand);
            throw new ArgumentException("不支持的表达式，正确示例：x=>x.Name");
        }

        /// <summary>
        /// 获取表达式对应的列的名称
        /// </summary>
        /// <param name="expBody"></param>
        /// <returns></returns>
        public static List<string> GetNameCollection(Expression expBody)
        {
            NewExpression newExp = expBody as NewExpression;
            if (newExp != null)
                return newExp.Members.Select(x => x.Name).ToList();
            string tmp = GetName(expBody);
            return new List<string>() { tmp };
        }
    }
}
