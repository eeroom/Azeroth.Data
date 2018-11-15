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

        protected override ResolveContext GetResolveContext()
        {
            return new ResolveContext("@",()=>new MySql.Data.MySqlClient.MySqlParameter());
        }
    }
}
