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
        ResolveContext GetResolveContext();

        DbCud<T> Cud<T>() where T : class;
        Container CreateContainer();

        DbSet<T> Set<T>(IContainer container);
    }
}
