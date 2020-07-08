using Azeroth.Nalu.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// OMT查询解析为sql语句
    /// </summary>
    public interface IQuery:IResolver
    {
        List<IQuery> SubContainer { get; }
        List<ISelectNode> Select {  get; }
        List<INodeBase> JOIN { get; }
        List<IColumn> GroupBy { get; }

        List<INodeBase> OrderBy { get; }
        string NameForCTE { set; get; }
        
        List<T> ToList<CnnType, T>(Func<object[], T> transfer,string cnnstr) where CnnType : System.Data.Common.DbConnection, new();

        List<ITable> Items { get; }
    }
}
