using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public interface IDbContext
    {
        Tuple<List<T>, int> ToList<T>(Func<System.Data.Common.DbDataReader, T> map, Action<ParseSqlContext,bool> initParseSqlContext);
    }
}
