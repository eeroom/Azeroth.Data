using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu.Excel
{
    public class CellValueHandlerFactory
    {
       
       static readonly Action<NPOI.SS.UserModel.ICell, string> setvalueString = (cell, value) => cell.SetCellValue(value);
       static readonly Action<NPOI.SS.UserModel.ICell, int> setvalueInt32 = (cell, value) => cell.SetCellValue((double)value);
       static readonly Action<NPOI.SS.UserModel.ICell, double> setvalueDouble = (cell, value) => cell.SetCellValue(value);
       static readonly Action<NPOI.SS.UserModel.ICell, DateTime> setvalueDateTime = (cell, value) =>cell.SetCellValue(value);
       static readonly Action<NPOI.SS.UserModel.ICell, Guid> setvalueGuid = (cell, value) => cell.SetCellValue(value.ToString());
       static readonly Action<NPOI.SS.UserModel.ICell, long> setvalueInt64 = (cell, value) => cell.SetCellValue((double)value);
       static readonly Action<NPOI.SS.UserModel.ICell, short> setvalueInt16 = (cell, value) => cell.SetCellValue((double)value);
       static readonly Action<NPOI.SS.UserModel.ICell, bool> setvalueBool = (cell, value) => cell.SetCellValue(value);
       static readonly Action<NPOI.SS.UserModel.ICell, char> setvalueChar = (cell, value) => cell.SetCellValue(value.ToString());
       static readonly Action<NPOI.SS.UserModel.ICell, Nullable<int>> setvalueInt32Null = (cell, value) => { if (value.HasValue)cell.SetCellValue((double)value.Value); };
       static readonly Action<NPOI.SS.UserModel.ICell, Nullable<double>> setvalueDoubleNull = (cell, value) => { if (value.HasValue)cell.SetCellValue(value.Value); };
       static readonly Action<NPOI.SS.UserModel.ICell, Nullable<DateTime>> setvalueDateTimeNull = (cell, value) => { if (value.HasValue)cell.SetCellValue(value.Value); };
       static readonly Action<NPOI.SS.UserModel.ICell, Nullable<Guid>> setvalueGuidNull = (cell, value) => { if (value.HasValue)cell.SetCellValue(value.Value.ToString()); };
       static readonly Action<NPOI.SS.UserModel.ICell, Nullable<long>> setvalueInt64Null = (cell, value) => { if (value.HasValue)cell.SetCellValue((double)value.Value); };
       static readonly Action<NPOI.SS.UserModel.ICell, Nullable<short>> setvalueInt16Null = (cell, value) => { if (value.HasValue)cell.SetCellValue((double)value.Value); };
       static readonly Action<NPOI.SS.UserModel.ICell, Nullable<bool>> setvalueBoolNull = (cell, value) => { if (value.HasValue)cell.SetCellValue(value.Value); };
       static readonly Action<NPOI.SS.UserModel.ICell, Nullable<char>> setvalueCharNull = (cell, value) => { if (value.HasValue)cell.SetCellValue(value.Value.ToString()); };

       static readonly Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, string> getvalueString = (cell, eva) => cell.GetString(eva);
       static readonly Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, int> getvalueInt32 = (cell, eva) => (int)(cell.GetDouble(eva).Value);
       static readonly Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, double> getvalueDouble = (cell, eva) => cell.GetDouble(eva).Value;
       static readonly Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, DateTime> getvalueDateTime = (cell, eva) => cell.GetDateTime(eva).Value;
       static readonly Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, Guid> getvalueGuid = (cell, eva) => new Guid(cell.GetString(eva));
       static readonly Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, long> getvalueInt64 = (cell, eva) => (long)cell.GetDouble(eva).Value;
       static readonly Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, short> getvalueInt16 = (cell, eva) => (short)cell.GetDouble(eva).Value;
       static readonly Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, bool> getvalueBool = (cell, eva) => cell.GetBool(eva).Value;
       static readonly Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, char> getvalueChar = (cell, eva) => cell.GetString(eva)[0];
       static readonly Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, Nullable<int>> getvalueInt32Null = (cell, eva) => (Nullable<int>)cell.GetDouble(eva);
       static readonly Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, Nullable<double>> getvalueDoubleNull = (cell, eva) => cell.GetDouble(eva);
       static readonly Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, Nullable<DateTime>> getvalueDateTimeNull = (cell, eva) => cell.GetDateTime(eva);
       static readonly Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, Nullable<Guid>> getvalueGuidNull = (cell, eva) => { var tmp = cell.GetString(eva); return tmp == null ? default(Nullable<Guid>) : new Guid(tmp); };
       static readonly Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, Nullable<long>> getvalueInt64Null = (cell, eva) => (Nullable<long>)cell.GetDouble(eva);
       static readonly Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, Nullable<short>> getvalueInt16Null = (cell, eva) => (Nullable<short>)cell.GetDouble(eva);
       static readonly Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, Nullable<bool>> getvalueBoolNull = (cell, eva) => cell.GetBool(eva);
       static readonly Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, Nullable<char>> getvalueCharNull = (cell, eva) => { var tmp = cell.GetString(eva); return tmp == null ? default(Nullable<char>) : tmp[0]; };


       //static readonly Func<NPOI.SS.UserModel.ICell, string> setvalueString = (cell, value) => cell.SetCellValue(value);


        static readonly System.RuntimeTypeHandle RTHString = typeof(String).TypeHandle;
       static readonly System.RuntimeTypeHandle RTHInt = typeof(Int32).TypeHandle;
       static readonly System.RuntimeTypeHandle RTHDouble = typeof(Double).TypeHandle;
       static readonly System.RuntimeTypeHandle RTHGuid = typeof(Guid).TypeHandle;
       static readonly System.RuntimeTypeHandle RTHDateTime = typeof(DateTime).TypeHandle;
       static readonly System.RuntimeTypeHandle RTHChar = typeof(Char).TypeHandle;
       static readonly System.RuntimeTypeHandle RTHInt64 = typeof(Int64).TypeHandle;
       static readonly System.RuntimeTypeHandle RTHBool = typeof(Boolean).TypeHandle;
       static readonly System.RuntimeTypeHandle RTHInt16 = typeof(Int16).TypeHandle;

       static readonly System.RuntimeTypeHandle RTHIntNullable = typeof(Nullable<Int32>).TypeHandle;
       static readonly System.RuntimeTypeHandle RTHDoubleNullable = typeof(Nullable<Double>).TypeHandle;
       static readonly System.RuntimeTypeHandle RTHGuidNullable = typeof(Nullable<Guid>).TypeHandle;
       static readonly System.RuntimeTypeHandle RTHDateTimeNullable = typeof(Nullable<DateTime>).TypeHandle;
       static readonly System.RuntimeTypeHandle RTHCharNullable = typeof(Nullable<Char>).TypeHandle;
       static readonly System.RuntimeTypeHandle RTHInt64Nullable = typeof(Nullable<Int64>).TypeHandle;
       static readonly System.RuntimeTypeHandle RTHBoolNullable = typeof(Nullable<Boolean>).TypeHandle;
       static readonly System.RuntimeTypeHandle RTHInt16Nullable = typeof(Nullable<Int16>).TypeHandle;


        public static Delegate CreateSetValueHandler<P>()
        {
            var handle= typeof(P).TypeHandle;
            if (handle.Equals(RTHString))
                return setvalueString;
            else if (handle.Equals(RTHInt))
                return setvalueInt32;
            else if (handle.Equals(RTHIntNullable))
                return setvalueInt32Null;
            else if (handle.Equals(RTHDateTime))
                return setvalueDateTime;
            else if (handle.Equals(RTHDateTimeNullable))
                return setvalueDateTimeNull;
            else if (handle.Equals(RTHGuid))
                return setvalueGuid;
            else if (handle.Equals(RTHGuidNullable))
                return setvalueGuidNull;
            else if (handle.Equals(RTHDouble))
                return setvalueDouble;
            else if (handle.Equals(RTHDoubleNullable))
                return setvalueDoubleNull;
            else if (handle.Equals(RTHInt64))
                return setvalueInt64;
            else if (handle.Equals(RTHInt64Nullable))
                return setvalueInt64Null;
            else if (handle.Equals(RTHInt16))
                return setvalueInt16;
            else if (handle.Equals(RTHInt16Nullable))
                return setvalueInt16Null;
            else if (handle.Equals(RTHBool))
                return setvalueBool;
            else if (handle.Equals(RTHBoolNullable))
                return setvalueBoolNull;
            else if (handle.Equals(RTHChar))
                return setvalueChar;
            else if (handle.Equals(RTHCharNullable))
                return setvalueCharNull;
            else
                throw new ArgumentException("P的类型不支持");
        }

        public static Delegate CreateGetValueHandler<P>()
        {
            var handle = typeof(P).TypeHandle;
            if (handle.Equals(RTHString))
                return getvalueString;
            else if (handle.Equals(RTHInt))
                return getvalueInt32;
            else if (handle.Equals(RTHIntNullable))
                return getvalueInt32Null;
            else if (handle.Equals(RTHDateTime))
                return getvalueDateTime;
            else if (handle.Equals(RTHDateTimeNullable))
                return getvalueDateTimeNull;
            else if (handle.Equals(RTHGuid))
                return getvalueGuid;
            else if (handle.Equals(RTHGuidNullable))
                return getvalueGuidNull;
            else if (handle.Equals(RTHDouble))
                return getvalueDouble;
            else if (handle.Equals(RTHDoubleNullable))
                return getvalueDoubleNull;
            else if (handle.Equals(RTHInt64))
                return getvalueInt64;
            else if (handle.Equals(RTHInt64Nullable))
                return getvalueInt64Null;
            else if (handle.Equals(RTHInt16))
                return getvalueInt16;
            else if (handle.Equals(RTHInt16Nullable))
                return getvalueInt16Null;
            else if (handle.Equals(RTHBool))
                return getvalueBool;
            else if (handle.Equals(RTHBoolNullable))
                return getvalueBoolNull;
            else if (handle.Equals(RTHChar))
                return getvalueChar;
            else if (handle.Equals(RTHCharNullable))
                return getvalueCharNull;
            else
                throw new ArgumentException("P的类型不支持");
        }
    }
}
