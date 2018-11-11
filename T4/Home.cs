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
            Azeroth.Nalu.MySqlDbContext dbContext = new Azeroth.Nalu.MySqlDbContext() {  Cnnstr=cnnstr};
            var container= dbContext.CreateContainer();
            var dbset= container.Set<Tb_ArticleInfo>();
            container.Select(dbset.Cols());
            //query.Where = dbset.Col(x => x.ContentType) == 888;
            //query.Select(dbset.Col(x => x.ContentType));
            //query.Select(dbset.Col(x => x.Clicks).Function(Azeroth.Nalu.Function.Sum));
            var dbTopic = container.Set<Tb_ArticleTopic>();
            dbset.InnerJoin(dbTopic).ON = dbset.Col(x => x.ArticleTopicId) == dbTopic.Col(x=>x.Id);
            container.Select(dbTopic.Cols());
            container.Where = dbset.Col(x => x.Clicks) >= 5 && dbset.Col(x => x.ContentBody) == 1
                && dbset.Col(x => x.Content).Like("%股权%") || dbset.Col(x => x.ContentType).In(1, 2);
            //query.Distinct();
            //query.Top(4);
            //query.GroupBy(dbset.Col(x => x.ContentType));
            //query.Having = dbset.Col(x => x.Clicks).Function(Azeroth.Nalu.Function.Max) > 10;
            //query.Take(1,10);
            container.OrderBy(dbset.Col(x => x.ContentType), Azeroth.Nalu.Order.ASC);
            int rowscount;
            var lst= container.ToList<Tb_ArticleInfo,Tb_ArticleTopic>();
            var lst2= container.ToList<Tb_ArticleInfo,Tb_ArticleTopic>()
                .Select(x=> new { x.Item1.Name,x.Item1.Id,TopicName=x.Item2.Name}).ToList();
            //lst.ForEach(x=>x.ContentType=999);
            //lst.ForEach(x=>x.Id=Guid.NewGuid());

            //var addArticle= dbContext.CreateNoQuery<Tb_ArticleInfo>();
            //addArticle.Del();
            //addArticle.WH = addArticle.Col(x => x.ContentType) == 999;
            ////addArticle.Add(lst);
            ////addArticle.Select(addArticle.Cols());

            ////addArticle.Edit(new Tb_ArticleInfo() {  ContentType=888});
            ////addArticle.WH = addArticle.Col(x => x.ContentType) == 999;
            ////addArticle.Select(addArticle.Col(x=>x.ContentType));
            //var m= dbContext.ExecuteNoQuery(addArticle);
        }

    }
}
