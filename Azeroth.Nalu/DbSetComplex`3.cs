using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class DbSetComplex<L, R, C> : DbSet<C> 
    {
        DbSet<L> left { set; get; }
        DbSet<R> right { set; get; }
        Node.WhereNode joinwh { set; get; }
        Func<L, R, C> mapper { set; get; }

        internal DbSetComplex(DbContext context,DbSet<L> left,DbSet<R> right,Func<DbSet<L>,DbSet<R>,Node.WhereJoinOnNode> jwHandler,Func<L,R,C> mapper) :base(context, true)
        {
            this.left = left;
            this.right = right;
            this.mapper = mapper;
            this.identity = this.mapper(left.identity, right.identity);
            this.joinwh = jwHandler(this.left, this.right);
        }

        internal override Table SeekTableByIdentity(object identity)
        {
            var table = this.left.SeekTableByIdentity(identity);
            if (table != null)
                return table;
            return this.right.SeekTableByIdentity(identity);
        }

        internal override C CreateInstance()
        {
            var obj = this.mapper(this.left.CreateInstance(), this.right.CreateInstance());
            return obj;
        }

        internal override C Map(DbDataReader reader)
        {
            var obj= this.mapper(this.left.Map(reader), this.right.Map(reader));
            return obj;
        }

        protected internal override void InitParseSqlContext(ParseSqlContext context, bool initLeftTable = false)
        {
            context.WhereNode = this.AddWhereNode(context.WhereNode, this.whereNode);
            context.Having = this.AddWhereNode(context.Having, this.havingNode);
            context.JoinNode.Insert(0,new Node.JoinNode(this.right, this.joinwh, JOIN.Inner));
            context.GroupbyNode.InsertRange(0, this.groupbyNode);
            context.SelectNode.InsertRange(0, this.selectNode);
            context.OrderbyNode.InsertRange(0, this.orderbyNode);
            this.right.InitParseSqlContext(context, false);
            this.left.InitParseSqlContext(context, true);
        }
    }
}
