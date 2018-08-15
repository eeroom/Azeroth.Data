using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Azeroth.Nalu
{
    public class DbCud<T>:Container<T>,ICud
    {
        public string CommandText {protected set; get; }
        public System.Data.Common.DbParameterCollection DbParameters { get;protected set; }
        public NodeWhere WH{set;get;}
        public DbCud<T> Select(Column col)
        {
            this.lstSelectNode.Add(new NodeSelect(col));
            return this;
        }

        public DbCud<T> Select(IList<Column> cols)
        {
            this.lstSelectNode.AddRange(cols.Select(x => new NodeSelect(x)));
            return this;
        }

        //public void Where(ComponentWHERE predicate)
        //{
        //    this.WH = predicate;
        //}

        /// <summary>
        /// 增、删、改对应的行数据
        /// </summary>
        protected IEnumerable<T> values;

        protected Cmd OptCmd { set; get; }

        private int Del(System.Data.Common.DbCommand cmd, ResovleContext context)
        {
            int rst = 0;
            if (values == null||values.Count()<1)
                return Del(cmd,context,null);
            foreach (var value in values)
                rst += Del(cmd,context,value);

            return rst;
        }

        private int Del(System.Data.Common.DbCommand cmd, ResovleContext context, object value)
        {
            context.Parameters.Clear();
            context.Tag = value;
            string strwhere =((INode)this.WH).ToSQL(context);
            if (!string.IsNullOrEmpty(strwhere))
                strwhere = " WHERE " + strwhere;
            cmd.CommandText = string.Format("DELETE FROM {0} {1}", this.nameHandler(context), strwhere);
            cmd.Parameters.Clear();
            cmd.Parameters.AddRange(context.Parameters.ToArray());
            this.CommandText = cmd.CommandText;
            this.DbParameters = cmd.Parameters; 
            return cmd.ExecuteNonQuery();
        }

        private int Edit(System.Data.Common.DbCommand cmd, ResovleContext context)
        {
            int rst = 0;
            List<string> lstSet = this.lstSelectNode.Select(col => col.Column.ColumnName + "=" + context.Symbol + col.Column.ColumnName).ToList();
            string strSet = string.Join(",", lstSet);
            var dictParameter = this.lstSelectNode.ToDictionary(col => col.Column.ColumnName, col => cmd.CreateParameter());
            foreach (var kv in dictParameter)
                kv.Value.ParameterName = context.Symbol + kv.Key;
            foreach (var value in this.values)
            {
                this.lstSelectNode.ForEach(col => dictParameter[col.Column.ColumnName].Value = this.dictMapHandler[col.Column.ColumnName].GetValueFromInstance(value, null));
                context.Parameters.Clear();
                context.Tag = value;
                string strwhere = ((INode)this.WH).ToSQL(context);
                if (!string.IsNullOrEmpty(strwhere))
                    strwhere = " WHERE " + strwhere;
                context.Parameters.AddRange(dictParameter.Values);
                cmd.CommandText = string.Format("UPDATE {0} SET {1} {2}", this.nameHandler(context), strSet, strwhere);
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(context.Parameters.ToArray());
                rst += cmd.ExecuteNonQuery();
            }
            this.CommandText = cmd.CommandText;
            this.DbParameters = cmd.Parameters; 
            return rst;
        }

        private int Add(System.Data.Common.DbCommand cmd, ResovleContext context)
        {
            int rst = 0;
            List<string> lstcolName;
            cmd.CommandText= Add(cmd,context,out  lstcolName);
            Dictionary<string, System.Data.Common.DbParameter> dictParameter = lstcolName.ToDictionary(x => x, x => cmd.CreateParameter());
            foreach (var kv in dictParameter)
                kv.Value.ParameterName = context.Symbol + kv.Key;
            foreach (var value in values)
            {
                cmd.Parameters.Clear();
                lstcolName.ForEach(pName =>
                {
                    dictParameter[pName].Value = this.dictMapHandler[pName].GetValueFromInstance(value, null);
                    cmd.Parameters.Add(dictParameter[pName]);
                });
                rst += cmd.ExecuteNonQuery();
            }
            this.CommandText = cmd.CommandText;
            this.DbParameters = cmd.Parameters;
            return rst;
        }

        private string Add(System.Data.Common.DbCommand cmd, ResovleContext context, out List<string> lstcolName)
        {
           lstcolName = this.lstSelectNode.Select(x => x.Column.ColumnName).ToList();
            string strCol = string.Join(",", lstcolName);
            string strParamter = context.Symbol + string.Join("," + context.Symbol, lstcolName);
            return string.Format("INSERT INTO {0} ({1}) VALUES ({2})", this.nameHandler(context), strCol, strParamter);
        }

        int ICud.Execute(System.Data.Common.DbCommand cmd, ResovleContext context)
        {
            int rst = 0;
            switch (this.OptCmd)
            {
                case Cmd.Add:
                    rst = Add(cmd, context);
                    break;
                case Cmd.Edit:
                    rst = Edit(cmd, context);
                    break;
                case Cmd.Del:
                    rst = Del(cmd, context);
                    break;
                default:
                    throw new ArgumentException("NoQuery操作必须属于增、删、改");
            }
            return rst;
        }


        public Container<T> Add(IEnumerable<T> value)
        {
            this.OptCmd = Cmd.Add;
            this.values = value;
            return this;
        }

        public Container<T> Add(params T[] value)
        {
            this.OptCmd = Cmd.Add;
            this.values = value;
            return this;
        }

        public Container<T> Edit(IEnumerable<T> value)
        {
            this.OptCmd = Cmd.Edit;
            this.values = value;
            return this;
        }

        public Container<T> Edit(params T[] value)
        {
            this.OptCmd = Cmd.Edit;
            this.values = value;
            return this;
        }

        public Container<T> Del(IEnumerable<T> value)
        {
            this.OptCmd = Cmd.Del;
            this.values = value;
            return this;
        }

        public Container<T> Del(params T[] value)
        {
            this.OptCmd = Cmd.Del;
            this.values = value;
            return this;
        }

        bool ICud.Validate(out string msg)
        {
            msg = string.Empty;
            if (this.OptCmd != Cmd.Add && this.OptCmd != Cmd.Edit)
                return true;
            foreach (object value in this.values)
            {
                foreach (var node in this.lstSelectNode)
                {
                    if (!this.dictMapHandler[node.Column.ColumnName].ValidateInstance(value, out msg))
                    {
                        msg = string.Format("{0}\r\n类型名称={1}\r\n属性名称={2}", msg, Type.GetTypeFromHandle(GetMetaInfo()).FullName, node.Column.ColumnName);
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
