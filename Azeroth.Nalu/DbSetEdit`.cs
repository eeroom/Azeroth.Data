using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Azeroth.Nalu.Node;

namespace Azeroth.Nalu
{
    public class DbSetEdit<T>:Table<T>,ICud
    {
        IEnumerable<T> lstEntity { set; get; }

        List<Column> selectNode { set; get; }

        Func<DbSetEdit<T>, T, WhereNode> whereNodeHandler { set; get; }
      
        internal DbSetEdit(IEnumerable<T> lst)
        {
            lstEntity = lst;
        }

        public DbSetEdit<T> Where(Func<DbSetEdit<T>,T,WhereNode> predicate)
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

        public DbSetEdit<T> UpdateColumn<S>(Expression<Func<T,S>> exp)
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

        int ICud.Execute(System.Data.Common.DbCommand cmd, ParseSqlContext context)
        {
          
            if (this.selectNode.Count <= 0)
                throw new ArgumentException("必须指定修改要赋值的列");
            if (this.whereNodeHandler == null)
                throw new ArgumentException("必须指定where条件");
            var lstParameter = this.selectNode.Select(x => context.CreateParameter(x.Name, null, false)).ToArray();
            List<string> lstSet = this.selectNode.Zip(lstParameter, (x, y) => $"{x.Name}={y.ParameterName}").ToList();
            string strset = string.Join(",", lstSet);
            var lstWrapper = lstParameter.Zip(this.selectNode, (parameter, sn) => new { parameter, sn }).ToList();
            int rst = 0;
            foreach (var entity in lstEntity)
            {
                lstWrapper.ForEach(x => x.parameter.Value = DictMapHandlerInternal[x.sn.Name].GetValueFromInstance(entity, null));
                context.DbParameters.Clear();
                var strwhere= this.whereNodeHandler(this, entity).Parse(context);
                cmd.CommandText = $"update {this.Name} set {strset} where {strwhere}";
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(lstParameter);
                cmd.Parameters.AddRange(context.DbParameters.ToArray());
                rst += cmd.ExecuteNonQuery();
            }
            return rst;
        }
    }
}
