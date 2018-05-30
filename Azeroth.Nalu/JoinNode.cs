using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public class JoinNode:Node
    {
        public PredicateNode ON { set; get; }

        JOIN opt;

        IDbSet container;
        public JoinNode(JOIN opt, DbSet dbr)
        {
            this.opt = opt;
            this.container = dbr;
        }

        protected override string ResolveSQL(ResovleContext context)
        {
            if (opt == JOIN.None)
                return ToSQLWithScalar(context);
            string strwhere = ((ISQL)this.ON).ResolveSQL(context);
            return string.Format("\r\n{0} {1} AS {2} ON {3}",this.opt.Totxt(),this.container.NameHandler(context),this.container.NameNick,strwhere);
        }

        private string ToSQLWithScalar(ResovleContext context)
        {
            throw new NotImplementedException();
        }

        //public void ON(PredicateNode predicate)
        //{
        //    this.predicate = predicate;
            
        //}

    }
}
