using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu.Node
{
    public  class NodeBase:INodeBase
    {
        protected IColumn column;
        public NodeBase()
        { 
        
        }
        public NodeBase(IColumn column)
        {
            this.column = column;
        }

        IColumn INodeBase.Column
        {
            get { return this.column; }
        }

        string IResolver.ToSQL(ResolveContext context) {
            return this.ToSQL(context);
        }

        protected virtual string ToSQL(ResolveContext context)
        {
            return this.column.ToSQL(context);
        }
    }
}
