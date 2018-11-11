using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class MySqlDbContext : DbContext<MySql.Data.MySqlClient.MySqlConnection>
    {
        public override Container CreateContainer()
        {
            return new MySqlContainer(this);
        }

        public override ResovleContext GetResolveContext()
        {
            return new ResovleContext("@",()=>new MySql.Data.MySqlClient.MySqlParameter());
        }
    }
}
