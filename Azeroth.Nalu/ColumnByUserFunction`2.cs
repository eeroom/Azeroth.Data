using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class ColumnByUserFunction<T, S> : Column<T, S>
    {
        Func<Column<T, S>, ParseSqlContext, string> userfunction;
        public ColumnByUserFunction(Table db, string colName,Func<Column<T,S>, ParseSqlContext,string> userfunction) :base(db,  colName)
        {
            this.userfunction = userfunction;
        }

        public override string Parse(ParseSqlContext context)
        {
            return this.userfunction(this, context);
        }
    }
}
