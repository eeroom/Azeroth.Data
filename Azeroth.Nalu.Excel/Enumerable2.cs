using Azeroth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace System.Linq
{
    public static class Enumerable2
    {
        public static string GetString(this NPOI.SS.UserModel.ICell cell,NPOI.SS.UserModel.IFormulaEvaluator eva)
        {
            string tmp = null;
            switch (cell.CellType)
            {
                case NPOI.SS.UserModel.CellType.Blank:
                    break;
                case NPOI.SS.UserModel.CellType.Boolean:
                    tmp = cell.BooleanCellValue.ToString();
                    break;
                case NPOI.SS.UserModel.CellType.Error:
                    break;
                case NPOI.SS.UserModel.CellType.Formula:
                    tmp = eva.Evaluate(cell).FormatAsString();
                    break;
                case NPOI.SS.UserModel.CellType.Numeric:
                    tmp = NPOI.SS.UserModel.DateUtil.IsCellDateFormatted(cell) ? 
                        cell.DateCellValue.ToString("yyyy-MM-dd HH:mm:ss") : cell.NumericCellValue.ToString();
                    break;
                case NPOI.SS.UserModel.CellType.String:
                    tmp = cell.StringCellValue;
                    break;
                case NPOI.SS.UserModel.CellType.Unknown:
                    break;
                default:
                    break;
            }
            return tmp;
        }

        public static Nullable<bool> GetBool(this NPOI.SS.UserModel.ICell cell,NPOI.SS.UserModel.IFormulaEvaluator eva)
        {
            Nullable<bool> tmp = default(Nullable<bool>);
            switch (cell.CellType)
            {
                case NPOI.SS.UserModel.CellType.Blank:
                    break;
                case NPOI.SS.UserModel.CellType.Boolean:
                    tmp = cell.BooleanCellValue;
                    break;
                case NPOI.SS.UserModel.CellType.Error:
                    break;
                case NPOI.SS.UserModel.CellType.Formula:
                    break;
                case NPOI.SS.UserModel.CellType.Numeric:
                    if(NPOI.SS.UserModel.DateUtil.IsCellDateFormatted(cell))
                        throw new ArgumentException("单元格是日期时间格式，请转换为bool格式");
                    tmp = ((int)cell.NumericCellValue)!=0;
                    break;
                case NPOI.SS.UserModel.CellType.String:
                    tmp = bool.Parse(cell.StringCellValue);
                    break;
                case NPOI.SS.UserModel.CellType.Unknown:
                    break;
                default:
                    break;
            }
            return tmp;
        }

        public static Nullable<double> GetDouble(this NPOI.SS.UserModel.ICell cell,NPOI.SS.UserModel.IFormulaEvaluator eva)
        {
            Nullable<double> tmp = default(Nullable<double>);
            switch (cell.CellType)
            {
                case NPOI.SS.UserModel.CellType.Blank:
                    break;
                case NPOI.SS.UserModel.CellType.Boolean:
                    tmp = cell.BooleanCellValue ? 1.0 : 0.0;
                    break;
                case NPOI.SS.UserModel.CellType.Error:
                    break;
                case NPOI.SS.UserModel.CellType.Formula:
                    var cellValue = eva.Evaluate(cell);
                    if (cellValue.CellType == NPOI.SS.UserModel.CellType.Numeric)
                        tmp = cellValue.NumberValue;
                    if (cellValue.CellType == NPOI.SS.UserModel.CellType.String)
                        tmp = double.Parse(cellValue.StringValue);
                    break;
                case NPOI.SS.UserModel.CellType.Numeric:
                    if(NPOI.SS.UserModel.DateUtil.IsCellDateFormatted(cell))
                        throw new ArgumentException("单元格是日期时间格式，请转换为数字格式");
                    tmp = cell.NumericCellValue;
                    break;
                case NPOI.SS.UserModel.CellType.String:
                    tmp = double.Parse(cell.StringCellValue);
                    break;
                case NPOI.SS.UserModel.CellType.Unknown:
                    break;
                default:
                    break;
            }
            return tmp;
        }

        public static Nullable<DateTime> GetDateTime(this NPOI.SS.UserModel.ICell cell,NPOI.SS.UserModel.IFormulaEvaluator eva)
        {
            Nullable<DateTime> tmp=default(Nullable<DateTime>);
            switch (cell.CellType)
            {
                case NPOI.SS.UserModel.CellType.Blank:
                    break;
                case NPOI.SS.UserModel.CellType.Boolean:
                    break;
                case NPOI.SS.UserModel.CellType.Error:
                    break;
                case NPOI.SS.UserModel.CellType.Formula:
                    var cellValue =eva.Evaluate(cell);
                    if (cellValue.CellType == NPOI.SS.UserModel.CellType.Numeric)
                        tmp = NPOI.SS.UserModel.DateUtil.GetJavaDate(cellValue.NumberValue);
                    if (cellValue.CellType == NPOI.SS.UserModel.CellType.String)
                        tmp = DateTime.Parse(cellValue.StringValue);
                    break;
                case NPOI.SS.UserModel.CellType.Numeric:
                    if(!NPOI.SS.UserModel.DateUtil.IsCellDateFormatted(cell))
                        throw new ArgumentException("单元格是数字格式，请转换为日期时间格式");
                    tmp = cell.DateCellValue;
                    break;
                case NPOI.SS.UserModel.CellType.String:
                    tmp = DateTime.Parse(cell.StringCellValue);
                    break;
                case NPOI.SS.UserModel.CellType.Unknown:
                    break;
                default:
                    break;
            }
            return tmp;
        }
    }
}
