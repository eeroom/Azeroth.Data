using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu.Node
{
    public class SelectNode:IParseSql
    {
        public Column Column { set; get; }
        public SelectNode(Column column)
        {
            this.Column = column;
        }

        internal string nameNick { set; get; }
        internal int index { set; get; }


        public string Parse(ParseSqlContext context)
        {
            if (string.IsNullOrEmpty(this.nameNick))
                return this.Column.Parse(context);
            return $"{this.Column.Parse(context)} as {this.nameNick}";
        }

    
    }
}
