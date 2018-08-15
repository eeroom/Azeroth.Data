using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class DbContextMysql : DbContext<MySql.Data.MySqlClient.MySqlConnection>
    {
        public override Query CreateQuery()
        {
            return new QueryMysql(this);
        }

        public override ResovleContext GetResolvContext()
        {
            return new ResovleContext("@",()=>new MySql.Data.MySqlClient.MySqlParameter());
        }
    }
}
