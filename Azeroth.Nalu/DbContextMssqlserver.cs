using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class DbContextMssqlserver : DbContext<System.Data.SqlClient.SqlConnection>
    {
        public override Container CreateQuery()
        {
            return new QueryMssqlserver(this);
        }

        public override ResovleContext GetResolvContext()
        {
            return new ResovleContext("@",()=>new System.Data.SqlClient.SqlParameter());
        }
    }
}
