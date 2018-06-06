using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public class DbSet<T>:Container<T>
    {

        public DbSet(IDbSetContainer handler)
        {
            this.queryHandler = handler;
        }

        protected IDbSetContainer queryHandler;
        protected IDbSetContainer cteSource;

        string viewName;//也可能是表名称，泛指和class名称不一致的表名称
        public DbSet<T> From(string name)
        {
            this.viewName = name;
            this.nameHandler = context => this.viewName;
            return this;
        }

        public DbSet<T> From(DbSetContainer source)
        {
            //因为这个时候 dbset.SQLResolver.NameForCTE这个值还没有，所有需要解析的时候去取，
            //所有把Name定义成委托，解析开始后再去取这个名称
            this.cteSource = source;
            this.nameHandler = context => this.cteSource.NameForCTE;
            this.queryHandler.CTEHandlers.Add(source);
            return this;
        }


        public ComponentJOIN LeftJoin<B>(DbSet<B> dbset)
        {
            var tmp = new ComponentJOIN(JOIN.Left, dbset);
            this.queryHandler.JoinNode.Add(tmp);
            return tmp;
        }
        public ComponentJOIN RightJoin<B>(DbSet<B> dbset)
        {
            var tmp = new ComponentJOIN(JOIN.Right, dbset);
            this.queryHandler.JoinNode.Add(tmp);
            return tmp;
        }
        public ComponentJOIN InnerJoin<B>(DbSet<B> dbset)
        {
            var tmp = new ComponentJOIN(JOIN.Inner, dbset);
            this.queryHandler.JoinNode.Add(tmp);
            return tmp;
        }

        public override Column<T, S> Col<S>(Expression<Func<T, S>> exp)
        {
            if (cteSource == null)
                return base.Col(exp);
            //dbset同名称，col同名称
            string name = Type.GetTypeFromHandle(this.GetMetaInfo()).Name;//这里不用namehandler,因为这样会拿到他对于cte的名字，这里只需要他原本的表名称
            var colName = Column.GetColumnName(exp.Body);
            var mapColumn = cteSource.SelectNodes.FirstOrDefault(x => name.Equals(x.Column.Container.NameHandler(null)) && colName.Equals(x.Column.ColumnName));
            return new Column<T, S>(this, exp, mapColumn);
        }

        public override List<Column> Cols<S>(Expression<Func<T, S>> exp)
        {
            if (cteSource == null)
                return base.Cols(exp);
            //dbset同名称，col同名称
            string name = Type.GetTypeFromHandle(this.GetMetaInfo()).Name;//这里不用namehandler,因为这样会拿到他对于cte的名字，这里只需要他原本的表名称
            var lstName = Column.GetColumnNames(exp.Body);
            var lstmap= lstName.Select(x=>cteSource.SelectNodes.FirstOrDefault(a=>name.Equals(a.Column.Container.NameHandler(null))&&x.Equals(a.Column.ColumnName))).ToList();
            return lstName.Zip(lstmap, (x, y) => new Column(this, x, y)).ToList();
        }
    }
}
