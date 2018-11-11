using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public class NodeJOIN:Node
    {
        public NodeWhere ON { set; get; }

        JOIN opt;

        ITable container;
        public NodeJOIN(JOIN opt, Table dbr)
        {
            this.opt = opt;
            this.container = dbr;
        }

        protected override string ToSQL(ResovleContext context)
        {
            if (opt == JOIN.None)
                return ToSQLWithScalar(context);
            string strwhere = ((IResolver)this.ON).ToSQL(context);
            return string.Format("\r\n{0} {1} AS {2} ON {3}",this.opt.ToSQL(),this.container.NameHandler(context),this.container.NameNick,strwhere);
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
