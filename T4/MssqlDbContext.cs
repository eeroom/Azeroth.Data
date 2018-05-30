using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azeroth.Nalu;

namespace T4
{
    public class MssqlDbContext:Azeroth.Nalu.DbContext<System.Data.SqlClient.SqlConnection>
    {
        static string cnnstr=System.Configuration.ConfigurationManager.ConnectionStrings["mssqlmaster"].ConnectionString;

        public MssqlDbContext()
        {
            this.Cnnstr = cnnstr;
        }
        protected override ResovleContext GetResolvContext()
        {
            return new ResovleContext("@", () => new System.Data.SqlClient.SqlParameter());
        }
    }
}
