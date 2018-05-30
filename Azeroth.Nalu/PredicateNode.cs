using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public abstract class PredicateNode : Node
    {
        public PredicateNode(IColumn column):base(column)
        { 
        
        }

        public PredicateNode()
        { 
        
        }

        public bool Placeholder{protected set;get;}


        public static PredicateNode operator &(PredicateNode wh1,PredicateNode wh2)
        {
            PredicateSegment sm = new PredicateSegment(wh1, Logic.And,wh2);
            return sm;
        }

        public static PredicateNode operator |(PredicateNode wh1, PredicateNode wh2)
        {
            
            PredicateSegment sm = new PredicateSegment(wh1, Logic.OR, wh2);
            return sm;
        }

        

        public static bool operator true(PredicateNode wh1)
        {
            return false;
        }
        public static bool operator false(PredicateNode wh1)
        {
            return false;
        }
    }
}
