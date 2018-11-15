using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public class NodeOrderBy:Node
    {
        protected Order opt;

        public NodeOrderBy(IColumn col,Order opt): base(col)
        {
            this.opt = opt;
        }

        protected override string ToSQL(ResolveContext context)
        {
            return this.column.ToSQL(context) +" "+ opt.ToString();
        }
    }
}
