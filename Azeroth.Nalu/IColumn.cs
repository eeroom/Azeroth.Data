using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public interface IColumn:ISQL
    {
        IDbSet Container { get; }
        string ColumnName { get; }
        Function FunctionCode { get; }
        Func<Column, string> FunctionHandler { get; }
    }
}
