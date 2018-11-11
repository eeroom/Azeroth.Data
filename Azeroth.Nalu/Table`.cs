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
        static RuntimeTypeHandle MetaT = typeof(T).TypeHandle;

        /// <summary>
        /// 映射处理器的字典
        /// </summary>
        static readonly Dictionary<string, IMapHandler> DictMapHandlerInternal =
            typeof(T).GetProperties().ToDictionary(propmeta => propmeta.Name, propmeta => MapHandlerFactory<T>.Create(propmeta));

        public static DictionaryWrapper<string,IMapHandler> GetDictMapHandler()
        {
            return new DictionaryWrapper<string, IMapHandler>(DictMapHandlerInternal);
        }

        public Table()
        {
            this.nameHandler = context => Type.GetTypeFromHandle(MetaT).Name;
            this.dictMapHandler = new DictionaryWrapper<string, IMapHandler>(DictMapHandlerInternal);
        }

        protected override object CreateInstance(bool isCreateNull)
        {
            if (isCreateNull)
                return null;
            return System.Activator.CreateInstance<T>();
        }

        protected override RuntimeTypeHandle GetMetaInfo()
        {
            return MetaT;
        }

        public virtual Column<T, S> Col<S>(Expression<Func<T, S>> exp)
        {
            return new Column<T, S>(this, exp);
            //if (cteHandler == null)
            //    return new Column<T, S>(this, exp);
            ////dbset同名称，col同名称
            //string name = Type.GetTypeFromHandle(this.GetMetaInfo()).Name;//这里不用namehandler,因为这样会拿到他对于cte的名字，这里只需要他原本的表名称
            //var colName = Column.GetColumnName(exp.Body);
            //var mapColumn = cteHandler.SelectNodes.FirstOrDefault(x => name.Equals(x.Column.Container.NameHandler(null)) && colName.Equals(x.Column.ColumnName));
            //return new Column<T, S>(this, exp, mapColumn);
        }

        public virtual List<Column> Cols<S>(System.Linq.Expressions.Expression<Func<T, S>> exp)
        {
            var lstName = Column.GetNameCollection(exp.Body);
            return lstName.Select(x => new Column(this, x)).ToList();
        }

        public virtual List<Column> Cols()
        {
            return this.dictMapHandler.Keys.Select(x => new Column(this, x)).ToList();
        }
    }
}
