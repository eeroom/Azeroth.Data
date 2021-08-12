using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Azeroth.Nalu.Node;
using System.Data.Common;

namespace Azeroth.Nalu
{
    public class DbSetAdd<T>:Table<T>,IExecuteNonQuery
    {
        string CommandText {set; get; }
        System.Data.Common.DbParameterCollection DbParameters { get;set; }

        IEnumerable<T> lstEntity { set; get; }

     

        List<Column> selectNode { set; get; }

        internal DbSetAdd(IEnumerable<T> lst)
        {
            lstEntity = lst;
        }

        public DbSetAdd<T> InsertColumn<S>(Expression<Func<T,S>> exp)
        {
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

        int IExecuteNonQuery.ExecuteNonQuery(DbCommand cmd, ParseSqlContext context)
        {
            int rst = 0;
            if (this.selectNode.Count <= 0)
                throw new ArgumentException("必须指定要新增赋值的列");
            List<string> lstcolName = this.selectNode.Select(x => x.Name).ToList();
            string strCol = string.Join(",", lstcolName);
            var lstParameter= this.selectNode.Select(x => context.CreateParameter(x.Name, null,false)).ToArray();
            var lstParameterName = lstParameter.Select(x => x.ParameterName).ToList();
            string strParamter = string.Join(",",lstParameterName);
            cmd.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", this.Name, strCol, strParamter);
            var lstWrapper= lstParameter.Zip(lstcolName, (parameter, name) => new { parameter, name }).ToList();
            foreach (var entity in lstEntity)
            {
                lstWrapper.ForEach(x => x.parameter.Value = DictMapHandlerInternal[x.name].GetValueFromInstance(entity, null));
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(lstParameter);
                rst += cmd.ExecuteNonQuery();
            }
            return rst;
        }
    }
}
