using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// 处理枚举类型的属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MapHandler<T> :  IMapHandler
    {
        /// <summary>
        /// 对应某个属性的set方法
        /// </summary>
        Action<T,int> setvalueToInstance;
        /// <summary>
        /// 对应某个属性的get方法
        /// </summary>
        Func<T,int> getvalueFromInstance;

      
        System.RuntimeTypeHandle propertyRTH;

        //XStringMapEnumAttribute xStringMapEnumAttr;

        //static readonly System.RuntimeTypeHandle METAXStringToEnumAttr=typeof(XStringMapEnumAttribute).TypeHandle;

        public MapHandler(PropertyInfo pmeta)
        {
            this.setvalueToInstance = (Action<T, int>)System.Delegate.CreateDelegate(typeof(Action<T,int>),pmeta.GetSetMethod());
            this.getvalueFromInstance = (Func<T, int>)System.Delegate.CreateDelegate(typeof(Func<T, int>), pmeta.GetGetMethod());
            //this.xStringMapEnumAttr = pmeta.GetCustomAttributes(System.Type.GetTypeFromHandle(METAXStringToEnumAttr),false).Cast<XStringMapEnumAttribute>().FirstOrDefault();
            this.propertyRTH = pmeta.PropertyType.TypeHandle;
        }

        public void SetValueToInstance(object instance, object value, object[] index)
        {
            setvalueToInstance((T)instance, (int)value);
        }

        public object GetValueFromInstance(object instance, object[] index)
        {
            int value = getvalueFromInstance((T)instance);
            return value;
            //if (this.xStringMapEnumAttr == null)
                
            //return System.Enum.GetName(System.Type.GetTypeFromHandle(propertyRTH),value);//返回枚举的名称
        }

        public void SetValueToInstance(object instance, IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return;
            setvalueToInstance((T)instance, reader.GetInt32(index));
            //if (this.xStringMapEnumAttr == null)
                
            //else
            //    setvalueToInstance((T)instance,(int)System.Enum.Parse(System.Type.GetTypeFromHandle(propertyRTH),reader.GetString(index)));
        }

        //public bool ValidateInstance(object instance, out string msg)
        //{
        //    msg = string.Empty;
        //    return true;
        //}

        public void SetValueToInstance(object instance, string value)
        {
            int tmp;
            if (int.TryParse(value, out tmp))
                this.setvalueToInstance((T)instance,tmp);
        }

        //public bool IsMapStringToEnum()
        //{
        //    return this.xStringMapEnumAttr != null;
        //}
    }
}
