using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu.Node
{
    /// <summary>
    /// Join中的where
    /// </summary>
    /// <typeparam name="L"></typeparam>
    /// <typeparam name="R"></typeparam>
    public class WhereJoinOnNode : WhereNode
    {
        Column left;
        Column right;

        public WhereJoinOnNode(Column left, Column right)
        {
            this.left = left;
            this.right = right;
        }

        public override string Parse(ParseSqlContext context)
        {
            var str=$"{this.left.Parse(context)} = { this.right.Parse(context)}";
            return str;
        }

    }
}
