﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Azeroth.Nalu;

namespace T4
{
    class Home
    {
        static void Main(string[] args)
        {
            //CalTzp();
            DbContext dbcontext = new DbContext();
            
            var lst= dbcontext.Set<UserInfo>()
                .Select(x => new { x.Id, x.Name })
                .Where(x => x.Col(a => a.Name).In("张三", "李四"))
                .Join(dbcontext.Set<RUserInfoRoleInfo>(), (x, y) => x.Col(a => a.Id) == y.Col(a => a.UserId), (user, relation) => new { user, relation })
                .Join(dbcontext.Set<RoleInfo>(), (x, y) => x.Col(a => a.relation.RoleId) == y.Col(a => a.Id), (x, role) => new { x.user, role })
                .Where(x => x.Col(a => a.role.Name).In("管理员", "admin") || x.Col(a => a.user.Id) == Guid.Empty)
                .SkipTake(0,50)
                .ToList(x => Tuple.Create(x.user, x.role));

            dbcontext.Delete<UserInfo>();

            //var query = dbcontext.Query();
            //var user = query.Set<UserInfo>().Select(x => new { x.Name, x.Id });
            //var userRole = query.Set<RUserInfoRoleInfo>();
            //var role = query.Set<RoleInfo>().Select(x => new { x.Name, x.Id });

            //user.Join(userRole, Azeroth.Nalu.JOIN.Inner)
            //    .ON(user.Col(x => x.Id) == userRole.Col(x => x.UserId))

            //userRole.Join(role, Azeroth.Nalu.JOIN.Inner).ON = userRole.Col(x => x.RoleId) == role.Col(x => x.Id);
            //var lst = query.ToList(x => Tuple.Create((UserInfo)x[0], (RoleInfo)x[2]));


        }

        private static void CalTzp() {
            String appSecret = "0221860e";
            String nonce = "1234567890";
            String curTime = "1610717873";
            //curTime = ((long)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            var tmp = appSecret + nonce + curTime;

            System.Security.Cryptography.SHA1Cng sha = new System.Security.Cryptography.SHA1Cng();
            var buffer= sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(tmp));
            var lst= buffer.Select(x => x.ToString("x2")).ToList();
            string rt = string.Concat(lst);
        }
    }

    public class DbContext : Azeroth.Nalu.DbContext
    {
        //static string cnnstr = System.Configuration.ConfigurationManager.ConnectionStrings["mssqlmaster"].ConnectionString;
        public DbContext()
        {
            //this.Cnnstr = cnnstr;
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
