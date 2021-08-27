using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu.Node
{
    public abstract class WhereNode:IParseSql
    {

        public static WhereNode operator &(WhereNode wh1,WhereNode wh2)
        {
            WhereSegmentNode sm = new WhereSegmentNode(wh1, LogicOpt.And,wh2);
            return sm;
        }

        public static WhereNode operator |(WhereNode wh1, WhereNode wh2)
        {
            WhereSegmentNode sm = new WhereSegmentNode(wh1, LogicOpt.OR, wh2);
            return sm;
        }

        

        public static bool operator true(WhereNode wh1)
        {
            return false;
        }
        public static bool operator false(WhereNode wh1)
        {
            return false;
        }

        public abstract string Parse(ParseSqlContext context);
    }
}
