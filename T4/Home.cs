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
            var cnnstr= System.Configuration.ConfigurationManager.ConnectionStrings["mysqlmaster"].ConnectionString;
            Azeroth.Nalu.DbContextMysql dbContext = new Azeroth.Nalu.DbContextMysql() {  Cnnstr=cnnstr};
            var query= dbContext.CreateQuery();
            var dbset= query.Set<Tb_ArticleInfo>();
            query.Select(dbset.Cols());
            query.WH = dbset.Col(x => x.Clicks) >= 5 && dbset.Col(x => x.ContentBody) == 1
                &&dbset.Col(x=>x.Content).Like("%股权%") || dbset.Col(x=>x.ContentType).In(1,2);
            query.Take(1,10);
            query.OrderBy(dbset.Col(x => x.Id), Azeroth.Nalu.Order.ASC);
            int rowscount;
            var lst= query.ToList<Tb_ArticleInfo>(out rowscount);


        }

    }
}
