using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public abstract  class Container:IContainer
    {
        /// <summary>
        /// 总记录数的辅助列
        /// </summary>
        protected const string CountAuxiliaryColumn = "_CountAuxiliaryColumn";

        public Container(IDbContext contex)
        {
            this.dbContex = contex;
        }
        protected List<INodeSelect> lstNodeSelect = new List<INodeSelect>();
        protected List<INode> lstJoinNode = new List<INode>();
        public NodeWhere Where { set; get; }
        /// <summary>
        /// 筛选条件
        /// </summary>
        //public PredicateNode WH { set; get; }
        ///// <summary>
        ///// Having条件
        ///// </summary>
        //public PredicateNode Having { set; get; }

        protected List<IColumn> lstNodeGroupBy = new List<IColumn>();
        public NodeWhere Having { set; get; }
        protected List<INode> lstNodeOrderBy = new List<INode>();
        protected List<ITable> lstDbSet=new List<ITable>();
        protected List<IContainer> lstCTEHandler = new List<IContainer>();
        protected string nameForCTE;
        protected bool isDistinct;

        protected int pageIndex;
        protected int pageSize;
        protected int rowsCount;
        protected int top;
        protected IDbContext dbContex;

        public string ComandText { get; protected set; }
        public List<System.Data.Common.DbParameter> DbParameters { get; protected set; }

        /// <summary>
        /// Distinct去重
        /// </summary>
        /// <returns></returns>
        public Container Distinct()
        {
            this.isDistinct = true;
            return this;
        }

        public Container Take(int index, int size)
        {
            if (this.top > 0)
                throw new ArgumentException("top后不能进行分页查询");
            if (index * size <= 0)
                throw new ArgumentException("分页参数必须为正数");
            this.pageIndex = index;
            this.pageSize = size;
            return this;
        }

        public DbSet<B> Set<B>()
        {
             DbSet<B> tmp=new DbSet<B>(this);
             this.lstDbSet.Add(tmp);
             return tmp;
        }

        //public void Where(PredicateNode predicate)
        //{

        //    this.wherePredicate=predicate;
        //}

        //public void Having(PredicateNode predicate)
        //{
        //    this.havingPredicate = predicate;
        //}

        public Container Select(Column col)
        {
            var tmp = new NodeSelect(col);
            ((INodeSelect)tmp).Column.Container.SelectNodes.Add(tmp);
            this.lstNodeSelect.Add(tmp);
            return this;
        }



        public Container Select(IList<Column> cols)
        {
            var tmp = cols.Select(x => new NodeSelect(x)).ToList();
            tmp.ForEach(x=>((INodeSelect)x).Column.Container.SelectNodes.Add(x));
            this.lstNodeSelect.AddRange(tmp);
            return this;
        }

        public Container GroupBy(Column col)
        {
            this.lstNodeGroupBy.Add(col);
            return this;
        }

        public Container GroupBy(IList<Column> cols)
        {
            this.lstNodeGroupBy.AddRange(cols);
            return this;
        }

        public Container OrderBy(Column col,Order opt)
        {
            this.lstNodeOrderBy.Add(new NodeOrderBy(col, opt));
            return this;
        }

        public Container OrderBy(IList<Column> cols,Order opt)
        {
            this.lstNodeOrderBy.AddRange(cols.Select(x => new NodeOrderBy(x, opt)));
            return this;
        }


        string IContainer.NameForCTE
        {
            get
            {
                return this.nameForCTE;
            }
            set
            {
                this.nameForCTE=value;
            }
        }

        List<IContainer> IContainer.CTEHandlers
        {
            get { return this.lstCTEHandler; }
        }

        List<INode> IContainer.JoinNode
        {
            get { return this.lstJoinNode; }
        }
        List<INodeSelect> IContainer.SelectNodes
        {
            get
            {
                return this.lstNodeSelect;
            }
        }

       
        string IContainer.GetCommandText(ResovleContext context)
        {
            return this.GetCommandText(context);
        }

        protected virtual string ResolveCTE(ResovleContext context, IList<IContainer> lstCTEHandler)
        {
            if (lstCTEHandler.Count < 1 || !context.CanCTE)
                return string.Empty;
            List<IContainer> lstCTEHandler2 = new List<IContainer>();
            ResolveCTE(lstCTEHandler, lstCTEHandler2);
            lstCTEHandler2.ForEach(x => x.NameForCTE = "W" + context.NextSetIndex().ToString());
            context.CanCTE = false;
            var lst2 = lstCTEHandler2.Select(x => string.Format("{0} AS ({1})\r\n", x.NameForCTE, x.GetCommandText(context))).ToList();
            return "WITH " + string.Join(",\r\n", lst2);
        }

        /// <summary>
        /// 把解析器的层层包含关系处理成扁平的集合
        /// </summary>
        /// <param name="resolvers"></param>
        /// <param name="resolversWithFlat"></param>
        protected virtual void ResolveCTE(IList<IContainer> resolvers, IList<IContainer> resolversWithFlat)
        {
            foreach (var resolver in resolvers)
            {
                if (resolver.CTEHandlers.Count > 0)
                    ResolveCTE(resolver.CTEHandlers, resolversWithFlat);
                if (!resolversWithFlat.Contains(resolver))
                    resolversWithFlat.Add(resolver);
            }
        }

        protected virtual string ResolveNodeSelect(ResovleContext context, IList<INodeSelect> lstNode)
        {
            if (lstNode.Count < 1)
                throw new ArgumentException("需要指定查询的列");
            var lststr = lstNode.Select(x => x.ToSQL(null));
            return string.Join(",", lststr);
        }

        protected virtual string ResolveNodeJOIN(ResovleContext context, IList<INode> lstNode)
        {
            var lststr = lstNode.Select(x => x.ToSQL(context));
            return string.Join(" ", lststr).Trim(',');//","用于处理UnKown 的连接类型，只是把表放在一起
        }

        protected virtual string ResolverNodeGroupBy(ResovleContext context, IList<IColumn> lstNode)
        {
            var lst = lstNode.Select(x => x.ToSQL(context));
            var tmp = string.Join(",", lst);
            if (tmp.Length > 0)
                tmp = " \r\nGROUP BY " + tmp;
            return tmp;
        }

        protected virtual string ResolveNodeOrderBy(ResovleContext context, IList<INode> lstNode)
        {
            var lststr = lstNode.Select(x => x.ToSQL(context));
            string strOrder = string.Join(",", lststr);
            if (strOrder.Length > 0)
                strOrder = " \r\nORDER BY " + strOrder;
            return strOrder;
        }

        public  List<S> ToList<S>()
        {
            return this.dbContex.ExecuteQuery(this,x=>(S)x[0]);
        }

        public List<S> ToList<S>(out int rowscount)
        {
            var lst = this.dbContex.ExecuteQuery(this, x => (S)x[0]);
            rowscount = this.rowsCount;
            if (pageIndex * pageSize <= 0)
                rowscount = lst.Count;
            return lst;
        }

        public List<Tuple<A,B>> ToList<A,B>()
        {
            return this.dbContex.ExecuteQuery(this, x =>Tuple.Create((A)x[0],(B)x[1]));
        }

        public List<Tuple<A, B>> ToList<A, B>(out int rowscount)
        {
            var lst= this.dbContex.ExecuteQuery(this, x => Tuple.Create((A)x[0], (B)x[1])); 
            rowscount = this.rowsCount;
            if (pageIndex * pageSize <= 0)
                rowscount = lst.Count;
            return lst;
        }

        public  List<S> ToList<S>(Func<object[], S> transfer)
        {
            return this.dbContex.ExecuteQuery(this, transfer);
        }

        public  List<S> ToList<S>(Func<object[], S> transfer, out int rowscount)
        {
            var lst = this.dbContex.ExecuteQuery(this, transfer);
            rowscount = this.rowsCount;
            if (pageIndex * pageSize <= 0)
                rowscount = lst.Count;
            return lst;
        }

        public string ToString(Azeroth.Nalu.ResovleContext context)
        {
            return this.GetCommandText(context);
        }

        public Container Top(int top)
        {
            if (this.pageIndex * this.pageSize > 0)
                throw new ArgumentException("分页后不能再进行top查询");
            if (top <= 0)
                throw new ArgumentException("top必须大于0");
            this.top = top;
            return this;
        }

        List<T> IContainer.Execute<H,T>(Func<object[], T> transfer,string cnnstr)
        {
            using (H cnn = new H())
            {
                cnn.ConnectionString = cnnstr;
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    var context = this.dbContex.GetResolvContext();
                    this.ComandText = this.GetCommandText(context);
                    cmd.CommandText = this.ComandText;
                    this.DbParameters = context.Parameters;
                    if (context.Parameters.Count > 0)
                        cmd.Parameters.AddRange(context.Parameters.ToArray());
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                            return Execute(transfer, reader);
                    }
                    if (this.pageIndex * this.pageSize <= 0)
                        return new List<T>();//没有分页，或者第一页都没有数据
                    this.pageIndex = 1;//回到第一页
                    context = this.dbContex.GetResolvContext();
                    this.ComandText = this.GetCommandText(context);
                    cmd.CommandText = this.ComandText;
                    this.DbParameters = context.Parameters;
                    cmd.Parameters.Clear();
                    if (context.Parameters.Count > 0)
                        cmd.Parameters.AddRange(context.Parameters.ToArray());
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return new List<T>();//第一页都没数据
                        var lst = Execute(transfer, reader);
                        if (this.rowsCount <= this.pageSize)//总行数只够第一页
                            return lst;
                        this.pageIndex = (int)Math.Ceiling(1.0 * rowsCount / pageSize);//跳到最后一页
                    }
                    context = this.dbContex.GetResolvContext();
                    this.ComandText = this.GetCommandText(context);
                    cmd.CommandText = this.ComandText;
                    this.DbParameters = context.Parameters;
                    cmd.Parameters.Clear();
                    if (context.Parameters.Count > 0)
                        cmd.Parameters.AddRange(context.Parameters.ToArray());
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                            return Execute(transfer, reader);
                        throw new ArgumentException("分页处理发生异常");
                    }
                }
            }
        }

        List<T> Execute<T>(Func<object[], T> transfer, System.Data.Common.DbDataReader reader)
        {
            List<T> lstEntity = new List<T>();
            object[] values = new object[this.lstDbSet.Count];
            DictionaryWrapper<string, IMapHandler> dict;
            this.lstDbSet.ForEach(dbset => dbset.SelectNodes.ForEach(col => col.ColIndex = reader.GetOrdinal(col.ColumnNameNick)));
            reader.Read();
            if (this.pageIndex * this.pageSize > 0)
            {//分页特别处理
                this.rowsCount = System.Convert.ToInt32(reader[CountAuxiliaryColumn]);
                lstEntity.Capacity = this.pageSize;
            }
            for (int i = 0; i < this.lstDbSet.Count; i++)
            {
                values[i] = lstDbSet[i].CreateInstance(lstDbSet[i].SelectNodes.Count <= 0);//如果没有选择任何列，就返回null实例，避免浪费
                dict = lstDbSet[i].DictMapHandler;
                lstDbSet[i].SelectNodes.ForEach(col => dict[col.Column.ColumnName].SetValueToInstance(values[i], reader, col.ColIndex));
            }
            lstEntity.Add(transfer(values));
            while (reader.Read())
            {
                for (int i = 0; i < lstDbSet.Count; i++)
                {
                    values[i] = lstDbSet[i].CreateInstance(lstDbSet[i].SelectNodes.Count <= 0);//如果没有选择任何列，就返回null，避免浪费
                    lstDbSet[i].SelectNodes.ForEach(col => lstDbSet[i].DictMapHandler[col.Column.ColumnName].SetValueToInstance(values[i], reader, col.ColIndex));
                }
                lstEntity.Add(transfer(values));
            }
            return lstEntity;
        }


        protected abstract string GetCommandText(ResovleContext context);

        protected string ResolveNodeWhere(ResovleContext context, NodeWhere lstNode, string verb)
        {
            string sqlstr = string.Empty;
            if (lstNode != null)
                sqlstr = ((IConvertible)lstNode).ToSQL(context);//筛选条件
            if (!string.IsNullOrEmpty(sqlstr))
                sqlstr =string.Format(" \r\n{0} {1}",verb,sqlstr);
            return sqlstr;
        }
    }
}
