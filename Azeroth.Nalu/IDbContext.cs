using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public interface IDbContext
    {
        List<T> ExecuteQuery<T>(IQuery query,Func<object[], T> transfer);
        SaveChangeResult SaveChange(params ICud[] lstcud);
        ResovleContext GetResolvContext();
    }
}
