﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu.Node
{
    public class SelectNode :NodeBase, ISelectNode
    {
        public SelectNode(IColumn column):base(column)
        {
            this.columNameNick = column.ColumnName;
            //column.Container.SelectNodes.Add(this);
        }

        protected string columNameNick;
        protected int colIndex;

        protected override string ToSQL(ResolveContext context)
        {
            if (this.column.FunctionCode != Azeroth.Nalu.Function.NONE || this.column.FunctionHandler != null || this.column.ColumnName != this.columNameNick)
                return string.Format("{0} AS {1}", this.column.ToSQL(context), this.columNameNick);//特殊情况的select
            return this.column.ToString();
        }

        string ISelectNode.ColumnNameNick
        {
            get {return this.columNameNick; }
            set { this.columNameNick = value; }
        }

        int ISelectNode.ColIndex
        {
            get { return this.colIndex; }
            set { this.colIndex = value; }
        }

    
    }
}
