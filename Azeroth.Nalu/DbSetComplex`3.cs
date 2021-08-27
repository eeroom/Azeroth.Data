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
            this.left.InitParseSqlContext(context, true);
            this.right.InitParseSqlContext(context, false);
            context.WhereNode = this.AddWhereNode(context.WhereNode, this.whereNode);
            context.Having = this.AddWhereNode(context.Having, this.havingNode);
            context.JoinNode.Add(new Node.JoinNode(this.right, this.joinwh, JoinOpt.Inner));
            context.GroupbyNode.AddRange( this.groupbyNode);
            context.SelectNode.AddRange(this.selectNode);
            context.OrderbyNode.AddRange(this.orderbyNode);
            if (this.skiprows >= 0 && this.takerows > 0) {
                context.Take = this.takerows;
                context.Skip = this.skiprows;
                context.SkipTake = true;
            }

        }
    }
}
