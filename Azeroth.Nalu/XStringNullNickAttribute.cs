using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// 数据库中的列，
    /// 该列在业务上来看是可以为空的
    /// 但是设计表的时候，考虑建索引或者别的原因，把该列设计为不可空并预设一个替代空值的数据
    /// </summary>
    //public class XStringNullNickAttribute : System.Attribute
    //{
    //    const string NULLNICK = "_wch_*#*#1118#*#*_";
    //    public XStringNullNickAttribute(string nickValue)
    //    {
    //        this.nickValue = nickValue ?? NULLNICK;
    //    }

    //    string nickValue;
    //    public object ReverseFromInstanceToDB(object value)
    //    {
    //        if (value == null)
    //            return this.nickValue;
    //        return value;
    //    }

    //    public T ReverseFromDataReaderToInstance<T>(T value)
    //    {
    //        if (this.nickValue.Equals(value))
    //            return default(T);
    //        return value;
    //    }
    //}
}
