using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu.Node
{
    public class JoinNode:IParseSql
    {
        Table rightTable;
        WhereNode jw;
        JOIN joption;
        public JoinNode(Table rightTable,WhereNode jw,JOIN joption)
        {
            this.rightTable = rightTable;
            this.jw = jw;
            this.joption = joption;
        }


     

        public string Parse(ParseSqlContext context)
        {
            var str = $"{joption.ToSQL()} {this.rightTable.Name} AS {this.rightTable.NameNick} ON {this.jw.Parse(context)}";
            return str;    
        }
    }
}
