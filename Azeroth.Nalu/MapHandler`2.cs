using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Azeroth.Nalu
{
    public class MapHandler<T, M> : IMapHandler
    {
        /// <summary>
        /// 对应某个属性的get方法
        /// </summary>
        Func<T, M> getvalueFromInstance;
        /// <summary>
        /// 对应某个属性的set方法
        /// </summary>
        Action<T, M> setvalueToInstance;
        /// <summary>
        /// 从datareader获取数据，这个委托可以避免装箱和拆箱
        /// </summary>
        Func<IDataReader, int, M> getvalueFromDataReader;
        /// <summary>
        /// 把string类型的数据转成M类型
        /// </summary>
        Func<String, M> getvalueFromString;

        //XStringAttribute xStringAttr;

        //static readonly System.RuntimeTypeHandle METAXStringNullableAttr = typeof(Azeroth.Nalu.XStringAttribute).TypeHandle;
        //static readonly System.RuntimeTypeHandle METAXStringNullNickAttr=typeof(Azeroth.Nalu.XStringNullNickAttribute).TypeHandle;

        //XStringNullNickAttribute xStringNullNickAttr;

        public MapHandler(Expression<Func<T, M>> selector)
        {
            this.getvalueFromInstance = selector.Compile();
            System.Type metaM = typeof(M);
            //x,value=>x.Name=value
            var valueParameter = Expression.Parameter(metaM);
            var body = Expression.Assign(selector.Body, valueParameter);
            this.setvalueToInstance = Expression.Lambda<Action<T, M>>(body, selector.Parameters[0], valueParameter).Compile();
            this.getvalueFromDataReader = (reader, index) => reader.IsDBNull(index) ? default(M) : (M)reader.GetValue(index);
            this.getvalueFromString = x => (M)System.Convert.ChangeType(x,metaM);
        }

        public MapHandler(PropertyInfo pmeta,Func<System.Data.IDataReader, int, M> getvalueFromDataReader, Func<string, M> getvalueFromString)
        {
            this.getvalueFromInstance = (Func<T, M>)System.Delegate.CreateDelegate(typeof(Func<T, M>), pmeta.GetGetMethod());
            this.setvalueToInstance = (Action<T, M>)System.Delegate.CreateDelegate(typeof(Action<T, M>), pmeta.GetSetMethod());
            this.getvalueFromDataReader = getvalueFromDataReader;
            this.getvalueFromString = getvalueFromString;
            //this.xStringAttr = pmeta.GetCustomAttributes(Type.GetTypeFromHandle(METAXStringNullableAttr), false).Cast<XStringAttribute>().FirstOrDefault();
            //this.xStringNullNickAttr = pmeta.GetCustomAttributes(Type.GetTypeFromHandle(METAXStringNullNickAttr), false).Cast<XStringNullNickAttribute>().FirstOrDefault();
        }

        public void SetValueToInstance(object instance, object value, object[] index)
        {
            setvalueToInstance((T)instance, (M)value);
        }

        public object GetValueFromInstance(object instance, object[] index)
        {
            return getvalueFromInstance((T)instance);
        }

        public void SetValueToInstance(object instance, IDataReader reader, int index)
        {
            setvalueToInstance((T)instance, getvalueFromDataReader(reader, index));
        }

        //public bool ValidateInstance(object instance, out string msg)
        //{
        //    msg = string.Empty;
        //    if (this.xStringAttr == null)
        //        return true;
        //    string str = this.GetValueFromInstance(instance, null) as string;
        //    return xStringAttr.Validate(str, out msg);
        //}

        public void SetValueToInstance(object instance, string value)
        {
            this.setvalueToInstance((T)instance,this.getvalueFromString(value));
        }

        //public bool IsMapStringToEnum()
        //{
        //    return false;
        //}
    }
}
