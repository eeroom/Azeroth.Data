using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// 对class的某个属性的包装，
    /// 使用缓存委托的方法，提高orm映射和赋值的效率
    /// </summary>
    public interface IMapHandler
    {
        /// <summary>
        /// 设置这个属性的值（这样设计为了和反射里面的属性元数据的SetValue方法兼容）
        /// </summary>
        /// <param name="instance">实例对象</param>
        /// <param name="value">数值（属性类型值装箱后的数据，如果类型不匹配，产生异常）</param>
        /// <param name="index">额外的参数</param>
        void SetValueToInstance(object instance, object value, object[] index);

        /// <summary>
        /// 获取这个属性的值（这样设计为了和反射里面的属性元数据的GetValue方法兼容）
        /// </summary>
        /// <param name="instance">实例对象</param>
        /// <param name="index">额外的参数</param>
        /// <returns>返回的数据</returns>
        object GetValueFromInstance(object instance, object[] index);

        /// <summary>
        /// 把datareader中的数据赋值给这个属性，可以避免拆箱
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="reader"></param>
        /// <param name="index"></param>
        void SetValueToInstance(object instance, IDataReader reader, int index);

        /// <summary>
        /// 通过转换，把字符串类型的值赋给这个属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        void SetValueToInstance(object obj,string value);


        /// <summary>
        /// 在数据写入数据库之前，配合XStringNullableAtrr做值得校验，非空，长度，等等（当前只针对字符串类型的属性）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        //bool ValidateInstance(object instance, out string msg);

        ///// <summary>
        ///// 把数据库中string的列映射为enum的属性
        ///// </summary>
        ///// <returns></returns>
        //bool IsMapStringToEnum();

    }
}
