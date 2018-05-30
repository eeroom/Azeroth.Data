using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu.Excel
{
    public class ListWrapper<T>
    {
        public IList<T> Value { get; private set; }
        public bool OnError { get;private set; }
        public string Message { get;private set; }
        public ListWrapper(IList<T> lst, bool onError, string msg)
        {
            this.Value = lst;
            this.OnError = onError;
            this.Message = msg;
        }

        public override string ToString()
        {
            return "结果："+(!OnError).ToString()+"，"+this.Message;
        }
    }
}
