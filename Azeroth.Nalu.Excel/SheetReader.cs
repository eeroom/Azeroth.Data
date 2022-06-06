using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu.Excel
{
    public class SheetReader<T>:ColumnMapper<T, SheetReader<T>> where T : class,new()
    {
        
        public List<T> ToListByColIndex(NPOI.SS.UserModel.ISheet sheet,int startrow)
        {
            if (dictMapHandlerByColIndex.Count < 1)
                throw new ArgumentException("必须指定列索引和model之间的映射");
            //sheet.Workbook.MissingCellPolicy = NPOI.SS.UserModel.MissingCellPolicy.CREATE_NULL_AS_BLANK;
            //NPOI.SS.UserModel.IFormulaEvaluator eva= NPOI.SS.UserModel.WorkbookFactory.CreateFormulaEvaluator(sheet.Workbook);
            //foreach (var kv in dictMapHandler)
            //    kv.Value.SetEvaHandler(eva);
            System.Collections.IEnumerator reader = sheet.GetRowEnumerator();
            List<string> lstError = new List<string>();
            List<T> lstdata = new List<T>();
            while (reader.MoveNext())
            {
                var row= (NPOI.SS.UserModel.IRow)reader.Current;
                if (row.RowNum < startrow)
                    continue;
                T value = new T();
                foreach (var kv in dictMapHandlerByColIndex)
                {
                    try
                    {
                        kv.Value(row.GetCell(kv.Key), value);
                    }
                    catch (Exception ex)
                    {
                        lstError.Add(string.Format("行列[{0},{1}]错误：{2}",row.RowNum,kv.Key,ex.Message));
                    }
                }
                lstdata.Add(value);
            }
            if (lstError.Count > 0)
                throw new ArgumentException(string.Join(";", lstError));
            return lstdata;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="callback">典型场景，数据校验</param>
        /// <returns></returns>
        public List<T> ToListByColName(NPOI.SS.UserModel.ISheet sheet,int startrow)
        {
            if (dictMapHandlerByColName == null)
                throw new ArgumentException("必须指定列名称和model之间的映射");
            var row= sheet.GetRow(startrow);
            var lstIndexName = System.Linq.Enumerable.Range(0, row.Cells.Count).Select(x => new { Index=x,Name=row.GetCell(x).StringCellValue})
                .Where(x=>dictMapHandlerByColName.Keys.Contains(x.Name)).ToList();
            var cf= lstIndexName.GroupBy(x => x.Name).Count(gp=>gp.Count()>1);
            if (cf > 0)
                throw new ArgumentException("Excel中存在重复的列名称");
            var dictTmp= lstIndexName.ToDictionary(x=>x.Name,x=>x.Index);
            var lose = dictMapHandlerByColName.Count(x=>!dictTmp.ContainsKey(x.Key));
            if (lose > 0)
                throw new ArgumentException("Excel中缺少必须的列");
            var dictNameIndex= dictMapHandlerByColName.ToDictionary(kv=>kv.Key,kv=>dictTmp[kv.Key]);
            System.Collections.IEnumerator reader = sheet.GetRowEnumerator();
            List<string> lstError = new List<string>();
            List<T> lstdata = new List<T>();
            while (reader.MoveNext())
            {
                row = (NPOI.SS.UserModel.IRow)reader.Current;
                if (row.RowNum < startrow+1)
                    continue;
                T value = new T();
                foreach (var kv in dictMapHandlerByColName)
                {
                    try
                    {
                        kv.Value(row.GetCell(dictNameIndex[kv.Key]), value);
                    }
                    catch (Exception ex)
                    {
                        lstError.Add(string.Format("行列[{0},{1}]错误：{2}", row.RowNum, kv.Key, ex.Message));
                    }
                }
                lstdata.Add(value);
            }
            if(lstError.Count>0)
                throw new ArgumentException(string.Join(";", lstError));
            return lstdata;
        }
    }
}
