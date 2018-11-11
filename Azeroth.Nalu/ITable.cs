using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public interface ITable
    {
        /// <summary>
        /// 映射处理器，把值从datareader取出赋值到对象的属性等
        /// </summary>
        DictionaryWrapper<string, IMapHandler> DictMapHandler {  get; }

        /// <summary>
        /// 获取该set对应的from语句的名称
        /// 为了支持from嵌套查询，这里从静态字符串改成运行时委托
        /// </summary>
        Func<ResovleContext, string> NameHandler { set; get; }
        /// <summary>
        /// 别名
        /// </summary>
        string NameNick { get; set; }

        string Name { get; }


        /// <summary>
        /// 需要查询的列
        /// 需要修改的列
        /// 需要新增值得列
        /// </summary>
        List<INodeSelect> SelectNodes { set; get; }

        /// <summary>
        /// 创建该Set对应的实例
        /// </summary>
        /// <param name="colCount"></param>
        /// <returns></returns>
        object CreateInstance(bool isCreateNull);
    }
}
