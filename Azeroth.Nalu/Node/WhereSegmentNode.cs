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
        INodeBase right;
        Logic opt;
        public WhereSegmentNode(WhereNode left, Logic opt, WhereNode right)
        {
            this.left = left;
            this.opt = opt;
            this.right = right;
        }

        protected override string ToSQL(ResolveContext context)
        {
            if (left.Placeholder)
                return right.ToSQL(context);
            if (this.opt == Logic.And)
                return string.Format("{0} {1} {2}", ((IResolver)left).ToSQL(context), AND, right.ToSQL(context));
            return string.Format("({0} {1} {2})", ((IResolver)left).ToSQL(context), OR, right.ToSQL(context)); ;
        }
    }
}
