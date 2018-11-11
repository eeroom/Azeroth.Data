using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class MssqlserverContainer:Container
    {
        public MssqlserverContainer(IDbContext dbcontext):base(dbcontext)
        {

        }

        protected override string ToSQL(ResovleContext context)
        {
            string strWithAS = ResolveCTE(context, this.lstCETContainer);
            if (!string.IsNullOrEmpty(strWithAS))
                strWithAS = strWithAS + " \r\n";
            this.lstDbSet.ForEach(x => x.NameNick = "T" + context.NextSetIndex().ToString());//表的别名
            //因为会出现重复的列名，所以要使用别名，比如表1和表2都使用A列
            this.lstNodeSelect.GroupBy(x => x.Column.ColumnName, (k, v) => v.ToList()).Where(v => v.Count > 1).ToList()
                .ForEach(x => x.ForEach(a => a.ColumnNameNick = a.Column.ColumnName + context.NextColIndex().ToString()));
            string strCol = ResolveNodeSelect(context, this.lstNodeSelect);//查询的列
            string strfrom = this.lstDbSet[0].NameHandler(context) + " AS " + this.lstDbSet[0].NameNick;
            string strjn = ResolveNodeJOIN(context, this.lstJoinNode);
            string strwhere = ResolveNodeWhere(context, this.Where, "WHERE");
            string strgroup = ResolverNodeGroupBy(context, this.lstNodeGroupBy);
            string strhaving = ResolveNodeWhere(context, this.Having, "HAVING");
            string strOrder = ResolveNodeOrderBy(context, this.lstNodeOrderBy);//排序
            if (this.pageIndex * this.pageSize <= 0)//不分页
                return string.Format("{0}SELECT {1} {2} {3} \r\nFROM {4} {5} {6} {7} {8} {9}",  strWithAS
                    , this.isDistinct ? "DISTINCT" : string.Empty
                    , top > 0 ? "TOP " + top.ToString() : string.Empty
                    ,strCol
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
    }
}
