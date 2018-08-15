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
            var lst= query.ToList<Tb_ArticleInfo>();

            using (var cnn=new MySql.Data.MySqlClient.MySqlConnection(cnnstr))
            {
                cnn.Open();
                using (var cmd=cnn.CreateCommand())
                {
                    cmd.CommandText = "select * from Tb_ArticleInfo";
                    using (var reader=cmd.ExecuteReader( System.Data.CommandBehavior.SchemaOnly))
                    {
                        var table= reader.GetSchemaTable();
                    }
                }
            }

        }

    }
}
