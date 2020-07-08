using Azeroth.Nalu.Node;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class Query : IResolver
    {
        /// <summary>
        /// 总记录数的辅助列
        /// </summary>
        protected const string CountAuxiliaryColumn = "_CountAuxiliaryColumn";

        public Query(IDbContext contex)
        {
            this.dbContex = contex;
        }
        protected List<ISelectNode> lstSelectNode = new List<ISelectNode>();
        protected List<INodeBase> lstJoinNode = new List<INodeBase>();
        protected WhereNode whereNode;
        /// <summary>
        /// 筛选条件
        /// </summary>
        //public PredicateNode WH { set; get; }
        ///// <summary>
        ///// Having条件
        ///// </summary>
        //public PredicateNode Having { set; get; }

        protected List<IColumn> lstGroupByNode = new List<IColumn>();
        protected WhereNode havingNode;

        public Query Having(WhereNode predicate)
        {
            if (this.havingNode == null)
                this.havingNode = predicate;
            else
                this.havingNode = this.havingNode && predicate;
            return this;
        }

        public Query Where(WhereNode predicate)
        {
            if (this.whereNode == null)
                this.whereNode = predicate;
            else
                this.whereNode = this.whereNode && predicate;
            return this;
        }
        protected List<INodeBase> lstOrderBy = new List<INodeBase>();
        protected List<ITable> lstItem = new List<ITable>();
        //protected List<IQuery> lstSubContainer = new List<IQuery>();
        //protected string nameForCTE;
        protected bool isDistinct;

        protected int pageIndex;
        protected int pageSize;
        protected int rowsCount;
        protected int top;
        protected IDbContext dbContex;

        public string ComandText { get; protected set; }
        public ResolveContext Context { get; protected set; }

        /// <summary>
        /// Distinct去重
        /// </summary>
        /// <returns></returns>
        public Query Distinct()
        {
            this.isDistinct = true;
            return this;
        }


        //public JoinNode Join<B>(DbSet<B> dbset, JOIN opt) {

        //    if (dbset.query != this.query)
        //        throw new ArgumentException("必须同一container下的dbset才可以进行join");
        //    var tmp = new JoinNode(opt, dbset, (Query)this.query);
        //    this.query.JOIN.Add(tmp);
        //    return tmp;
        //}
        public DbSet<T> Set<T>()
        {
            var dbset= new DbSet<T>();
            this.lstItem.Add(dbset);
            this.fromeTable = this.fromeTable ?? dbset;
            return dbset;
            
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

        protected ITable fromeTable;

        public Query From<T>(DbSet<T> table)
        {
            this.fromeTable = table;
            return this;
        }

        public JoinNode Join<T>(DbSet<T> table,JOIN joinOpt)
        {
            var joinNode = new JoinNode(joinOpt, table,this);
            this.lstJoinNode.Add(joinNode);
            return joinNode;
        }

        //string IQuery.NameForCTE
        //{
        //    get
        //    {
        //        return this.nameForCTE;
        //    }
        //    set
        //    {
        //        this.nameForCTE = value;
        //    }
        //}

        //List<IQuery> IQuery.SubContainer
        //{
        //    get { return this.lstSubContainer; }
        //}

        //List<INodeBase> IQuery.JOIN
        //{
        //    get { return this.lstJoinNode; }
        //}
        //List<ISelectNode> IQuery.Select
        //{
        //    get
        //    {
        //        return this.lstSelectNode;
        //    }
        //}



        //protected virtual string ResolveCTE(ResolveContext context, IList<IQuery> lstCTEHandler)
        //{
        //    if (lstCTEHandler.Count < 1 || !context.CanCTE)
        //        return string.Empty;
        //    List<IQuery> lstCTEHandler2 = new List<IQuery>();
        //    ResolveCTE(lstCTEHandler, lstCTEHandler2);
        //    lstCTEHandler2.ForEach(x => x.NameForCTE = "W" + context.NextSetIndex().ToString());
        //    context.CanCTE = false;
        //    var lst2 = lstCTEHandler2.Select(x => string.Format("{0} AS ({1})\r\n", x.NameForCTE, x.ToSQL(context))).ToList();
        //    return "WITH " + string.Join(",\r\n", lst2);
        //}

        /// <summary>
        /// 把解析器的层层包含关系处理成扁平的集合
        /// </summary>
        /// <param name="resolvers"></param>
        /// <param name="resolversWithFlat"></param>
        //protected virtual void ResolveCTE(IList<IQuery> resolvers, IList<IQuery> resolversWithFlat)
        //{
        //    foreach (var resolver in resolvers)
        //    {
        //        if (resolver.SubContainer.Count > 0)
        //            ResolveCTE(resolver.SubContainer, resolversWithFlat);
        //        if (!resolversWithFlat.Contains(resolver))
        //            resolversWithFlat.Add(resolver);
        //    }
        //}

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

        public List<S> ToList<S>()
        {
            return this.ToList(x => (S)x[0], this.dbContex.Cnnstr);
        }

        public List<S> ToList<S>(out int rowscount)
        {
            var lst = this.ToList(x => (S)x[0], this.dbContex.Cnnstr);
            rowscount = this.rowsCount;
            if (pageIndex * pageSize <= 0)
                rowscount = lst.Count;
            return lst;
        }

        public List<S> ToList<S>(Func<object[], S> transfer)
        {
            return this.ToList(transfer, this.dbContex.Cnnstr);
        }

        public List<S> ToList<S>(Func<object[], S> transfer, out int rowscount)
        {
            var lst = this.ToList(transfer);
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

        List<T> ToList<T>(Func<object[], T> transfer, string cnnstr)
        {
            using (DbConnection cnn = this.dbContex.GetDbProviderFactory().CreateConnection())
            {
                cnn.ConnectionString = cnnstr;
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    var context = this.dbContex.GetResolveContext();
                    this.ComandText = this.ToSQL(context);
                    cmd.CommandText = this.ComandText;
                    this.Context = context;
                    if (context.Parameters.Count > 0)
                        cmd.Parameters.AddRange(context.Parameters.ToArray());
                    using (var reader = cmd.ExecuteReader())
                    {
                        return ToList(transfer, reader);
                    }

                }
            }
        }

        List<T> ToList<T>(Func<object[], T> transfer, System.Data.Common.DbDataReader reader)
        {

            List<T> lstEntity = new List<T>();
            if (!reader.HasRows)
                return lstEntity;
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
            //string strWithAS = ResolveCTE(context, this.lstSubContainer);
            //if (!string.IsNullOrEmpty(strWithAS))
            //    strWithAS = strWithAS + " \r\n";
            string strWithAS = string.Empty;
            this.lstItem.ForEach(x => x.NameNick = "T" + context.NextTableIndex().ToString());//表的别名
            //因为会出现重复的列名，所以要使用别名，比如表1和表2都使用A列
            this.lstSelectNode.GroupBy(x => x.Column.ColumnName, (k, v) => v.ToList()).Where(v => v.Count > 1).ToList()
                .ForEach(x => x.ForEach(a => a.ColumnNameNick = a.Column.ColumnName + context.NextColIndex().ToString()));
            string strCol = ResolveNodeSelect(context, this.lstSelectNode);//查询的列
            string strfrom = this.fromeTable.NameHandler(context) + " AS " + this.fromeTable.NameNick;
            string strjn = ResolveNodeJOIN(context, this.lstJoinNode);
            string strwhere = ResolveNodeWhere(context, this.whereNode, "WHERE");
            string strgroup = ResolverNodeGroupBy(context, this.lstGroupByNode);
            string strhaving = ResolveNodeWhere(context, this.havingNode, "HAVING");
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
                sqlstr = string.Format(" \r\n{0} {1}", verb, sqlstr);
            return sqlstr;
        }

        string IResolver.ToSQL(ResolveContext context)
        {
            return this.ToSQL(context);
        }


        //List<ITable> IQuery.Items
        //{
        //    get { return this.lstItem; }
        //}

        //List<IColumn> IQuery.GroupBy
        //{
        //    get { return this.lstGroupByNode; }
        //}

        //List<INodeBase> IQuery.OrderBy
        //{
        //    get { return this.lstOrderBy; }
        //}
    }
}
