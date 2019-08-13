using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Azeroth.Nalu
{
    public class DbCud<T>:Table<T>,ICud where T:class
    {
        public string CommandText {protected set; get; }
        public System.Data.Common.DbParameterCollection DbParameters { get;protected set; }
        public NodeWhere WHERE{set;get;}
        //public DbCud<T> Select(Column col)
        //{
        //    this.lstSelect.Add(new NodeSelect(col));
        //    return this;
        //}

        public DbCud<T> Select<S>(Expression<Func<T,S>> exp)
        {
            var lstcol= this.Cols(exp);
            this.lstSelect.AddRange(lstcol.Select(x => new NodeSelect(x)));
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

        private int Del(System.Data.Common.DbCommand cmd, ResolveContext context)
        {
            int rst = 0;
            if (values == null||values.Count()<1)
                return Del(cmd,context,null);
            foreach (var value in values)
            {
                rst += Del(cmd, context, value);
            } 
            return rst;
        }

        private int Del(System.Data.Common.DbCommand cmd, ResolveContext context, object value)
        {
            context.Parameters.Clear();
            context.Tag = value;
            string strwhere =((ISqlResolver)this.WHERE).ToSQL(context);
            if (!string.IsNullOrEmpty(strwhere))
                strwhere = " WHERE " + strwhere;
            cmd.CommandText = string.Format("DELETE FROM {0} {1}", this.nameHandler(context), strwhere);
            cmd.Parameters.Clear();
            cmd.Parameters.AddRange(context.Parameters.ToArray());
            this.CommandText = cmd.CommandText;
            this.DbParameters = cmd.Parameters; 
            return cmd.ExecuteNonQuery();
        }

        private int Edit(System.Data.Common.DbCommand cmd, ResolveContext context)
        {
            int rst = 0;
            if (this.lstSelect.Count <= 0)
                throw new ArithmeticException("必须指定修改要赋值的列");
            List<string> lstSet = this.lstSelect.Select(col => col.Column.ColumnName + "=" + context.Symbol + col.Column.ColumnName).ToList();
            string strSet = string.Join(",", lstSet);
            var dictParameter = this.lstSelect.ToDictionary(col => col.Column.ColumnName, col => cmd.CreateParameter());
            foreach (var kv in dictParameter)
                kv.Value.ParameterName = context.Symbol + kv.Key;
            foreach (var value in this.values)
            {
                this.lstSelect.ForEach(col => dictParameter[col.Column.ColumnName].Value = this.dictMapHandler[col.Column.ColumnName].GetValueFromInstance(value, null));
                context.Parameters.Clear();
                context.Tag = value;
                string strwhere = ((INode)this.WHERE).ToSQL(context);
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

        private int Add(System.Data.Common.DbCommand cmd, ResolveContext context)
        {
            int rst = 0;
            List<string> lstcolName;
            cmd.CommandText= Add(context,out  lstcolName);
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

        private string Add(ResolveContext context, out List<string> lstcolName)
        {
            if (this.lstSelect.Count <= 0)
                throw new ArgumentException("必须指定要新增赋值的列");
           lstcolName = this.lstSelect.Select(x => x.Column.ColumnName).ToList();
            string strCol = string.Join(",", lstcolName);
            string strParamter = context.Symbol + string.Join("," + context.Symbol, lstcolName);
            return string.Format("INSERT INTO {0} ({1}) VALUES ({2})", this.nameHandler(context), strCol, strParamter);
        }

        int ICud.Execute(System.Data.Common.DbCommand cmd, ResolveContext context)
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


        public Table<T> InsertRange(IEnumerable<T> value)
        {
            this.OptCmd = Cmd.Add;
            this.values = value;
            return this;
        }

        public Table<T> Insert(T value)
        {
            this.OptCmd = Cmd.Add;
            this.values = new List<T>() { value};
            return this;
        }

        public Table<T> UpdateRange(IEnumerable<T> value)
        {
            this.OptCmd = Cmd.Edit;
            this.values = value;
            return this;
        }

        public Table<T> Update(T value)
        {
            this.OptCmd = Cmd.Edit;
            this.values = new List<T>() { value};
            return this;
        }

        public Table<T> RemoveRange(IEnumerable<T> value)
        {
            this.OptCmd = Cmd.Del;
            this.values = value;
            return this;
        }

        public Table<T> Remove(T value)
        {
            this.OptCmd = Cmd.Del;
            this.values = new List<T>() {value };
            return this;
        }

        //bool ICud.Validate(out string msg)
        //{
        //    msg = string.Empty;
        //    if (this.OptCmd != Cmd.Add && this.OptCmd != Cmd.Edit)
        //        return true;
        //    foreach (object value in this.values)
        //    {
        //        foreach (var node in this.lstSelect)
        //        {
        //            if (!this.dictMapHandler[node.Column.ColumnName].ValidateInstance(value, out msg))
        //            {
        //                msg = string.Format("{0}\r\n类型名称={1}\r\n属性名称={2}", msg, Type.GetTypeFromHandle(GetMetaInfo()).FullName, node.Column.ColumnName);
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}
    }
}
