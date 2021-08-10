using Azeroth.Nalu.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public class Column:IParseSql
    {
        protected Table table { set; get; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { protected set; get; }

        public Column(Table table, string columnName)
        {
            this.table = table;
            this.Name = columnName;
        }

        public virtual string Parse(ParseSqlContext context)
        {
            var str = $"{this.table.NameNick}.{this.Name}";
            return str;
        }
    }
}
