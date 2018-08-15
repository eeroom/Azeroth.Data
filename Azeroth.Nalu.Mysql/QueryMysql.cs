using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class QueryMysql : Query
    {
        public QueryMysql(IDbContext dbcontext):base(dbcontext)
        {

        }
        protected override string GetCommandText(ResovleContext context)
        {
            string strWithAS = ResolveCTE(context, this.lstCTEHandler);
            if (!string.IsNullOrEmpty(strWithAS))
                strWithAS = strWithAS + " \r\n";
            this.lstDbSet.ForEach(x => x.NameNick = "T" + context.NextSetIndex().ToString());//表的别名
            //因为会出现重复的列名，所以要使用别名，比如表1和表2都使用A列
            this.lstSelectNode.GroupBy(x => x.Column.ColumnName, (k, v) => v.ToList()).Where(v => v.Count > 1).ToList()
                .ForEach(x => x.ForEach(a => a.ColumnNameNick = a.Column.ColumnName + context.NextColIndex().ToString()));
            string strCol = ResolveComponentSELECT(context, this.lstSelectNode);//查询的列
            string strfrom = this.lstDbSet[0].NameHandler(context) + " AS " + this.lstDbSet[0].NameNick;
            string strjn = ResolveComponentJOIN(context, this.lstJoinNode);
            string strwhere = ResolveComponentWHERE(context, this.WH, "WHERE");
            string strgroup = ResolverComponentGroupBy(context, this.lstGroupByNode);
            string strhaving = ResolveComponentWHERE(context, this.Having, "HAVING");
            string strOrder = ResolveComponentOrderBy(context, this.lstOrderByNode);//排序
            if (this.pageIndex * this.pageSize <= 0)//不分页
                return string.Format("{0}SELECT {1} {2} {3} \r\nFROM {4} {5} {6} {7} {8} {9} {10}", strWithAS
                    ,this.isDistinct ? "DISTINCT" : string.Empty
                    //,top > 0 ? "TOP " + top.ToString() : string.Empty
                    , string.Empty
                    , strCol
                    ,strfrom, strjn, strwhere, strgroup, strhaving, strOrder
                    ,top>0?"limit "+top.ToString():string.Empty);
            string cmdstrCount = string.Format("{0}SELECT {1} {2} {3} \r\nFROM {4} {5} {6} {7} {8}", strWithAS
                    ,this.isDistinct ? "DISTINCT" : string.Empty
                    //,top > 0 ? "TOP " + top.ToString() : string.Empty
                    ,string.Empty
                   ,strCol
                    ,strfrom, strjn, strwhere, strgroup, strhaving, strOrder
                    ,top > 0 ? "limit " + top.ToString() : string.Empty);
            cmdstrCount = string.Format(@"select count(0)
                                                                from({0}) _avatar",cmdstrCount);
            string cmdstr = string.Format("{0}SELECT {1} {2} {3} ,({12}) as {13} \r\nFROM {4} {5} {6} {7} {8} {9} limit {10},{11}", strWithAS
                    ,this.isDistinct ? "DISTINCT" : string.Empty
                    //top > 0 ? "TOP " + top.ToString() : string.Empty
                    ,string.Empty
                    ,strCol
                    , strfrom , strjn, strwhere, strgroup, strhaving, strOrder
                    ,(this.pageSize*this.pageIndex-this.pageSize).ToString()
                    ,this.pageSize.ToString()
                    ,cmdstrCount
                    ,Enumerable.ColNameForRowCount);
            return cmdstr;
        }
    }
}
