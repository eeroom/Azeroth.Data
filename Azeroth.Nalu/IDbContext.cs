using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public interface IDbContext
    {
        string Cnnstr { get; set; }
        int SaveChange(params ICud[] lstcud);
        ResolveContext GetResolveContext();

        DbCud<T> Cud<T>() where T : class;
        Query Query();

        System.Data.Common.DbProviderFactory GetDbProviderFactory();
    }
}
