using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu.Node
{
    public class OrderByNode:IParseSql
    {
        OrderOpt opt;

        Column col;
        public OrderByNode(Column col,OrderOpt opt)
        {
            this.col = col;
            this.opt = opt;
        }


        public string Parse(ParseSqlContext context)
        {
            var str = $"{this.col.Parse(context)} {opt.ToSql()}";
            return str;
        }
    }
}
