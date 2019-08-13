using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T4
{
    class Home
    {
        static void Main(string[] args)
        {
            DbContext dbcontext = new DbContext();
            var container= dbcontext.CreateContainer();
            var user= dbcontext.Set<UserInfo>(container).Select(x => new { x.Name,x.Id });
            var userRole = dbcontext.Set<RUserInfoRoleInfo>(container);
            var role = dbcontext.Set<RoleInfo>(container).Select(x => new { x.Name,x.Id});
            user.Join(userRole, Azeroth.Nalu.JOIN.Inner).ON = user.Col(x => x.Id) == userRole.Col(x => x.UserId);
            userRole.Join(role, Azeroth.Nalu.JOIN.Inner).ON = userRole.Col(x => x.RoleId) == role.Col(x=>x.Id);
            var lst= container.ToList(x => Tuple.Create((UserInfo)x[0], (RoleInfo)x[2]));


        }

    }

    public class DbContext:Azeroth.Nalu.DbContext<System.Data.SqlClient.SqlConnection>
    {
        static string cnnstr = System.Configuration.ConfigurationManager.ConnectionStrings["mssqlmaster"].ConnectionString;
        public DbContext()
        {
            this.Cnnstr = cnnstr;
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
