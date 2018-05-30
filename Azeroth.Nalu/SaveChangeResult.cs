using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class SaveChangeResult
    {
        public bool IsError { get; set; }
        public int Value { get; set; }
        public string Msg { get; set; }

        public SaveChangeResult(int value)
        {
            this.Value = value;
        }

        public SaveChangeResult(bool iserror, string msg)
        {
            this.IsError = iserror;
            this.Msg = msg;
        }
    }
}
