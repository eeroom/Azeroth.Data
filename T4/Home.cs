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
            object o = new object();
            Azeroth.Nalu.IDbContext dbContext = new MssqlDbContext();
            var query= dbContext.CreateQuery();
            var tbEvaPeople= query.Set<Tb_EvaluatedPeople>();
            query.Select(tbEvaPeople.Cols());
            var lst= query.ToList<Tb_EvaluatedPeople>();

        }

    }
}
