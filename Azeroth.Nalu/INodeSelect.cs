using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public interface INodeSelect:INode
    {

        /// <summary>
        /// 别名
        /// </summary>
        string ColumnNameNick { get; set; }

        /// <summary>
        /// 查询的时候，该列对应于reader中的索引
        /// </summary>
        int ColIndex { get; set; }
    }
}
