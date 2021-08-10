using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu.Node
{
    public class SelectNode:IParseSql
    {
        internal Column column { set; get; }
        public SelectNode(Column column)
        {
            this.column = column;
        }

        string columNameNick;
        public int Index { set; get; }


        public string Parse(ParseSqlContext context)
        {
            return this.column.ToString();
        }

    
    }
}
