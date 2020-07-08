using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu.Node
{
    public class JoinNode: NodeBase
    {
        WhereNode whereNode { set; get; }

        JOIN opt;

        ITable container;
        Query query;
        public JoinNode(JOIN opt, Table dbr,Query query)
        {
            this.opt = opt;
            this.container = dbr;
            this.query = query;
        }

        protected override string ToSQL(ResolveContext context)
        {
            if (opt == JOIN.None)
                return ToSQLWithScalar(context);
            string strwhere = ((IResolver)this.whereNode).ToSQL(context);
            return string.Format("\r\n{0} {1} AS {2} ON {3}",this.opt.ToSQL(),this.container.NameHandler(context),this.container.NameNick,strwhere);
        }

        private string ToSQLWithScalar(ResolveContext context)
        {
            throw new NotImplementedException();
        }

        //public void ON(PredicateNode predicate)
        //{
        //    this.predicate = predicate;
            
        //}

        public Query ON(WhereNode predicate)
        {
            this.whereNode = predicate;
            return this.query;
            
        }

    }
}
