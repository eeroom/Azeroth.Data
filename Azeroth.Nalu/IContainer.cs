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
        List<IContainer> CTEContainer { get; }
        List<INodeSelect> SelectNode {  get; }
        List<INode> JoinNode { get; }

        string NameForCTE { set; get; }
        
        List<T> ToList<CnnType, T>(Func<object[], T> transfer,string cnnstr) where CnnType : System.Data.Common.DbConnection, new();
    }
}
