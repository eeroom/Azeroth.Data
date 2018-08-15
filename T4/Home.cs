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
            var tbEvaPeople= query.Set<Tb_EvaluatedPeople>();
            query.Select(tbEvaPeople.Cols());
            var lst= query.ToList<Tb_EvaluatedPeople>();

        }

    }
}
