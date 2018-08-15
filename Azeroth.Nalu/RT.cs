using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// NoQuery的执行结果
    /// </summary>
    public class RT
    {
        /// <summary>
        /// 执行成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 受影响行数
        /// </summary>
        public int Effected { get; set; }
        /// <summary>
        /// 消息提示
        /// </summary>
        public string Message { get; set; }

        public RT(int effected)
        {
            this.Effected = effected;
            this.Success = true;
        }

        public RT(bool success, string msg)
        {
            this.Success = success;
            this.Message = msg;
        }
    }
}
