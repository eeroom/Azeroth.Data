using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu.Excel
{
    public class SheetSet<T> where T : class,new()
    {
        Dictionary<string, Action<NPOI.SS.UserModel.ICell, T>> dictMapHandlerByColName = new Dictionary<string, Action<NPOI.SS.UserModel.ICell, T>>();

        Dictionary<int, Action<NPOI.SS.UserModel.ICell, T>> dictMapHandlerByColIndex = new Dictionary<int, Action<NPOI.SS.UserModel.ICell, T>>();

        public SheetValue<T> ToListByColIndex(NPOI.SS.UserModel.ISheet sheet,int startrow)
        {
            SheetValue<T> rt = new SheetValue<T>();
            if (dictMapHandlerByColIndex.Count < 1)
                throw new ArgumentException("必须指定列索引和model之间的映射");
            //sheet.Workbook.MissingCellPolicy = NPOI.SS.UserModel.MissingCellPolicy.CREATE_NULL_AS_BLANK;
            //NPOI.SS.UserModel.IFormulaEvaluator eva= NPOI.SS.UserModel.WorkbookFactory.CreateFormulaEvaluator(sheet.Workbook);
            //foreach (var kv in dictMapHandler)
            //    kv.Value.SetEvaHandler(eva);
            System.Collections.IEnumerator reader = sheet.GetRowEnumerator();
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
                        string msg=string.Format("第 {0} 行第 {1} 列附近发生错误，错误信息：{2}",row.RowNum,kv.Key,ex.Message);
                        rt.AddMessage(msg);
                    }
                }
                rt.Value.Add(value);
            }
            return rt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="callback">典型场景，数据校验</param>
        /// <returns></returns>
        public SheetValue<T> ToListByColName(NPOI.SS.UserModel.ISheet sheet,int startrow)
        {
            if (dictMapHandlerByColName == null)
                throw new ArgumentException("必须指定列名称和model之间的映射");
            SheetValue<T> rt = new SheetValue<T>();
            var row= sheet.GetRow(startrow);
            var lstIndexName = System.Linq.Enumerable.Range(0, row.Cells.Count).Select(x => new { Index=x,Name=row.GetCell(x).StringCellValue})
                .Where(x=>dictMapHandlerByColName.Keys.Contains(x.Name)).ToList();
            var cf= lstIndexName.GroupBy(x => x.Name).Count(gp=>gp.Count()>1);
            if (cf > 0)
                return rt.AddMessage("Excel中存在重复的列名称");
            var dictTmp= lstIndexName.ToDictionary(x=>x.Name,x=>x.Index);
            var lose = dictMapHandlerByColName.Count(x=>!dictTmp.ContainsKey(x.Key));
            if (lose > 0)
                return rt.AddMessage("Excel中缺少必须的列");
            var dictNameIndex= dictMapHandlerByColName.ToDictionary(kv=>kv.Key,kv=>dictTmp[kv.Key]);
            System.Collections.IEnumerator reader = sheet.GetRowEnumerator();
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
                        string msg = string.Format("第 {0} 行第 {1} 列附近发生错误，错误信息：{2}", row.RowNum, kv.Key, ex.Message);
                        rt.AddMessage(msg);
                    }
                }
                rt.Value.Add(value);
            }
            return rt;
        }

        /// <summary>
        /// 把数据写入到工作表
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="lst"></param>
        /// <returns></returns>
        public void InsertByColName(NPOI.SS.UserModel.ISheet sheet, IEnumerable<T> lst,int startrow=0)
        {
            if (sheet == null)
                throw new ArgumentException("sheet不能为Null");
            if (lst == null)
                throw new ArgumentException("value不能为Null");
            if (dictMapHandlerByColName == null)
                throw new ArgumentException("必须指定列名称和model之间的映射");
            int colIndex = 0;
            int rowIndex = startrow;
            NPOI.SS.UserModel.IRow row = sheet.CreateRow(rowIndex++);
            foreach (var kv in dictMapHandlerByColName)
            {
                row.CreateCell(colIndex++).SetCellValue(kv.Key);
            }
            foreach (var value in lst)
            {
                row = sheet.CreateRow(rowIndex++);
                colIndex = 0;
                foreach (var kv in dictMapHandlerByColName)
                {
                    //dictMapHandler[kv.Value].SetValueToCell(value, row.CreateCell(kv.Key));
                    kv.Value(row.CreateCell(colIndex++),value);
                }

            }
        }


        public void InsertByColIndex(NPOI.SS.UserModel.ISheet sheet, IEnumerable<T> lst,int startrow)
        {
            if (sheet == null)
                throw new ArgumentException("sheet不能为Null");
            if (lst == null)
                throw new ArgumentException("value不能为Null");
            if (dictMapHandlerByColIndex == null)
                throw new ArgumentException("必须指定列索引和model之间的映射");
            //int colIndex = 0;
            int rowIndex = startrow;
            //NPOI.SS.UserModel.IRow row = sheet.CreateRow(rowIndex++);
            //foreach (var kv in dictMapHandlerByColName)
            //{
            //    row.CreateCell(colIndex++).SetCellValue(kv.Key);
            //}
            foreach (var value in lst)
            {
                var  row = sheet.CreateRow(rowIndex++);
                //colIndex = 0;
                foreach (var kv in dictMapHandlerByColIndex)
                {
                    //dictMapHandler[kv.Value].SetValueToCell(value, row.CreateCell(kv.Key));
                    kv.Value(row.CreateCell(kv.Key), value);
                }

            }
        }

        public SheetSet<T> Map(Action<NPOI.SS.UserModel.ICell,T> handler)
        {
            dictMapHandlerByColIndex.Add(dictMapHandlerByColIndex.Count,handler);
            return this;
        }

        public SheetSet<T> Map(Action<NPOI.SS.UserModel.ICell, T> handler,string excelColName)
        {
            dictMapHandlerByColName.Add(excelColName,handler);
            return this;
        }
    }
}
