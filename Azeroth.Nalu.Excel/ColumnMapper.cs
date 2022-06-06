using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu.Excel
{
    public class ColumnMapper<T,SR> where SR: ColumnMapper<T, SR>
    {
       protected Dictionary<string, Action<NPOI.SS.UserModel.ICell, T>> dictMapHandlerByColName { set; get; }


        protected Dictionary<int, Action<NPOI.SS.UserModel.ICell, T>> dictMapHandlerByColIndex{ set; get; }

        public ColumnMapper()
        {
            this.dictMapHandlerByColName = new Dictionary<string, Action<NPOI.SS.UserModel.ICell, T>>();
            this.dictMapHandlerByColIndex = new Dictionary<int, Action<NPOI.SS.UserModel.ICell, T>>();
        }

        public SR Map(Action<NPOI.SS.UserModel.ICell, T> handler)
        {
            dictMapHandlerByColIndex.Add(dictMapHandlerByColIndex.Count, handler);
            return (SR)this;
        }

        public SR Map(Action<NPOI.SS.UserModel.ICell, T> handler, string excelColName)
        {
            dictMapHandlerByColName.Add(excelColName, handler);
            return (SR)this;
        }
    }
}
