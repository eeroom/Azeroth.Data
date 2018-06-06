using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// 聚合函数，自定义函数等
    /// </summary>
    public enum Function
    {
        NONE,
        Sum,
        Avg,
        Count,
        Max,
        Min,
        Lower,
        Upper,
        SumOrDefault ,
        AvgOrDefault,
        CountOrDefault,
        MaxOrDefault,
        MinOrDefault,
        LowerOrDefault,
        UpperOrDefault
    }
}
