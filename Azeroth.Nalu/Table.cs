using Azeroth.Nalu.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public abstract class Table
    {
        public string NameNick {protected set; get; }
        public string Name { protected set; get; }

    }
}
