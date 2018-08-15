using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class Result
    {
        /// <summary>
        /// 执行成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 受影响行数
        /// </summary>
        public int Effect { get; set; }
        /// <summary>
        /// 消息提示
        /// </summary>
        public string Message { get; set; }

        public Result(int effect)
        {
            this.Effect = effect;
            this.Success = true;
        }

        public Result(bool success, string msg)
        {
            this.Success = success;
            this.Message = msg;
        }
    }
}
