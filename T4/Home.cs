using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace T4
{
    class Home
    {
        static void Main(string[] args)
        {
            DbContext dbcontext = new DbContext();
            var query= dbcontext.Query();
            var user= query.Set<UserInfo>().Select(x => new { x.Name,x.Id });
            var userRole = query.Set<RUserInfoRoleInfo>();
            var role = query.Set<RoleInfo>().Select(x => new { x.Name,x.Id});

            //user.Join(userRole, Azeroth.Nalu.JOIN.Inner)
            //    .ON(user.Col(x => x.Id) == userRole.Col(x => x.UserId))

            //userRole.Join(role, Azeroth.Nalu.JOIN.Inner).ON = userRole.Col(x => x.RoleId) == role.Col(x=>x.Id);
            //var lst= query.ToList(x => Tuple.Create((UserInfo)x[0], (RoleInfo)x[2]));


        }

    }

    public class DbContext:Azeroth.Nalu.DbContext
    {
        static string cnnstr = System.Configuration.ConfigurationManager.ConnectionStrings["mssqlmaster"].ConnectionString;
        public DbContext()
        {
            this.Cnnstr = cnnstr;
        }

        public override DbProviderFactory GetDbProviderFactory()
        {
            return System.Data.SqlClient.SqlClientFactory.Instance;
        }
    }

    public class UserInfo
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }

    public class RoleInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class RUserInfoRoleInfo
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }
    }
}
