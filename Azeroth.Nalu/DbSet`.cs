using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public class DbSet<T>:Table<T>
    {

        public DbSet(IContainer container)
        {
            this.container = container;
        }

        protected IContainer container;
        protected IContainer subContainer;

        string viewName;//也可能是表名称，泛指和class名称不一致的表名称
        public DbSet<T> From(string name)
        {
            this.viewName = name;
            this.nameHandler = context => this.viewName;
            return this;
        }

        public DbSet<T> From(Container subContainer)
        {
            //因为这个时候 dbset.SQLResolver.NameForCTE这个值还没有，所有需要解析的时候去取，
            //所有把Name定义成委托，解析开始后再去取这个名称
            this.subContainer = subContainer;
            this.nameHandler = context => this.subContainer.NameForCTE;
            this.container.CTEContainer.Add(subContainer);
            return this;
        }


        public NodeJOIN LeftJoin<B>(DbSet<B> dbset)
        {
            var tmp = new NodeJOIN(JOIN.Left, dbset);
            this.container.JoinNode.Add(tmp);
            return tmp;
        }
        public NodeJOIN RightJoin<B>(DbSet<B> dbset)
        {
            var tmp = new NodeJOIN(JOIN.Right, dbset);
            this.container.JoinNode.Add(tmp);
            return tmp;
        }
        public NodeJOIN InnerJoin<B>(DbSet<B> dbset)
        {
            var tmp = new NodeJOIN(JOIN.Inner, dbset);
            this.container.JoinNode.Add(tmp);
            return tmp;
        }

        public override Column<T, S> Col<S>(Expression<Func<T, S>> exp)
        {
            if (subContainer == null)
                return base.Col(exp);
            //dbset同名称，col同名称
            string name = Type.GetTypeFromHandle(this.GetMetaInfo()).Name;//这里不用namehandler,因为这样会拿到他对于cte的名字，这里只需要他原本的表名称
            var colName = Column.GetName(exp.Body);
            var mapColumn = subContainer.SelectNode.FirstOrDefault(x => name.Equals(x.Column.Table.NameHandler(null)) && colName.Equals(x.Column.ColumnName));
            return new Column<T, S>(this, exp, mapColumn);
        }

        public override List<Column> Cols<S>(Expression<Func<T, S>> exp)
        {
            if (subContainer == null)
                return base.Cols(exp);
            //dbset同名称，col同名称
            string name = Type.GetTypeFromHandle(this.GetMetaInfo()).Name;//这里不用namehandler,因为这样会拿到他对于cte的名字，这里只需要他原本的表名称
            var lstName = Column.GetNameCollection(exp.Body);
            var lstmap= lstName.Select(x=>subContainer.SelectNode.FirstOrDefault(a=>name.Equals(a.Column.Table.NameHandler(null))&&x.Equals(a.Column.ColumnName))).ToList();
            return lstName.Zip(lstmap, (x, y) => new Column(this, x, y)).ToList();
        }
    }
}
