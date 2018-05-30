using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// 让Node做一些显示实现，避免直接暴露一些属性和方法
    /// </summary>
    public interface INode:ISQL
    {
        IColumn Column { get; }
    }
}
