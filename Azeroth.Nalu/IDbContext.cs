using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public interface IDbContext
    {
        List<T> ToList<T>(Func<System.Data.Common.DbDataReader, T> map, Action<ParseSqlContext,bool> initParseSqlContext);
    }
}
