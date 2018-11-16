using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Azeroth.Nalu
{
    public class MapHandler :  IMapHandler
    {
        /// <summary>
        /// 对应某个属性的set方法
        /// </summary>
        Action<object, object, object[]> setvalueHandler;
        /// <summary>
        /// 对应某个属性的get方法
        /// </summary>
        Func<object, object[], object> getvalueHandler;

      
        System.RuntimeTypeHandle propertyRTH;


        public MapHandler(PropertyInfo pmeta)
        {
            this.setvalueHandler = pmeta.SetValue;
            this.getvalueHandler = pmeta.GetValue;
            this.propertyRTH = pmeta.PropertyType.TypeHandle;
        }

        public void SetValueToInstance(object instance, object value, object[] index)
        {
            setvalueHandler(instance, value, index);
        }

        public object GetValueFromInstance(object instance, object[] index)
        {
            return getvalueHandler(instance,index);
        }

        public void SetValueToInstance(object instance, IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return;
            setvalueHandler(instance, reader.GetValue(index),null);
        }

        //public bool ValidateInstance(object instance, out string msg)
        //{
        //    msg = string.Empty;
        //    return true;
        //}

        public void SetValueToInstance(object instance, string value)
        {
            try
            {
                object tmp = System.Convert.ChangeType(value, System.Type.GetTypeFromHandle(this.propertyRTH));
                this.setvalueHandler(instance,tmp,null);
            }
            catch(Exception)
            {
            }
        }


        //public bool IsMapStringToEnum()
        //{
        //    return false;
        //}
    }
}
