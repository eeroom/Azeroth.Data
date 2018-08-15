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
            //query.Where = dbset.Col(x => x.ContentType) == 888;
            //query.Select(dbset.Col(x => x.ContentType));
            //query.Select(dbset.Col(x => x.Clicks).Function(Azeroth.Nalu.Function.Sum));
            //query.Where = dbset.Col(x => x.Clicks) >= 5 && dbset.Col(x => x.ContentBody) == 1
            //    && dbset.Col(x => x.Content).Like("%股权%") || dbset.Col(x => x.ContentType).In(1, 2);
            //query.Distinct();
            //query.Top(4);
            //query.GroupBy(dbset.Col(x => x.ContentType));
            //query.Having = dbset.Col(x => x.Clicks).Function(Azeroth.Nalu.Function.Max) > 10;
            //query.Take(1,10);
            query.OrderBy(dbset.Col(x => x.ContentType), Azeroth.Nalu.Order.ASC);
            int rowscount;
            var lst= query.ToList<Tb_ArticleInfo>(out rowscount);

            lst.ForEach(x=>x.ContentType=999);
            lst.ForEach(x=>x.Id=Guid.NewGuid());

            var addArticle= dbContext.CreateNoQuery<Tb_ArticleInfo>();
            addArticle.Del();
            addArticle.WH = addArticle.Col(x => x.ContentType) == 999;
            //addArticle.Add(lst);
            //addArticle.Select(addArticle.Cols());

            //addArticle.Edit(new Tb_ArticleInfo() {  ContentType=888});
            //addArticle.WH = addArticle.Col(x => x.ContentType) == 999;
            //addArticle.Select(addArticle.Col(x=>x.ContentType));
            var m= dbContext.ExecuteNoQuery(addArticle);
        }

    }
}
