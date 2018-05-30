using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// 要执行的命令类别
    /// </summary>
    public enum Cmd
    {
        None = 0,
        /// <summary>
        /// 增
        /// </summary>
        Add = 1,
        /// <summary>
        /// 删
        /// </summary>
        Del = 3,
        /// <summary>
        /// 改
        /// </summary>
        Edit = 2,
        /// <summary>
        /// 查
        /// </summary>
        Query = 4
    }

    /// <summary>
    /// 连接关系
    /// </summary>
    public enum JOIN
    {
        /// <summary>
        /// 内连接
        /// </summary>
        Inner = 1,
        /// <summary>
        /// 左连接
        /// </summary>
        Left = 2,
        /// <summary>
        /// 右连接
        /// </summary>
        Right = 3,
        /// <summary>
        /// 未指定（特别情况）
        /// </summary>
        None = 4,
    }

    public enum Order
    {
        /// <summary>
        /// 顺序
        /// </summary>
        ASC = 0,
        /// <summary>
        /// 降序
        /// </summary>
        DESC = 1
    }

    /// <summary>
    /// 关系运算符(where和having)
    /// </summary>
    [Flags]
    public enum WH
    {
        /// <summary>
        /// 等于
        /// </summary>
        EQ = 0x1,
        /// <summary>
        /// 小于
        /// </summary>
        LT = 0x2,
        /// <summary>
        /// 大于
        /// </summary>
        GT = 0x4,
        /// <summary>
        /// IN
        /// </summary>
        IN = 0x8,
        /// <summary>
        /// Between
        /// </summary>
        BT = 0x10,
        /// <summary>
        /// LIke
        /// </summary>
        LIKE = 0x20,
        /// <summary>
        /// 不使用参数
        /// </summary>
        NoParameter = 0x40,
        /// <summary>
        /// 小于等于
        /// </summary>
        LTE = 0x80,
        /// <summary>
        /// 大于等于
        /// </summary>
        GTE = 0x100,
        //为null
        NULL=0x200,
        //是否exsit
        Exists=0x400,
    }

    /// <summary>
    /// 逻辑运算符
    /// </summary>
    public enum Logic
    {
        And,
        OR
    }
}
