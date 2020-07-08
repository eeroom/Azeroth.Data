using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu.Node
{
    public class OrderByNode:NodeBase
    {
        protected Order opt;

        public OrderByNode(IColumn col,Order opt): base(col)
        {
            this.opt = opt;
        }

        protected override string ToSQL(ResolveContext context)
        {
            return this.column.ToSQL(context) +" "+ opt.ToString();
        }
    }
}
