using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Azeroth.Nalu.Node;

namespace Azeroth.Nalu
{
    public class DbCud<T>:Table<T>,ICud
    {
        string CommandText {set; get; }
        System.Data.Common.DbParameterCollection DbParameters { get;set; }

        List<T> lstEntity { set; get; }

        Cmd cmd { set; get; }

        List<Column> selectNode { set; get; }

        Func<DbCud<T>, T, WhereNode> whereNodeHandler { set; get; }
        internal DbCud(T entity,Cmd cmd):this(new List<T>() { entity},cmd)
        {
         

        }

        internal DbCud(IEnumerable<T> lst, Cmd cmd)
        {
            this.lstEntity = new List<T>();
            this.lstEntity.AddRange(lst);
            this.cmd = cmd;
            switch (this.cmd)
            {
                case Cmd.Add:
                    this.selectNode = new List<Column>();
                    break;
                case Cmd.Del:
                    break;
                case Cmd.Edit:
                    break;
                default:
                    throw new ArgumentException("只支持增、删、改");
            }
        }

        public DbCud<T> Where(Func<DbCud<T>,T,WhereNode> predicate)
        {
            this.whereNodeHandler = predicate;
            return this;
        }

        public Column<T, S> Col<S>(Expression<Func<T, S>> exp)
        {
            var colmem = exp.Body as MemberExpression;
            if (colmem == null)
                throw new ArgumentException("不支持的表达式");
            var col = new Column<T, S>(this, colmem.Member.Name);
            return col;
        }

        public DbCud<T> Select<S>(Expression<Func<T,S>> exp)
        {
            if (this.cmd != Cmd.Add)
                throw new ArgumentException("只有新增数据支持此方法");
            List<string> lstName = new List<string>();
            var colmem = exp.Body as MemberExpression;
            if (colmem != null)
                lstName.Add(colmem.Member.Name);
            else
            {
                var colmem2 = exp.Body as NewExpression;
                if (colmem2 != null)
                    lstName.AddRange(colmem2.Members.Select(x => x.Name));
            }
            if (lstName.Count < 1)
                throw new ArgumentException("不支持的表达式");
            var lst = lstName.Select(x => new Column(this, x))
                .ToList();
            this.selectNode.AddRange(lst);
            return this;
        }

        private int Del(System.Data.Common.DbCommand cmd, ParseSqlContext context)
        {
            int rst = 0;
            foreach (var value in lstEntity)
            {
                context.DbParameters.Clear();
                var wherenode = this.whereNodeHandler(this, value);
                string strwhere = wherenode.Parse(context);
                if (!string.IsNullOrEmpty(strwhere))
                    strwhere = " WHERE " + strwhere;
                cmd.CommandText = string.Format("DELETE FROM {0} {1}", this.Name, strwhere);
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(context.DbParameters.ToArray());
                this.CommandText = cmd.CommandText;
                this.DbParameters = cmd.Parameters;
                rst += cmd.ExecuteNonQuery();
            } 
            return rst;
        }

        private int Edit(System.Data.Common.DbCommand cmd, ParseSqlContext context)
        {
            int rst = 0;
            if (this.selectNode.Count <= 0)
                throw new ArithmeticException("必须指定修改要赋值的列");
            List<string> lstSet = this.selectNode.Select(col => col.Name + "=" + context.DbParameterNamePrefix + col.Name).ToList();
            string strSet = string.Join(",", lstSet);
            var dictParameter = this.selectNode.ToDictionary(col => col.Name, col => cmd.CreateParameter());
            foreach (var kv in dictParameter)
                kv.Value.ParameterName = context.DbParameterNamePrefix + kv.Key;
            foreach (var value in this.lstEntity)
            {
                this.selectNode.ForEach(col => dictParameter[col.Name].Value = DictMapHandlerInternal[col.Name].GetValueFromInstance(value, null));
                var wherenode= this.whereNodeHandler(this, value);
                string strwhere = wherenode.Parse(context);
                if (!string.IsNullOrEmpty(strwhere))
                    strwhere = " WHERE " + strwhere;
                context.DbParameters.AddRange(dictParameter.Values);
                cmd.CommandText = string.Format("UPDATE {0} SET {1} {2}", this.Name, strSet, strwhere);
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(context.DbParameters.ToArray());
                rst += cmd.ExecuteNonQuery();
            }
            this.CommandText = cmd.CommandText;
            this.DbParameters = cmd.Parameters;
            return rst;
        }

        private int Add(System.Data.Common.DbCommand cmd, ParseSqlContext context)
        {
            int rst = 0;
            if (this.selectNode.Count <= 0)
                throw new ArgumentException("必须指定要新增赋值的列");
            List<string> lstcolName = this.selectNode.Select(x => x.Name).ToList();
            string strCol = string.Join(",", lstcolName);
            string strParamter = context.DbParameterNamePrefix + string.Join("," + context.DbParameterNamePrefix, lstcolName);
            cmd.CommandText= string.Format("INSERT INTO {0} ({1}) VALUES ({2})", this.Name, strCol, strParamter);

            Dictionary<string, System.Data.Common.DbParameter> dictParameter = lstcolName.ToDictionary(x => x, x => cmd.CreateParameter());
            foreach (var kv in dictParameter)
                kv.Value.ParameterName = context.DbParameterNamePrefix + kv.Key;
            foreach (var value in lstEntity)
            {
                cmd.Parameters.Clear();
                lstcolName.ForEach(pName =>
                {
                    dictParameter[pName].Value = DictMapHandlerInternal[pName].GetValueFromInstance(value, null);
                    cmd.Parameters.Add(dictParameter[pName]);
                });
                rst += cmd.ExecuteNonQuery();
            }
            this.CommandText = cmd.CommandText;
            this.DbParameters = cmd.Parameters;
            return rst;
        }

        int ICud.Execute(System.Data.Common.DbCommand cmd, ParseSqlContext context)
        {
            int rst = 0;
            switch (this.cmd)
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
                    throw new ArgumentException("不支持的操作");
            }
            return rst;
        }
    }
}
