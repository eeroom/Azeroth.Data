using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class Result
    {
        public bool Success { get; set; }
        public int Value { get; set; }
        public string Msg { get; set; }

        public Result(int value)
        {
            this.Value = value;
        }

        public Result(bool success, string msg)
        {
            this.Success = success;
            this.Msg = msg;
        }
    }
}
