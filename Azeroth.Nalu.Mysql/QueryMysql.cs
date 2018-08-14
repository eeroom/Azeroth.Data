using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu.Mysql
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
                return string.Format("{7}SELECT {8} {9} {0} \r\nFROM {1} {2} {3} {4} {5} {6}", strCol, strfrom, strjn, strwhere, strgroup, strhaving, strOrder, strWithAS, this.isDistinct ? "DISTINCT" : string.Empty, top > 0 ? "TOP " + top.ToString() : string.Empty);
            string tmp = string.Format("SELECT {7} {0},ROW_NUMBER() OVER({3}) AS theRowIndex FROM {1} {2} {4} {5} {6}", strCol, strfrom, strjn, strOrder, strwhere, strgroup, strhaving, this.isDistinct ? "DISTINCT" : string.Empty);
            int numEnd = this.pageIndex * this.pageSize;
            var p1 = context.CreateParameter();
            p1.ParameterName = context.Symbol + Nalu.Enumerable.ParameterNameForPaginationEnd;
            p1.Value = numEnd;
            context.Parameters.Add(p1);
            var p2 = context.CreateParameter();
            p2.ParameterName = context.Symbol + Nalu.Enumerable.ParameterNameForPaginationStart;
            p2.Value = numEnd + 1 - this.pageSize;
            context.Parameters.Add(p2);
            if (string.IsNullOrEmpty(strWithAS))
                return string.Format("WITH HTT AS ({0})\r\n,\r\nHBB AS (\r\nSELECT COUNT(0) AS {3} FROM HTT)\r\n\r\nSELECT HTT.*,HBB.* FROM HTT,HBB WHERE HTT.theRowIndex BETWEEN {1} AND {2}", tmp
                    , p2.ParameterName, p1.ParameterName, Nalu.Enumerable.ColNameForRowCount);
            else
                return string.Format("{1},HTT AS ({0})\r\n,\r\nHBB AS (\r\nSELECT COUNT(0) AS {4} FROM HTT)\r\n\r\nSELECT HTT.*,HBB.* FROM HTT,HBB WHERE HTT.theRowIndex BETWEEN {2} AND {3}", tmp
                    , strWithAS, p2.ParameterName, p1.ParameterName, Nalu.Enumerable.ColNameForRowCount);
        }
    }
}
