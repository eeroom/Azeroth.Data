using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// 从数据的有效性角度抽象的记录属性,用于软删除
    /// </summary>
    public enum RowState
    {
        /// <summary>
        /// 正常的记录，默认值
        /// </summary>
        Ok,
        /// <summary>
        /// 删除的记录（用于程序层面追溯）
        /// </summary>
        Del,
        /// <summary>
        /// 备份的记录（用于用户层面追溯，例如编辑的历史记录详情）
        /// </summary>
        Bak
    }
}
