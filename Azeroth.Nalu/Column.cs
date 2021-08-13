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
        internal string name {private set; get; }

        public Column(Table table, string columnName)
        {
            this.table = table;
            this.name = columnName;
        }

        public virtual string Parse(ParseSqlContext context)
        {
            var str = $"{((ITable)this.table).NameNick}.{this.name}";
            return str;
        }
    }
}
