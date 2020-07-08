using Azeroth.Nalu.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public   class Query:IQuery
    {
        /// <summary>
        /// 总记录数的辅助列
        /// </summary>
        protected const string CountAuxiliaryColumn = "_CountAuxiliaryColumn";

        public Query(IDbContext contex)
        {
            this.dbContex = contex;
        }
        protected List<ISelectNode> lstSelect = new List<ISelectNode>();
        protected List<INodeBase> lstJoin = new List<INodeBase>();
        public WhereNode WHERE { set; get; }
        /// <summary>
        /// 筛选条件
        /// </summary>
        //public PredicateNode WH { set; get; }
        ///// <summary>
        ///// Having条件
        ///// </summary>
        //public PredicateNode Having { set; get; }

        protected List<IColumn> lstGroupBy = new List<IColumn>();
        public WhereNode Having { set; get; }
        protected List<INodeBase> lstOrderBy = new List<INodeBase>();
        protected List<ITable> lstItem=new List<ITable>();
        protected List<IQuery> lstSubContainer = new List<IQuery>();
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
        public Query Distinct()
        {
            this.isDistinct = true;
            return this;
        }

        public Query Take(int index, int size)
        {
            if (this.top > 0)
                throw new ArgumentException("top后不能进行分页查询");
            if (index * size <= 0)
                throw new ArgumentException("分页参数必须为正数");
            this.pageIndex = index;
            this.pageSize = size;
            return this;
        }

        //public DbSet<B> Set<B>()
        //{
        //     DbSet<B> tmp=new DbSet<B>(this);
        //     this.lstDbSet.Add(tmp);
        //     return tmp;
        //}

        //public void Where(PredicateNode predicate)
        //{

        //    this.wherePredicate=predicate;
        //}

        //public void Having(PredicateNode predicate)
        //{
        //    this.havingPredicate = predicate;
        //}

        //public Container Select(Column col)
        //{
        //    var tmp = new NodeSelect(col);
        //    ((INodeSelect)tmp).Column.Table.SelectNode.Add(tmp);
        //    this.lstNodeSelect.Add(tmp);
        //    return this;
        //}



        //public Container Select(IList<Column> cols)
        //{
        //    var tmp = cols.Select(x => new NodeSelect(x)).ToList();
        //    tmp.ForEach(x=>((INodeSelect)x).Column.Table.SelectNode.Add(x));
        //    this.lstNodeSelect.AddRange(tmp);
        //    return this;
        //}

       


        string IQuery.NameForCTE
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

        List<IQuery> IQuery.SubContainer
        {
            get { return this.lstSubContainer; }
        }

        List<INodeBase> IQuery.JOIN
        {
            get { return this.lstJoin; }
        }
        List<ISelectNode> IQuery.Select
        {
            get
            {
                return this.lstSelect;
            }
        }

       
        string IResolver.ToSQL(ResolveContext context)
        {
            return this.ToSQL(context);
        }

        protected virtual string ResolveCTE(ResolveContext context, IList<IQuery> lstCTEHandler)
        {
            if (lstCTEHandler.Count < 1 || !context.CanCTE)
                return string.Empty;
            List<IQuery> lstCTEHandler2 = new List<IQuery>();
            ResolveCTE(lstCTEHandler, lstCTEHandler2);
            lstCTEHandler2.ForEach(x => x.NameForCTE = "W" + context.NextSetIndex().ToString());
            context.CanCTE = false;
            var lst2 = lstCTEHandler2.Select(x => string.Format("{0} AS ({1})\r\n", x.NameForCTE, x.ToSQL(context))).ToList();
            return "WITH " + string.Join(",\r\n", lst2);
        }

        /// <summary>
        /// 把解析器的层层包含关系处理成扁平的集合
        /// </summary>
        /// <param name="resolvers"></param>
        /// <param name="resolversWithFlat"></param>
        protected virtual void ResolveCTE(IList<IQuery> resolvers, IList<IQuery> resolversWithFlat)
        {
            foreach (var resolver in resolvers)
            {
                if (resolver.SubContainer.Count > 0)
                    ResolveCTE(resolver.SubContainer, resolversWithFlat);
                if (!resolversWithFlat.Contains(resolver))
                    resolversWithFlat.Add(resolver);
            }
        }

        protected virtual string ResolveNodeSelect(ResolveContext context, IList<ISelectNode> lstNode)
        {
            if (lstNode.Count < 1)
                throw new ArgumentException("需要指定查询的列");
            var lststr = lstNode.Select(x => x.ToSQL(null));
            return string.Join(",", lststr);
        }

        protected virtual string ResolveNodeJOIN(ResolveContext context, IList<INodeBase> lstNode)
        {
            var lststr = lstNode.Select(x => x.ToSQL(context));
            return string.Join(" ", lststr).Trim(',');//","用于处理UnKown 的连接类型，只是把表放在一起
        }

        protected virtual string ResolverNodeGroupBy(ResolveContext context, IList<IColumn> lstNode)
        {
            var lst = lstNode.Select(x => x.ToSQL(context));
            var tmp = string.Join(",", lst);
            if (tmp.Length > 0)
                tmp = " \r\nGROUP BY " + tmp;
            return tmp;
        }

        protected virtual string ResolveNodeOrderBy(ResolveContext context, IList<INodeBase> lstNode)
        {
            var lststr = lstNode.Select(x => x.ToSQL(context));
            string strOrder = string.Join(",", lststr);
            if (strOrder.Length > 0)
                strOrder = " \r\nORDER BY " + strOrder;
            return strOrder;
        }

        public  List<S> ToList<S>()
        {
            return this.dbContex.ToList(this,x=>(S)x[0]);
        }

        public List<S> ToList<S>(out int rowscount)
        {
            var lst = this.dbContex.ToList(this, x => (S)x[0]);
            rowscount = this.rowsCount;
            if (pageIndex * pageSize <= 0)
                rowscount = lst.Count;
            return lst;
        }

        public  List<S> ToList<S>(Func<object[], S> transfer)
        {
            return this.dbContex.ToList(this, transfer);
        }

        public  List<S> ToList<S>(Func<object[], S> transfer, out int rowscount)
        {
            var lst = this.dbContex.ToList(this, transfer);
            rowscount = this.rowsCount;
            if (pageIndex * pageSize <= 0)
                rowscount = lst.Count;
            return lst;
        }

        //public string ToString(Azeroth.Nalu.ResovleContext context)
        //{
        //    return this.ToSQL(context);
        //}

        public Query Top(int top)
        {
            if (this.pageIndex * this.pageSize > 0)
                throw new ArgumentException("分页后不能再进行top查询");
            if (top <= 0)
                throw new ArgumentException("top必须大于0");
            this.top = top;
            return this;
        }

        List<T> IQuery.ToList<H,T>(Func<object[], T> transfer,string cnnstr)
        {
            using (H cnn = new H())
            {
                cnn.ConnectionString = cnnstr;
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    var context = this.dbContex.GetResolveContext();
                    this.ComandText = this.ToSQL(context);
                    cmd.CommandText = this.ComandText;
                    this.DbParameters = context.Parameters;
                    if (context.Parameters.Count > 0)
                        cmd.Parameters.AddRange(context.Parameters.ToArray());
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                            return ToList(transfer, reader);
                    }
                    if (this.pageIndex * this.pageSize <= 0)
                        return new List<T>();//没有分页，或者第一页都没有数据
                    this.pageIndex = 1;//回到第一页
                    context = this.dbContex.GetResolveContext();
                    this.ComandText = this.ToSQL(context);
                    cmd.CommandText = this.ComandText;
                    this.DbParameters = context.Parameters;
                    cmd.Parameters.Clear();
                    if (context.Parameters.Count > 0)
                        cmd.Parameters.AddRange(context.Parameters.ToArray());
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return new List<T>();//第一页都没数据
                        var lst = ToList(transfer, reader);
                        if (this.rowsCount <= this.pageSize)//总行数只够第一页
                            return lst;
                        this.pageIndex = (int)Math.Ceiling(1.0 * rowsCount / pageSize);//跳到最后一页
                    }
                    context = this.dbContex.GetResolveContext();
                    this.ComandText = this.ToSQL(context);
                    cmd.CommandText = this.ComandText;
                    this.DbParameters = context.Parameters;
                    cmd.Parameters.Clear();
                    if (context.Parameters.Count > 0)
                        cmd.Parameters.AddRange(context.Parameters.ToArray());
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                            return ToList(transfer, reader);
                        throw new ArgumentException("分页处理发生异常");
                    }
                }
            }
        }

        List<T> ToList<T>(Func<object[], T> transfer, System.Data.Common.DbDataReader reader)
        {
            List<T> lstEntity = new List<T>();
            object[] values = new object[this.lstItem.Count];
            DictionaryWrapper<string, IMapHandler> dict;
            this.lstItem.ForEach(dbset => dbset.Select.ForEach(col => col.ColIndex = reader.GetOrdinal(col.ColumnNameNick)));
            reader.Read();
            if (this.pageIndex * this.pageSize > 0)
            {//分页特别处理
                this.rowsCount = System.Convert.ToInt32(reader[CountAuxiliaryColumn]);
                lstEntity.Capacity = this.pageSize;
            }
            for (int i = 0; i < this.lstItem.Count; i++)
            {
                values[i] = lstItem[i].CreateInstance(lstItem[i].Select.Count <= 0);//如果没有选择任何列，就返回null实例，避免浪费
                dict = lstItem[i].DictMapHandler;
                lstItem[i].Select.ForEach(col => dict[col.Column.ColumnName].SetValueToInstance(values[i], reader, col.ColIndex));
            }
            lstEntity.Add(transfer(values));
            while (reader.Read())
            {
                for (int i = 0; i < lstItem.Count; i++)
                {
                    values[i] = lstItem[i].CreateInstance(lstItem[i].Select.Count <= 0);//如果没有选择任何列，就返回null，避免浪费
                    lstItem[i].Select.ForEach(col => lstItem[i].DictMapHandler[col.Column.ColumnName].SetValueToInstance(values[i], reader, col.ColIndex));
                }
                lstEntity.Add(transfer(values));
            }
            return lstEntity;
        }


        protected virtual string ToSQL(ResolveContext context)
        {
            string strWithAS = ResolveCTE(context, this.lstSubContainer);
            if (!string.IsNullOrEmpty(strWithAS))
                strWithAS = strWithAS + " \r\n";
            this.lstItem.ForEach(x => x.NameNick = "T" + context.NextSetIndex().ToString());//表的别名
            //因为会出现重复的列名，所以要使用别名，比如表1和表2都使用A列
            this.lstSelect.GroupBy(x => x.Column.ColumnName, (k, v) => v.ToList()).Where(v => v.Count > 1).ToList()
                .ForEach(x => x.ForEach(a => a.ColumnNameNick = a.Column.ColumnName + context.NextColIndex().ToString()));
            string strCol = ResolveNodeSelect(context, this.lstSelect);//查询的列
            string strfrom = this.lstItem[0].NameHandler(context) + " AS " + this.lstItem[0].NameNick;
            string strjn = ResolveNodeJOIN(context, this.lstJoin);
            string strwhere = ResolveNodeWhere(context, this.WHERE, "WHERE");
            string strgroup = ResolverNodeGroupBy(context, this.lstGroupBy);
            string strhaving = ResolveNodeWhere(context, this.Having, "HAVING");
            string strOrder = ResolveNodeOrderBy(context, this.lstOrderBy);//排序
            if (this.pageIndex * this.pageSize <= 0)//不分页
                return string.Format("{0}SELECT {1} {2} {3} \r\nFROM {4} {5} {6} {7} {8} {9}", strWithAS
                    , this.isDistinct ? "DISTINCT" : string.Empty
                    , top > 0 ? "TOP " + top.ToString() : string.Empty
                    , strCol
                    , strfrom, strjn, strwhere, strgroup, strhaving, strOrder);
            string tmp = string.Format("SELECT {7} {0},ROW_NUMBER() OVER({3}) AS theRowIndex FROM {1} {2} {4} {5} {6}", strCol
                , strfrom, strjn, strOrder, strwhere, strgroup, strhaving
                , this.isDistinct ? "DISTINCT" : string.Empty);
            int numEnd = this.pageIndex * this.pageSize;
            int numStart = numEnd + 1 - this.pageSize;
            if (string.IsNullOrEmpty(strWithAS))
                return string.Format(@"WITH HTT AS ({0}
                                                     ),HBB AS (
                                                     SELECT COUNT(0) AS {3} FROM HTT)
                                                     SELECT HTT.*,HBB.* FROM HTT,HBB WHERE HTT.theRowIndex BETWEEN {1} AND {2}", tmp
                                                         , numStart.ToString()
                                                         , numEnd.ToString()
                                                         , CountAuxiliaryColumn);
            else
                return string.Format(@"{1},HTT AS ({0}
                                                        ),HBB AS (SELECT COUNT(0) AS {4} FROM HTT)
                                                        SELECT HTT.*,HBB.* FROM HTT,HBB WHERE HTT.theRowIndex BETWEEN {2} AND {3}", tmp
                                                         , strWithAS
                                                          , numStart.ToString()
                                                         , numEnd.ToString()
                                                         , CountAuxiliaryColumn);
        }

        protected string ResolveNodeWhere(ResolveContext context, WhereNode lstNode, string verb)
        {
            string sqlstr = string.Empty;
            if (lstNode != null)
                sqlstr = ((IResolver)lstNode).ToSQL(context);//筛选条件
            if (!string.IsNullOrEmpty(sqlstr))
                sqlstr =string.Format(" \r\n{0} {1}",verb,sqlstr);
            return sqlstr;
        }


        List<ITable> IQuery.Items
        {
            get { return this.lstItem; }
        }

        List<IColumn> IQuery.GroupBy
        {
            get { return this.lstGroupBy; }
        }

        List<INodeBase> IQuery.OrderBy
        {
            get { return this.lstOrderBy; }
        }
    }
}
