using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public interface IDbContext
    {
        List<T> ToList<T>(IQuery container, Func<object[], T> transfer);
        int SaveChange(params ICud[] lstcud);
        ResolveContext GetResolveContext();

        DbCud<T> Cud<T>() where T : class;
        Query CreateContainer();

        DbSet<T> Set<T>(IQuery container);
    }
}
