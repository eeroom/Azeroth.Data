﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu.Node
{
    public class JoinNode:IParseSql
    {
        ITable rightTable;
        WhereNode jw;
        JoinOpt joption;
        public JoinNode(Table rightTable,WhereNode jw,JoinOpt joption)
        {
            this.rightTable = rightTable;
            this.jw = jw;
            this.joption = joption;
        }


     

        public string Parse(ParseSqlContext context)
        {
            var str = $"{joption.ToSql()} {this.rightTable.Name} AS {this.rightTable.NameNick} ON {this.jw.Parse(context)}";
            return str;    
        }
    }
}
