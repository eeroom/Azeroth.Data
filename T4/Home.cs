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
            //用户表--用户角色关系表--角色表
            Azeroth.Nalu.Query myQuery= dbcontext.Query();//新建一个查询
            var tbUser= myQuery.DbSet<Tb_User>();//用户表
            var reUserRole = myQuery.DbSet<Re_User_Role>();//用户角色关系表
            var tbRole = myQuery.DbSet<Tb_Role>();//角色表

            myQuery.Select(tbUser.Columns(x=>new { x.Account,x.Avatar,x.CreateTime,x.RealName,x.UserPwd,x.UserPwdHistory}));//查询用户表的这些列
            myQuery.Select(tbRole.Columns(x => new { x.Name, x.Id }));//查询角色表的这些列

            tbUser.InnerJoin(reUserRole).ON = tbUser.Col(x => x.Id) == reUserRole.Col(x=>x.UserId);//用户表 关联 用户角色关系，
            reUserRole.InnerJoin(tbRole).ON = reUserRole.Col(x => x.RoleId) == tbRole.Col(x=>x.Id);//用户角色关系 关联 角色表

            myQuery.WH =//筛选条件
                (tbUser.Col(x => x.CreateTime) > new DateTime(2011,1,1) || !tbRole.Col(x => x.Name).Contains("sysAdmin","sys","system")) 
                && tbUser.Col(x => x.CreateUser) =="system";

            string sqlstr = myQuery.ToString(new Azeroth.Nalu.ResovleContext("@",()=>new System.Data.SqlClient.SqlParameter()));
            //var lst= myQuery.ToList(x=>Tuple.Create((Tb_User)x[0],(Tb_Role)x[2]));//获取结果，
            //var dictUserRole= lst.GroupBy(x => x.Item1.Account).ToDictionary(x => x.Key, x => x.Select(a => a.Item2.Name).ToList());//转换成字典，键-用户名，值-角色名称的List

        }
    }
}
