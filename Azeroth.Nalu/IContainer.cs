using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// OMT查询解析为sql语句
    /// </summary>
    public interface IContainer
    {
        List<IContainer> CTEHandlers { get; }
        List<INodeSelect> SelectNodes {  get; }
        List<INode> JoinNode { get; }

        string NameForCTE { set; get; }

        string GetCommandText(ResovleContext contex);
        
        List<T> Execute<H, T>(Func<object[], T> transfer,string cnnstr) where H : System.Data.Common.DbConnection, new();
    }
}
