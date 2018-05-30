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
        NONE=0,
        SUM=1,
        AVG=2,
        COUNT=3,
        MAX=4,
        MIN=5,
        LOWER=6,
        UPPER=7
    }
}
