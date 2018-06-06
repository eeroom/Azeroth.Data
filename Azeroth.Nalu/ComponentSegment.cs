using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class ComponentSegment : ComponentWHERE
    {
        const string AND = "AND";
        const string OR = "OR";
        ComponentWHERE left;
        IComponent right;
        Logic opt;
        public ComponentSegment(ComponentWHERE left, Logic opt, ComponentWHERE right)
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
                return string.Format("{0} {1} {2}", ((IConvertible)left).ToSQL(context), AND, right.ToSQL(context));
            return string.Format("({0} {1} {2})", ((IConvertible)left).ToSQL(context), OR, right.ToSQL(context)); ;
        }
    }
}
