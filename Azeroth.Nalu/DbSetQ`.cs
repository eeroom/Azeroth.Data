﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public class DbSetQ<T>:DbSet<T>
    {

        public DbSetQ(IQuery handler)
        {
            this.queryHandler = handler;
        }

        protected IQuery queryHandler;
        protected IQuery cteSource;

        string viewName;//也可能是表名称，泛指和class名称不一致的表名称
        public DbSetQ<T> From(string name)
        {
            this.viewName = name;
            this.nameHandler = context => this.viewName;
            return this;
        }

        public DbSetQ<T> From(Query source)
        {
            //因为这个时候 dbset.SQLResolver.NameForCTE这个值还没有，所有需要解析的时候去取，
            //所有把Name定义成委托，解析开始后再去取这个名称
            this.cteSource = source;
            this.nameHandler = context => this.cteSource.NameForCTE;
            this.queryHandler.CTEHandlers.Add(source);
            return this;
        }


        public JoinNode LeftJoin<B>(DbSetQ<B> dbset)
        {
            var tmp = new JoinNode(JOIN.Left, dbset);
            this.queryHandler.JoinNode.Add(tmp);
            return tmp;
        }
        public JoinNode RightJoin<B>(DbSetQ<B> dbset)
        {
            var tmp = new JoinNode(JOIN.Right, dbset);
            this.queryHandler.JoinNode.Add(tmp);
            return tmp;
        }
        public JoinNode InnerJoin<B>(DbSetQ<B> dbset)
        {
            var tmp = new JoinNode(JOIN.Inner, dbset);
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

        public override List<Column> Columns<S>(Expression<Func<T, S>> exp)
        {
            if (cteSource == null)
                return base.Columns(exp);
            //dbset同名称，col同名称
            string name = Type.GetTypeFromHandle(this.GetMetaInfo()).Name;//这里不用namehandler,因为这样会拿到他对于cte的名字，这里只需要他原本的表名称
            var lstName = Column.GetColumnNames(exp.Body);
            var lstmap= lstName.Select(x=>cteSource.SelectNodes.FirstOrDefault(a=>name.Equals(a.Column.Container.NameHandler(null))&&x.Equals(a.Column.ColumnName))).ToList();
            return lstName.Zip(lstmap, (x, y) => new Column(this, x, y)).ToList();
        }
    }
}