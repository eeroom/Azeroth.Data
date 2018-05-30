using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu.Excel
{
    public class SheetSet<T> where T : class,new()
    {
        Dictionary<string, IMapHandler> dictMapHandler = new Dictionary<string, IMapHandler>();



        public ListWrapper<T> ToList(NPOI.SS.UserModel.ISheet sheet)
        {
            sheet.Workbook.MissingCellPolicy = NPOI.SS.UserModel.MissingCellPolicy.CREATE_NULL_AS_BLANK;
            List<T> lst = new List<T>();
            NPOI.SS.UserModel.IFormulaEvaluator eva= NPOI.SS.UserModel.WorkbookFactory.CreateFormulaEvaluator(sheet.Workbook);
            foreach (var kv in dictMapHandler)
                kv.Value.SetEvaHandler(eva);
            System.Collections.IEnumerator reader = sheet.GetRowEnumerator();
            reader.MoveNext();
            NPOI.SS.UserModel.IRow row = (NPOI.SS.UserModel.IRow)reader.Current;
            Dictionary<int, string> dictIndexAndColName;
            try
            {
                dictIndexAndColName = dictMapHandler.ToDictionary(kv =>
                    row.First(cell => cell.CellType == NPOI.SS.UserModel.CellType.String && kv.Key.Equals(cell.StringCellValue)).ColumnIndex, kv => kv.Key);
            }
            catch (Exception ex)
            {
                return new ListWrapper<T>(lst, true, "第一行标题名称和模版不一致！无法确定要获取数据所在的列。错误信息：" + ex.Message);
            }
            KeyValuePair<int, string> kvtmp = new KeyValuePair<int, string>();
            try
            {//为了知道错误发生在第几行第几列，包括callback中的异常，所以这么try,
                while (reader.MoveNext())
                {
                    row = (NPOI.SS.UserModel.IRow)reader.Current;
                    T instance = new T();
                    foreach (var kv in dictIndexAndColName)
                    {
                        kvtmp = kv;
                        dictMapHandler[kv.Value].SetValueToInstance(instance, row.GetCell(kv.Key));
                    }
                    lst.Add(instance);
                }
            }
            catch (Exception ex)
            {
                return new ListWrapper<T>(lst, true, string.Format("第 {0} 行第 {1} 列 {3} 附近发生错误，错误信息：{2}", row == null ? 2 : row.RowNum + 1, kvtmp.Key+1, ex.Message,kvtmp.Value));
            }
            return new ListWrapper<T>(lst, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="callback">典型场景，数据校验</param>
        /// <returns></returns>
        public ListWrapper<T> ToList(NPOI.SS.UserModel.ISheet sheet, Action<T, NPOI.SS.UserModel.IRow> callback)
        {
            if (callback == null)
                return ToList(sheet);
            sheet.Workbook.MissingCellPolicy = NPOI.SS.UserModel.MissingCellPolicy.CREATE_NULL_AS_BLANK;
            List<T> lst = new List<T>();
            NPOI.SS.UserModel.IFormulaEvaluator eva = NPOI.SS.UserModel.WorkbookFactory.CreateFormulaEvaluator(sheet.Workbook);
            foreach (var kv in dictMapHandler)
                kv.Value.SetEvaHandler(eva);
            System.Collections.IEnumerator reader = sheet.GetRowEnumerator();
            reader.MoveNext();
            NPOI.SS.UserModel.IRow row = (NPOI.SS.UserModel.IRow)reader.Current;
            Dictionary<int, string> dictIndexAndColName;
            try
            {
                dictIndexAndColName = dictMapHandler.ToDictionary(kv =>
                    row.First(cell => cell.CellType == NPOI.SS.UserModel.CellType.String && kv.Key.Equals(cell.StringCellValue)).ColumnIndex, kv => kv.Key);
            }
            catch (Exception ex)
            {
                return new ListWrapper<T>(lst, true, "第一行标题名称和模版不一致！无法确定要获取数据所在的列。错误信息：" + ex.Message);
            }
            KeyValuePair<int, string> kvtmp=new KeyValuePair<int,string>();
            try
            {//为了知道错误发生在第几行第几列，包括callback中的异常，所以这么try,
                while (reader.MoveNext())
                {
                    row = (NPOI.SS.UserModel.IRow)reader.Current;
                    T instance = new T();
                    foreach (var kv in dictIndexAndColName)
                    {
                        kvtmp = kv;
                        dictMapHandler[kv.Value].SetValueToInstance(instance, row.GetCell(kv.Key));
                    }
                    callback(instance,row);
                    lst.Add(instance);
                }
            }
            catch (Exception ex)
            {
                return new ListWrapper<T>(lst, true, string.Format("第 {0} 行第 {1} 列 {3} 附近发生错误，错误信息：{2}", row == null ? 2 : row.RowNum+1, kvtmp.Key+1, ex.Message,kvtmp.Value));
            }
            return new ListWrapper<T>(lst, false, null);
        }

        /// <summary>
        /// 把数据写入到工作表
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Add(NPOI.SS.UserModel.IWorkbook workbook, IEnumerable<T> value,string sheetName)
        {
            if (string.IsNullOrEmpty(sheetName))
                return false;
            NPOI.SS.UserModel.ISheet sheet = workbook.GetSheet(sheetName);
            if (sheet != null)
                return false;
            sheet = workbook.CreateSheet(sheetName);
            int colIndex = 0;
            int rowIndex = 0;
            Dictionary<int,string> dictIndexAndColName = dictMapHandler.ToDictionary(kv => colIndex++, kv => kv.Key);
            NPOI.SS.UserModel.IRow row = sheet.CreateRow(rowIndex++);
            foreach (var kv in dictIndexAndColName)
                row.CreateCell(kv.Key).SetCellValue(kv.Value);
            if (value == null)
                return true;
            foreach (var instance in value)
            {
                row = sheet.CreateRow(rowIndex++);
                foreach (var kv in dictIndexAndColName)
                    dictMapHandler[kv.Value].SetValueToCell(instance, row.CreateCell(kv.Key));
            }
            return true;
        }

        public bool AddWithFreezeHeadrow(NPOI.SS.UserModel.IWorkbook workbook, IEnumerable<T> value, string sheetName)
        {
            var rst = this.Add(workbook,value,sheetName);
            if (!rst)
                return rst;
            workbook.GetSheet(sheetName).CreateFreezePane(0,1);
            return rst;
        }

        public SheetSet<T> Map<P>(System.Linq.Expressions.Expression<Func<T, P>> exp, string colName)
        {
            MapHandler<T, P> handler = new MapHandler<T, P>(exp);
            dictMapHandler.Add(colName, handler);
            return this;
        }

        public int Map<P>(System.Linq.Expressions.Expression<Func<T, P>> exp, string colName, Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, P> getvalueFromCell)
        {
            MapHandler<T, P> handler = new MapHandler<T, P>(exp,getvalueFromCell);
            dictMapHandler.Add(colName, handler);
            return dictMapHandler.Count-1;
        }

        public int Map<P>(Expression<Func<T, P>> exp, string colName, Action<NPOI.SS.UserModel.ICell, P> setvalueToCell)
        {
            MapHandler<T, P> handler = new MapHandler<T, P>(exp, setvalueToCell);
            dictMapHandler.Add(colName, handler);
            return dictMapHandler.Count - 1;
        }

        public int Map<P>(Expression<Func<T, P>> exp, string colName, Action<NPOI.SS.UserModel.ICell, P> setvalueToCell, Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, P> getvalueFromCell)
        {
            MapHandler<T, P> handler = new MapHandler<T, P>(exp, getvalueFromCell,setvalueToCell);
            dictMapHandler.Add(colName, handler);
            return dictMapHandler.Count - 1;
        }
    }


}
