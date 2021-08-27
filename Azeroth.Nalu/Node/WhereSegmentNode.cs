using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu.Node
{
    public class WhereSegmentNode : WhereNode
    {
        const string AND = "AND";
        const string OR = "OR";
        WhereNode left;
        WhereNode right;
        LogicOpt opt;

        public WhereSegmentNode(WhereNode left, LogicOpt opt, WhereNode right)
        {
            this.left = left;
            this.opt = opt;
            this.right = right;
        }

        public override string Parse(ParseSqlContext context)
        {
            if (this.opt == LogicOpt.And)
                return string.Format("{0} {1} {2}", this.left.Parse(context), AND, this.right.Parse(context));
            return string.Format("({0} {1} {2})", this.left.Parse(context), OR, this.right.Parse(context));
        }
    }
}
