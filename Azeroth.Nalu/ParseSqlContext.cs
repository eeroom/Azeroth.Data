using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class ParseSqlContext
    {
        public ParseSqlContext(string prefix,Func<System.Data.Common.DbParameter> createParameter)
        {
            this.DbParameterNamePrefix = prefix;
            this.DbParameters = new List<System.Data.Common.DbParameter>(8);
            //this.CanCTE = true;
            this.createParameter = createParameter;
            this.JoinNode = new List<Node.JoinNode>();
            this.GroupbyNode = new List<Column>();
            this.OrderbyNode = new List<Node.OrderByNode>();
            this.SelectNode = new List<Node.SelectNode>();
            this.Tables = new List<Table>();
            
        }

        int colIndex = 0;

        int parameterIndex = 0;
        /// <summary>
        /// 参数的符号，msssql-@  oracle-:
        /// </summary>
        public string DbParameterNamePrefix { get;private set; }
        /// <summary>
        /// 本次请求的所有参数
        /// </summary>
        public List<System.Data.Common.DbParameter> DbParameters { get;private set; }

        Func<System.Data.Common.DbParameter> createParameter;
        /// <summary>
        /// 创建DbParameter的方法
        /// </summary>
        public System.Data.Common.DbParameter CreateParameter(string name,object value)
        {
            var p = this.createParameter();
            p.ParameterName = $"{this.DbParameterNamePrefix}{name}{this.NextParameterIndex()}";
            p.Value = value;
            return p;
        }

        /// <summary>
        /// 列别名的序号
        /// </summary>
        /// <returns></returns>
        public int NextColIndex()
        {
            return this.colIndex++;
        }

        int tableIndex;
        public int NextTableIndex()
        {
            return this.tableIndex++;
        }


        /// <summary>
        /// SQL参数的序号
        /// </summary>
        /// <returns></returns>
        public int NextParameterIndex()
        {
            return this.parameterIndex++;
        }

        public List<Node.SelectNode> SelectNode { set; get; }

        public Table FromTable { get; set; }

        public List<Node.JoinNode> JoinNode { get; set; }

        public Node.WhereNode WhereNode { get; set; }

        public Node.WhereNode Having { get; set; }

        public List<Column> GroupbyNode{ get; set; }

        public List<Node.OrderByNode> OrderbyNode { get; set; }

        public List<Table> Tables { set; get; }
    }
}
