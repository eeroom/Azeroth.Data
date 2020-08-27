using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu.Excel
{
    public class SheetValue<T>
    {
        public List<T> Value { set; get; }
        public bool OnError { get; set; }
        public List<string> Message { get; set; }
        public SheetValue()
        {
            this.Value = new List<T>();
            this.OnError = false;
            this.Message = new List<string>();
        }

        public SheetValue<T> AddMessage(string msg)
        {
            this.Message.Add(msg);
            this.OnError = true;
            return this;
        }
    }
}
