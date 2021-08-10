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
        Logic opt;

        public WhereSegmentNode(WhereNode left, Logic opt, WhereNode right)
        {
            this.left = left;
            this.opt = opt;
            this.right = right;
        }

        public override string Parse(ParseSqlContext context)
        {
            if (this.opt == Logic.And)
                return string.Format("{0} {1} {2}", this.right.Parse(context), AND, this.left.Parse(context));
            return string.Format("({0} {1} {2})", this.right.Parse(context), OR, this.left.Parse(context));
        }
    }
}
