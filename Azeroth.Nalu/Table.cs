using Azeroth.Nalu.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public abstract class Table : ITable {
        protected string NameNick {set; get; }
        protected string Name { set; get; }
        string ITable.Name {
            get {
                return this.Name;
            }
        }

        string ITable.NameNick {
            get {
                return this.NameNick;
            }
        }
    }

    public interface ITable {
        string NameNick {  get; }
        string Name {  get; }
    }
}
