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
            //query.Select(dbset.Col(x => x.ContentType));
            //query.Select(dbset.Col(x => x.Clicks).Function(Azeroth.Nalu.Function.Sum));
            query.Where = dbset.Col(x => x.Clicks) >= 5 && dbset.Col(x => x.ContentBody) == 1
                && dbset.Col(x => x.Content).Like("%股权%") || dbset.Col(x => x.ContentType).In(1, 2);
            //query.Distinct();
            //query.Top(4);
            //query.GroupBy(dbset.Col(x => x.ContentType));
            //query.Having = dbset.Col(x => x.Clicks).Function(Azeroth.Nalu.Function.Max) > 10;
            query.Take(4,10);
            query.OrderBy(dbset.Col(x => x.ContentType), Azeroth.Nalu.Order.ASC);
            int rowscount;
            var lst= query.ToList<Tb_ArticleInfo>(out rowscount);


        }

    }
}
