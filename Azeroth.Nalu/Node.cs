using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public  class Node:INode
    {
        protected IColumn column;
        public Node()
        { 
        
        }
        public Node(IColumn column)
        {
            this.column = column;
        }

        IColumn INode.Column
        {
            get { return this.column; }
        }

        string ISQL.ResolveSQL(ResovleContext context) {
            return this.ResolveSQL(context);
        }

        protected virtual string ResolveSQL(ResovleContext context)
        {
            return this.column.ResolveSQL(context);
        }
    }
}
