using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public interface IDbContext
    {
        List<T> ExecuteQuery<T>(IDbSetContainer query, Func<object[], T> transfer);
        Result ExecuteNoQuery(params ICud[] lstcud);
        ResovleContext GetResolvContext();

        DbCud<T> NoQuery<T>();
        DbSetContainer Query();
    }
}
