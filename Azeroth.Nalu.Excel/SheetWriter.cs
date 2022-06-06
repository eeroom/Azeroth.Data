using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu.Excel
{
    public class SheetWriter<T> : ColumnMapper<T, SheetReader<T>> where T : class, new()
    {
        /// <summary>
        /// 把数据写入到工作表
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="lst"></param>
        /// <returns></returns>
        public void InsertByColName(NPOI.SS.UserModel.ISheet sheet, IEnumerable<T> lst, int startrow = 0)
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
                    kv.Value(row.CreateCell(colIndex++), value);
                }

            }
        }


        public void InsertByColIndex(NPOI.SS.UserModel.ISheet sheet, IEnumerable<T> lst, int startrow)
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
                var row = sheet.CreateRow(rowIndex++);
                //colIndex = 0;
                foreach (var kv in dictMapHandlerByColIndex)
                {
                    //dictMapHandler[kv.Value].SetValueToCell(value, row.CreateCell(kv.Key));
                    kv.Value(row.CreateCell(kv.Key), value);
                }

            }
        }

    }
}
