using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class MssqlserverDbContext : DbContext<System.Data.SqlClient.SqlConnection>
    {
        public override Container CreateContainer()
        {
            return new MssqlserverContainer(this);
        }

        public override ResovleContext GetResolveContext()
        {
            return new ResovleContext("@",()=>new System.Data.SqlClient.SqlParameter());
        }
    }
}
