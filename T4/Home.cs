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
            var user= dbcontext.Set<GZ_User>(container).Select(x => new { x.Name, x.UserName });
            var userRole = dbcontext.Set<GZ_UserRole>(container);
            var role = dbcontext.Set<GZ_Role>(container).Select(x => new { x.Name,x.Code});
            user.Join(userRole, Azeroth.Nalu.JOIN.Inner).ON = user.Col(x => x.Id) == userRole.Col(x => x.UserId);
            userRole.Join(role, Azeroth.Nalu.JOIN.Inner).ON = userRole.Col(x => x.RoleId) == role.Col(x=>x.Id);
            var lst= container.ToList(x => Tuple.Create((GZ_User)x[0], (GZ_Role)x[2]));


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
}
