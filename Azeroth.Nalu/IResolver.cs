using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// 解析出SQL相关的信息
    /// </summary>
    public interface IResolver
    {
        
        /// <summary>
        /// 生成命令语句（某个node节点局部的sql语句或者完整的sql语句）
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        string ToSQL(ResolveContext context);
    }
}
