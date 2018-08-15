using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public interface IDbContext
    {
        List<T> ExecuteQuery<T>(IQuery query, Func<object[], T> transfer);
        RT ExecuteNoQuery(params ICud[] lstcud);
        ResovleContext GetResolvContext();

        DbCud<T> CreateNoQuery<T>();
        Query CreateQuery();
    }
}
