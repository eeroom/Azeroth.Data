using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// 对应数据库中某个表
    /// </summary>
    /// <typeparam name="T">返回的数据放到T类型的集合中</typeparam>
    public abstract class Table<T> : Table
    {
        /// <summary>
        /// 表对应的Class的元数据
        /// </summary>
       protected static RuntimeTypeHandle MetaT = typeof(T).TypeHandle;

        /// <summary>
        /// 映射处理器的字典
        /// </summary>
        protected static readonly Dictionary<string, IMapHandler> DictMapHandlerInternal =
            typeof(T).GetProperties().ToDictionary(propmeta => propmeta.Name, propmeta => MapHandlerFactory<T>.Create(propmeta));

        public Table()
        {
            this.Name = typeof(T).Name;
        }
    }
}
