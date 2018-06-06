using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public interface IDbContext
    {
        List<T> ExecuteQuery<T>(IDbSetContainer query,Func<object[], T> transfer);
        Result SaveChange(params ICud[] lstcud);
        ResovleContext GetResolvContext();
    }
}
