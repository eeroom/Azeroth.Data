using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// OMT查询解析为sql语句
    /// </summary>
    public interface IContainer:IResolver
    {
        List<IContainer> SubContainer { get; }
        List<INodeSelect> Select {  get; }
        List<INode> JOIN { get; }
        List<IColumn> GroupBy { get; }

        List<INode> OrderBy { get; }
        string NameForCTE { set; get; }
        
        List<T> ToList<CnnType, T>(Func<object[], T> transfer,string cnnstr) where CnnType : System.Data.Common.DbConnection, new();

        List<ITable> Items { get; }
    }
}
