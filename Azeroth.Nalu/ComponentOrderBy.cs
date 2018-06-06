using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public class ComponentOrderBy:Component
    {
        protected Order opt;

        public ComponentOrderBy(IColumn col,Order opt): base(col)
        {
            this.opt = opt;
        }

        protected override string ToSQL(ResovleContext context)
        {
            return this.column.ToSQL(context) +" "+ opt.ToString();
        }
    }
}
