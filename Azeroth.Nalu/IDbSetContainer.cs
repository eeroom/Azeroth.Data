using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// OMT查询解析为sql语句
    /// </summary>
    public interface IDbSetContainer
    {
        List<DbSetContainer> CTEHandlers {  get; }
        List<IComponentSELECT> SelectNodes {  get; }
        List<IComponent> JoinNode { get; }

        string NameForCTE { set; get; }

        string GetCommandText(ResovleContext contex);
        
        List<T> Execute<H, T>(Func<object[], T> transfer,string cnnstr) where H : System.Data.Common.DbConnection, new();
    }
}
