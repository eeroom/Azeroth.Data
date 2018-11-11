using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class NodeWhereSegment : NodeWhere
    {
        const string AND = "AND";
        const string OR = "OR";
        NodeWhere left;
        INode right;
        Logic opt;
        public NodeWhereSegment(NodeWhere left, Logic opt, NodeWhere right)
        {
            this.left = left;
            this.opt = opt;
            this.right = right;
        }

        protected override string ToSQL(ResovleContext context)
        {
            if (left.Placeholder)
                return right.ToSQL(context);
            if (this.opt == Logic.And)
                return string.Format("{0} {1} {2}", ((IResolver)left).ToSQL(context), AND, right.ToSQL(context));
            return string.Format("({0} {1} {2})", ((IResolver)left).ToSQL(context), OR, right.ToSQL(context)); ;
        }
    }
}
