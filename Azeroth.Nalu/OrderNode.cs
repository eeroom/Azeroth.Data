using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public class OrderNode:Node
    {
        protected Order opt;

        public OrderNode(IColumn col,Order opt): base(col)
        {
            this.opt = opt;
        }

        protected override string ResolveSQL(ResovleContext context)
        {
            return this.column.ResolveSQL(context) +" "+ opt.ToString();
        }
    }
}
