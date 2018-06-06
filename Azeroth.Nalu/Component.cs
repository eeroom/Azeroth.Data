using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public  class Component:IComponent
    {
        protected IColumn column;
        public Component()
        { 
        
        }
        public Component(IColumn column)
        {
            this.column = column;
        }

        IColumn IComponent.Column
        {
            get { return this.column; }
        }

        string IConvertible.ToSQL(ResovleContext context) {
            return this.ToSQL(context);
        }

        protected virtual string ToSQL(ResovleContext context)
        {
            return this.column.ToSQL(context);
        }
    }
}
