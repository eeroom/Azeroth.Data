using Azeroth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public static class Extension
    {
        //public static string ParameterNameForPaginationEnd = "Num_End";
        //public static string ParameterNameForPaginationStart = "Num_Start";
        

        public static string ToSQL(this WhereOpt opt)
        {
            string rst = string.Empty;
            switch (opt)
            {
                case WhereOpt.EQ:
                    rst = "=";
                    break;
                case ~WhereOpt.EQ:
                    rst = "<>";
                    break;
                case WhereOpt.LT:
                    rst = "<";
                    break;
                case WhereOpt.LT | WhereOpt.EQ:
                    rst = "<=";
                    break;
                case ~WhereOpt.GT:
                    rst = "<=";
                    break;
                case WhereOpt.GT:
                    rst = ">";
                    break;
                case WhereOpt.GT | WhereOpt.EQ:
                    rst = ">=";
                    break;
                case ~WhereOpt.LT:
                    rst = ">=";
                    break;
                case WhereOpt.IN:
                    rst = "IN";
                    break;
                case ~WhereOpt.IN:
                    rst = "NOT IN";
                    break;
                case WhereOpt.BT:
                    rst = "BETWEEN";
                    break;
                case ~WhereOpt.BT:
                    rst = "NOT BETWEEN";
                    break;
                case WhereOpt.LIKE:
                    rst = "LIKE";
                    break;
                case ~WhereOpt.LIKE:
                    rst = "NOT LIKE";
                    break;
                case WhereOpt.GTE:
                    rst = ">=";
                    break;
                case WhereOpt.LTE:
                    rst = "<=";
                    break;
                case WhereOpt.NoParameter:
                    break;
                case WhereOpt.NULL:
                    rst = "IS NULL";
                    break;
                case ~WhereOpt.NULL:
                    rst = "IS NOT NULL";
                    break;
                case WhereOpt.Exists:
                    rst = "EXISTS";
                    break;
                case ~WhereOpt.Exists:
                    rst = "NOT EXISTS";
                    break;
                default:
                    throw new ArgumentException("错误的关系运算符");
            }
            return rst;
        }

        public static string ToSQL(this JoinOpt opt)
        {
            string rst = string.Empty;
            switch (opt)
            {
                case JoinOpt.Inner:
                    rst = "INNER JOIN";
                    break;
                case JoinOpt.Left:
                    rst = "LEFT OUTER JOIN";
                    break;
                case JoinOpt.Right:
                    rst = "RIGHT OUTER JOIN";
                    break;
                case JoinOpt.None:
                    break;
                default:
                    throw new ArgumentException("未知的表连接运算符");
            }
            return rst;
        }

        public static string ToSQL(this OrderOpt opt)
        {
            switch (opt)
            {
                case OrderOpt.ASC:
                    return " ASC";
                case OrderOpt.DESC:
                    return " DESC";
                default:
                    throw new ArgumentException("未知的排序修饰符");
            }
        }

    }
}
