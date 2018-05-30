using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T4
{
    class Default
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Azeroth.Nalu的Demo");
            Console.WriteLine("QQ1344206656");
            OrmDemo();
            Console.ReadKey();
            

        }

        public static void OrmDemo()
        {
            MssqlDbContext dbcontext = new MssqlDbContext();

            Azeroth.Nalu.Query myQuery= dbcontext.Query();
            var tbUser= myQuery.DbSet<Tb_User>();
            var reUserRole = myQuery.DbSet<Re_User_Role>();
            var tbRole = myQuery.DbSet<Tb_Role>();

            myQuery.Select(tbUser.Columns(x=>new { x.Account,x.Avatar,x.CreateTime,x.RealName,x.UserPwd,x.UserPwdHistory}));
            myQuery.Select(tbRole.Columns(x => new { x.Name, x.Id }));

            tbUser.InnerJoin(reUserRole).ON = tbUser.Col(x => x.Id) == reUserRole.Col(x=>x.UserId);
            reUserRole.InnerJoin(tbRole).ON = reUserRole.Col(x => x.RoleId) == tbRole.Col(x=>x.Id);

            var lst= myQuery.ToList(x=>Tuple.Create((Tb_User)x[0],(Tb_Role)x[2]));
            var dictUserRole= lst.GroupBy(x => x.Item1.Account).ToDictionary(x => x.Key, x => x.Select(a => a.Item2.Name).ToList());

        }
    }
}
