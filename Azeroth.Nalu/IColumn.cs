using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public interface IColumn:IConvertible
    {
        ITable Container { get; }
        string ColumnName { get; }
        Function FunctionCode { get; }
        Func<Column, string> FunctionHandler { get; }
    }
}
