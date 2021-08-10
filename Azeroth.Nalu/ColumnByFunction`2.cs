using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class ColumnByFunction<T,S>:Column<T,S>
    {
        Function fc;
        public ColumnByFunction(Table db, string colName,Function fc) :base(db,colName)
        {
            this.fc = fc;
        }

        public override string Parse(ParseSqlContext context)
        {
            string tmp = base.Parse(context);
            string rst = string.Empty;
            switch (this.fc)
            {
                case Azeroth.Nalu.Function.Sum:
                    rst = string.Format("SUM({0})", tmp);
                    break;
                case Azeroth.Nalu.Function.Avg:
                    rst = string.Format("AVG({0})", tmp);
                    break;
                case Azeroth.Nalu.Function.Count:
                    rst = string.Format("COUNT({0})", tmp);
                    break;
                case Azeroth.Nalu.Function.Max:
                    rst = string.Format("MAX({0})", tmp);
                    break;
                case Azeroth.Nalu.Function.Min:
                    rst = string.Format("MIN({0})", tmp);
                    break;
                case Azeroth.Nalu.Function.Lower:
                    rst = string.Format("LOWER({0})", tmp);
                    break;
                case Azeroth.Nalu.Function.Upper:
                    rst = string.Format("UPPER({0})", tmp);
                    break;
                default:
                    throw new ArgumentException("未识别的函数");
            }
            return rst;
        }
    }
}
