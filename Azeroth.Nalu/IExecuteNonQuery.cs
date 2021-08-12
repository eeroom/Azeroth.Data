using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public interface IExecuteNonQuery
    {
        int ExecuteNonQuery(System.Data.Common.DbCommand cmd,ParseSqlContext context);

        //bool Validate(out string msg);

    }
}
