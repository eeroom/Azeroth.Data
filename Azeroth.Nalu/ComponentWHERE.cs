using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public abstract class ComponentWHERE : Component
    {
        public ComponentWHERE(IColumn column):base(column)
        { 
        
        }

        public ComponentWHERE()
        { 
        
        }

        public bool Placeholder{protected set;get;}


        public static ComponentWHERE operator &(ComponentWHERE wh1,ComponentWHERE wh2)
        {
            ComponentSegment sm = new ComponentSegment(wh1, Logic.And,wh2);
            return sm;
        }

        public static ComponentWHERE operator |(ComponentWHERE wh1, ComponentWHERE wh2)
        {
            
            ComponentSegment sm = new ComponentSegment(wh1, Logic.OR, wh2);
            return sm;
        }

        

        public static bool operator true(ComponentWHERE wh1)
        {
            return false;
        }
        public static bool operator false(ComponentWHERE wh1)
        {
            return false;
        }
    }
}
