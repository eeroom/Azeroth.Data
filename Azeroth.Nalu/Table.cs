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
        internal string NameNick { set; get; }
        internal string Name { set; get; }
    }
}
