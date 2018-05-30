using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class PredicateSegment : PredicateNode
    {
        const string AND = "AND";
        const string OR = "OR";
        PredicateNode left;
        INode right;
        Logic opt;
        public PredicateSegment(PredicateNode left, Logic opt, PredicateNode right)
        {
            this.left = left;
            this.opt = opt;
            this.right = right;
        }

        protected override string ResolveSQL(ResovleContext context)
        {
            if (left.Placeholder)
                return right.ResolveSQL(context);
            if (this.opt == Logic.And)
                return string.Format("{0} {1} {2}", ((ISQL)left).ResolveSQL(context), AND, right.ResolveSQL(context));
            return string.Format("({0} {1} {2})", ((ISQL)left).ResolveSQL(context), OR, right.ResolveSQL(context)); ;
        }
    }
}
