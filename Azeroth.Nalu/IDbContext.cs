using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public interface IDbContext
    {
        List<T> ToList<T>(IContainer container, Func<object[], T> transfer);
        int SaveChange(params ICud[] lstcud);
        ResovleContext GetResolveContext();

        DbCud<T> CreateCud<T>();
        Container CreateContainer();
    }
}
