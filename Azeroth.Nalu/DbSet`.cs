using Azeroth.Nalu.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public class DbSet<T>:Table<T>
    {
        
        internal T identity { set; get; }

        protected List<SelectNode> selectNode { set; get; }

        protected List<Column> groupbyNode { set; get; }

        protected   List<OrderByNode> orderbyNode { set; get; }

        protected DbContext dbContext { set; get; }


        protected WhereNode whereNode { set; get; }

        protected WhereNode havingNode { set; get; }


        internal virtual Table SeekTableByIdentity(object identity)
        {
            if (identity == (object)this.identity)
                return this;
            return null;
        }

        internal DbSet(DbContext context,bool complexDbset=false)
        {
            this.dbContext = context;
            this.selectNode = new List<SelectNode>();
            this.groupbyNode = new List<Column>();
            this.orderbyNode = new List<OrderByNode>();
            if (!complexDbset)
                this.identity = this.CreateInstance();
        }

        protected int takerows { set; get; }

        protected int skiprows { set; get; }
        public DbSet<T> SkipTake(int skiprows,int takerows) {
            this.skiprows = skiprows;
            this.takerows = takerows;
            return this;
        }

        public Column<T, S> Col<S>(Expression<Func<T, S>> exp)
        {
            //join的where,having,orderby,group之后，需要确定这个col是属于哪个dbset
            var colmem = exp.Body as MemberExpression;
            if (colmem == null)
                throw new ArgumentException("不支持的表达式");
            var body = Expression.Convert(colmem.Expression, typeof(object));
            var identity = Expression.Lambda<Func<T, object>>(body, exp.Parameters[0]).Compile()(this.identity);
          
            var table = this.SeekTableByIdentity(identity);
            if (table == null)
                throw new ArgumentException("内部错误，通过identity无法找到所在table");
            var col = new Column<T,S>(table, colmem.Member.Name);
            return col;
        }

        

        public DbSet<T> Select<S>(Expression<Func<T,S>> exp) 
        {
            List<string> lstName = new List<string>();
            var colmem = exp.Body as MemberExpression;
            if (colmem != null)
                lstName.Add(colmem.Member.Name);
            else
            {
                var colmem2 = exp.Body as NewExpression;
                if (colmem2 != null)
                    lstName.AddRange(colmem2.Members.Select(x => x.Name));
            }
            if (lstName.Count < 1)
                throw new ArgumentException("不支持的表达式");
            var lst= lstName.Select(x => new Column(this, x))
                .Select(x => new SelectNode(x))
                .ToList();
            this.selectNode.AddRange(lst);
            return this; 
        }

        public DbSet<T> SelectCol(Func<DbSet<T>,Column> colHandler)
        {
            var col= colHandler(this);
            var selectNode = new SelectNode(col);
            this.selectNode.Add(selectNode);
            return this;
        }

        public DbSet<T> GroupBy<S>(Expression<Func<T, S>> exp)
        {
            List<string> lstName = new List<string>();
            var colmem = exp.Body as MemberExpression;
            if (colmem != null)
                lstName.Add(colmem.Member.Name);
            else
            {
                var colmem2 = exp.Body as NewExpression;
                if (colmem2 != null)
                    lstName.AddRange(colmem2.Members.Select(x => x.Name));
            }
            if (lstName.Count < 1)
                throw new ArgumentException("不支持的表达式");
            var lst = lstName.Select(x => new Column(this, x))
                .Select(x => new SelectNode(x))
                .ToList();
            
            return this;
        }

        public DbSet<T> OrderBy<S>(Order opt, Expression<Func<T, S>> exp)
        {
            List<string> lstName = new List<string>();
            var colmem = exp.Body as MemberExpression;
            if (colmem != null)
                lstName.Add(colmem.Member.Name);
            else
            {
                var colmem2 = exp.Body as NewExpression;
                if (colmem2 != null)
                    lstName.AddRange(colmem2.Members.Select(x => x.Name));
            }
            if (lstName.Count < 1)
                throw new ArgumentException("不支持的表达式");
            

            return this;
        }

        internal virtual T CreateInstance()
        {
            return System.Activator.CreateInstance<T>();
        }

        public DbSet<T> Where(Func<DbSet<T>,WhereNode> predicate)
        {
            var wh = predicate(this);
            if (this.whereNode == null)
                this.whereNode = wh;
            else
                this.whereNode = this.whereNode && wh;
            return this;
        }

        public DbSet<Tuple<T,B>> Join<B>(DbSet<B> right,Func<DbSet<T>,DbSet<B>, WhereJoinOnNode> on)
        {
            var dbset = new DbSetComplex<T, B, Tuple<T, B>>(this.dbContext, this, right, on, (x, y) => Tuple.Create(x, y));
            return dbset;
        }

        public DbSet<C> Join<B,C>(DbSet<B> right, Func<DbSet<T>, DbSet<B>, WhereJoinOnNode> on,Func<T,B,C> mapper)
        {
            var dbset = new DbSetComplex<T, B, C>(this.dbContext, this, right, on, mapper);
            return dbset;
        }

        internal virtual T Map(System.Data.Common.DbDataReader reader)
        {
            var obj = this.CreateInstance();
            this.selectNode.ForEach(x => DictMapHandlerInternal[x.Column.name].SetValueToInstance(obj, reader, x.index));
            return (T)obj;
        }

        public List<T> ToList()
        {
           var wrapper= ((IDbContext)this.dbContext).ToList(this.Map, this.InitParseSqlContext);
            return wrapper.Item1;
        }

        public List<M> ToList<M>(Func<T,M> select)
        {
            var wrapper = ((IDbContext)this.dbContext).ToList(x=>select(this.Map(x)), this.InitParseSqlContext);
            return wrapper.Item1;
        }

        public List<T> ToList(out int rowcount) {
            var wrapper = ((IDbContext)this.dbContext).ToList(this.Map, this.InitParseSqlContext);
            rowcount = wrapper.Item2;
            return wrapper.Item1;
        }

        public List<M> ToList<M>(Func<T, M> select, out int rowcount) {
            var wrapper = ((IDbContext)this.dbContext).ToList(x => select(this.Map(x)), this.InitParseSqlContext);
            rowcount = wrapper.Item2;
            return wrapper.Item1;
        }

        protected internal virtual void InitParseSqlContext(ParseSqlContext context,bool initLeftTable=false)
        {
            this.NameNick = "T" + context.NextTableIndex();
            context.Tables.Add(this);
            context.WhereNode = this.AddWhereNode(context.WhereNode, this.whereNode);
            context.Having = this.AddWhereNode(context.Having, this.havingNode);
            if (initLeftTable)
            {
                context.FromTable = this;
            }
          
            context.GroupbyNode.AddRange( this.groupbyNode);
            this.selectNode.ForEach(x => x.index = context.NextColIndex());
            this.selectNode.ForEach(x => x.nameNick = "c" + x.index.ToString());
            context.SelectNode.AddRange(this.selectNode);
            context.OrderbyNode.AddRange(this.orderbyNode);
            if(this.skiprows>=0 && this.takerows > 0) {
                context.Take = this.takerows;
                context.Skip = this.skiprows;
                context.SkipTake = true;
            }
        
        }

        protected WhereNode AddWhereNode(WhereNode left,WhereNode right)
        {
            if (left == null)
                return right;
            if (right == null)
                return left;
            return left && right;
        }
    }
}
