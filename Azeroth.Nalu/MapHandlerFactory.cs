using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Azeroth.Nalu
{
    public class MapHandlerFactory<T>
    {
        static readonly System.RuntimeTypeHandle RTHString = typeof(String).TypeHandle;
        static readonly System.RuntimeTypeHandle RTHBytes = typeof(Byte[]).TypeHandle;

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

        public static IMapHandler Create(PropertyInfo propmeta)
        {
            System.RuntimeTypeHandle handle=propmeta.PropertyType.TypeHandle;
            if (handle.Equals(RTHString))
                return new MapHandler<T, String>(propmeta, (reader, index) => reader[index].ToString(), x => x);
            else if (handle.Equals(RTHInt))
                return new MapHandler<T, Int32>(propmeta, (reader, index) => reader.GetInt32(index),
                    x => { Int32 tmp; int.TryParse(x, out tmp); return tmp; });
            else if (handle.Equals(RTHIntNullable))
                return new MapHandler<T, Nullable<Int32>>(propmeta, (reader, index) => reader.IsDBNull(index) ? default(Nullable<Int32>) : reader.GetInt32(index),
                    x => { Int32 tmp; return int.TryParse(x, out tmp) ? tmp : default(Nullable<Int32>); });
            else if (handle.Equals(RTHDateTime))
                return new MapHandler<T, DateTime>(propmeta, (reader, index) => reader.GetDateTime(index),
                    x => { DateTime tmp; DateTime.TryParse(x, out tmp); return tmp; });
            else if (handle.Equals(RTHDateTimeNullable))
                return new MapHandler<T, Nullable<DateTime>>(propmeta, (reader, index) => reader.IsDBNull(index) ? default(Nullable<DateTime>) : reader.GetDateTime(index),
                    x => { DateTime tmp; return DateTime.TryParse(x, out tmp) ? tmp : default(Nullable<DateTime>); });
            else if (handle.Equals(RTHGuid))
                return new MapHandler<T, Guid>(propmeta, (reader, index) => reader.GetGuid(index),
                    x => { Guid tmp; Guid.TryParse(x, out tmp); return tmp; });
            else if (handle.Equals(RTHGuidNullable))
                return new MapHandler<T, Nullable<Guid>>(propmeta, (reader, index) => reader.IsDBNull(index) ? default(Nullable<Guid>) : reader.GetGuid(index),
                    x => { Guid tmp; return Guid.TryParse(x, out tmp) ? tmp : default(Nullable<Guid>); });
            else if (handle.Equals(RTHDouble))
                return new MapHandler<T, double>(propmeta, (reader, index) => reader.GetDouble(index),
                    x => { double tmp; double.TryParse(x, out tmp); return tmp; });
            else if (handle.Equals(RTHDoubleNullable))
                return new MapHandler<T, Nullable<double>>(propmeta, (reader, index) => reader.IsDBNull(index) ? default(Nullable<double>) : reader.GetDouble(index),
                    x => { double tmp; return double.TryParse(x, out tmp) ? tmp : default(Nullable<double>); });
            else if (handle.Equals(RTHInt64))
                return new MapHandler<T, long>(propmeta, (reader, index) => reader.GetInt64(index),
                    x => { long tmp; long.TryParse(x, out tmp); return tmp; });
            else if (handle.Equals(RTHInt64Nullable))
                return new MapHandler<T, Nullable<long>>(propmeta, (reader, index) => reader.IsDBNull(index) ? default(Nullable<long>) : reader.GetInt64(index),
                    x => { long tmp; return long.TryParse(x, out tmp) ? tmp : default(Nullable<long>); });
            else if (handle.Equals(RTHInt16))
                return new MapHandler<T, short>(propmeta, (reader, index) => reader.GetInt16(index),
                    x => { short tmp; short.TryParse(x, out tmp); return tmp; });
            else if (handle.Equals(RTHInt16Nullable))
                return new MapHandler<T, Nullable<short>>(propmeta, (reader, index) => reader.IsDBNull(index) ? default(Nullable<short>) : reader.GetInt16(index),
                    x => { short tmp; return short.TryParse(x, out tmp) ? tmp : default(Nullable<short>); });
            else if (handle.Equals(RTHBool))
                return new MapHandler<T, bool>(propmeta, (reader, index) => reader.GetBoolean(index),
                    x => { bool tmp; bool.TryParse(x, out tmp); return tmp; });
            else if (handle.Equals(RTHBoolNullable))
                return new MapHandler<T, Nullable<bool>>(propmeta, (reader, index) => reader.IsDBNull(index) ? default(Nullable<bool>) : reader.GetBoolean(index),
                    x => { bool tmp; return bool.TryParse(x, out tmp) ? tmp : default(Nullable<bool>); });
            else if (handle.Equals(RTHChar))
                return new MapHandler<T, char>(propmeta, (reader, index) => reader.GetChar(index),
                    x => { char tmp; char.TryParse(x, out tmp); return tmp; });
            else if (handle.Equals(RTHCharNullable))
                return new MapHandler<T, Nullable<char>>(propmeta, (reader, index) => reader.IsDBNull(index) ? default(Nullable<char>) : reader.GetChar(index),
                    x => { char tmp; return char.TryParse(x, out tmp) ? tmp : default(Nullable<char>); });
            else if (handle.Equals(RTHBytes))
                return new MapHandler<T, byte[]>(propmeta, (reader, index) => reader.IsDBNull(index) ? default(byte[]) : (byte[])reader.GetValue(index),
                    x => string.IsNullOrEmpty(x) ? default(byte[]) : System.Text.Encoding.UTF8.GetBytes(x));
            else if (propmeta.PropertyType.IsEnum)
                return new MapHandler<T>(propmeta);
            else
                return new MapHandler(propmeta);
        }
    }
}
