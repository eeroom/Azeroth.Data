using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public abstract class NodeWhere : Node
    {
        public NodeWhere(IColumn column):base(column)
        { 
        
        }

        public NodeWhere()
        { 
        
        }

        public bool Placeholder{protected set;get;}


        public static NodeWhere operator &(NodeWhere wh1,NodeWhere wh2)
        {
            NodeWhereSegment sm = new NodeWhereSegment(wh1, Logic.And,wh2);
            return sm;
        }

        public static NodeWhere operator |(NodeWhere wh1, NodeWhere wh2)
        {
            
            NodeWhereSegment sm = new NodeWhereSegment(wh1, Logic.OR, wh2);
            return sm;
        }

        

        public static bool operator true(NodeWhere wh1)
        {
            return false;
        }
        public static bool operator false(NodeWhere wh1)
        {
            return false;
        }
    }
}
