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
        

        public static string ToSQL(this WH opt)
        {
            string rst = string.Empty;
            switch (opt)
            {
                case WH.EQ:
                    rst = "=";
                    break;
                case ~WH.EQ:
                    rst = "<>";
                    break;
                case WH.LT:
                    rst = "<";
                    break;
                case WH.LT | WH.EQ:
                    rst = "<=";
                    break;
                case ~WH.GT:
                    rst = "<=";
                    break;
                case WH.GT:
                    rst = ">";
                    break;
                case WH.GT | WH.EQ:
                    rst = ">=";
                    break;
                case ~WH.LT:
                    rst = ">=";
                    break;
                case WH.IN:
                    rst = "IN";
                    break;
                case ~WH.IN:
                    rst = "NOT IN";
                    break;
                case WH.BT:
                    rst = "BETWEEN";
                    break;
                case ~WH.BT:
                    rst = "NOT BETWEEN";
                    break;
                case WH.LIKE:
                    rst = "LIKE";
                    break;
                case ~WH.LIKE:
                    rst = "NOT LIKE";
                    break;
                case WH.GTE:
                    rst = ">=";
                    break;
                case WH.LTE:
                    rst = "<=";
                    break;
                case WH.NoParameter:
                    break;
                case WH.NULL:
                    rst = "IS NULL";
                    break;
                case ~WH.NULL:
                    rst = "IS NOT NULL";
                    break;
                case WH.Exists:
                    rst = "EXISTS";
                    break;
                case ~WH.Exists:
                    rst = "NOT EXISTS";
                    break;
                default:
                    throw new ArgumentException("错误的关系运算符");
            }
            return rst;
        }

        public static string ToSQL(this JOIN opt)
        {
            string rst = string.Empty;
            switch (opt)
            {
                case JOIN.Inner:
                    rst = "INNER JOIN";
                    break;
                case JOIN.Left:
                    rst = "LEFT OUTER JOIN";
                    break;
                case JOIN.Right:
                    rst = "RIGHT OUTER JOIN";
                    break;
                case JOIN.None:
                    break;
                default:
                    throw new ArgumentException("未知的表连接运算符");
            }
            return rst;
        }

        public static string ToSQL(this Order opt)
        {
            switch (opt)
            {
                case Order.ASC:
                    return " ASC";
                case Order.DESC:
                    return " DESC";
                default:
                    throw new ArgumentException("未知的排序修饰符");
            }
        }

    }
}
