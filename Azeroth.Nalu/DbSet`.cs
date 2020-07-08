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

        public DbSet(IQuery container)
        {
            this.container = container;
            container.Items.Add(this);
                //DbSet<B> tmp = new DbSet<B>(this);
                //this.lstDbSet.Add(tmp);
                //return tmp;
        }

        protected IQuery container;
        protected IQuery subContainer;

        string viewName;//也可能是表名称，泛指和class名称不一致的表名称
        public DbSet<T> From(string name)
        {
            this.viewName = name;
            this.nameHandler = context => this.viewName;
            return this;
        }

        public DbSet<T> From(Query subContainer)
        {
            //因为这个时候 dbset.SQLResolver.NameForCTE这个值还没有，所有需要解析的时候去取，
            //所有把Name定义成委托，解析开始后再去取这个名称
            this.subContainer = subContainer;
            this.nameHandler = context => this.subContainer.NameForCTE;
            this.container.SubContainer.Add(subContainer);
            return this;
        }


        public JoinNode Join<B>(DbSet<B> dbset,JOIN opt)
        {
            if (dbset.container != this.container)
                throw new ArgumentException("必须同一container下的dbset才可以进行join");
            var tmp = new JoinNode(opt, dbset);
            this.container.JOIN.Add(tmp);
            return tmp;
        }
        //public NodeJOIN RightJoin<B>(DbSet<B> dbset)
        //{
        //    if (dbset.container != this.container)
        //        throw new ArgumentException("必须同一container下的dbset才可以进行join");
        //    var tmp = new NodeJOIN(JOIN.Right, dbset);
        //    this.container.JOIN.Add(tmp);
        //    return tmp;
        //}
        //public NodeJOIN InnerJoin<B>(DbSet<B> dbset)
        //{
        //    if (dbset.container != this.container)
        //        throw new ArgumentException("必须同一container下的dbset才可以进行join");
        //    var tmp = new NodeJOIN(JOIN.Inner, dbset);
        //    this.container.JOIN.Add(tmp);
        //    return tmp;
        //}

        public override Column<T, S> Col<S>(Expression<Func<T, S>> exp)
        {
            if (subContainer == null)
                return base.Col(exp);
            //dbset同名称，col同名称
            string name = Type.GetTypeFromHandle(this.GetMetaInfo()).Name;//这里不用namehandler,因为这样会拿到他对于cte的名字，这里只需要他原本的表名称
            var colName = Column.GetName(exp.Body);
            var mapColumn = subContainer.Select.FirstOrDefault(x => name.Equals(x.Column.Table.NameHandler(null)) && colName.Equals(x.Column.ColumnName));
            return new Column<T, S>(this, exp, mapColumn);
        }

        protected override List<Column> Cols<S>(Expression<Func<T, S>> exp)
        {
            if (subContainer == null)
                return base.Cols(exp);
            //dbset同名称，col同名称
            string name = Type.GetTypeFromHandle(this.GetMetaInfo()).Name;//这里不用namehandler,因为这样会拿到他对于cte的名字，这里只需要他原本的表名称
            var lstName = Column.GetNameCollection(exp.Body);
            var lstmap= lstName.Select(x=>subContainer.Select.FirstOrDefault(a=>name.Equals(a.Column.Table.NameHandler(null))&&x.Equals(a.Column.ColumnName))).ToList();
            return lstName.Zip(lstmap, (x, y) => new Column(this, x, y)).ToList();
        }

        public DbSet<T> Select<S>(Expression<Func<T,S>> exp) 
        {
            List<Column> cols = Cols(exp);
            var tmp = cols.Select(x => new SelectNode(x)).ToList();
            tmp.ForEach(x=>this.lstSelect.Add(x));
            tmp.ForEach(x => ((ISelectNode)x).Column.Table.Select.Add(x));
            this.container.Select.AddRange(tmp);
            return this; 
        }

        public DbSet<T> GroupBy<S>(Expression<Func<T, S>> exp)
        {
            var lstcol=this.Cols(exp);
            this.container.GroupBy.AddRange(lstcol);
            return this;
        }

        public DbSet<T> OrderBy<S>(Order opt, Expression<Func<T, S>> exp)
        {
            var lstcol = this.Cols(exp).Select(x=>new OrderByNode(x,opt));
            this.container.OrderBy.AddRange(lstcol);
            return this;
        }
    }
}
