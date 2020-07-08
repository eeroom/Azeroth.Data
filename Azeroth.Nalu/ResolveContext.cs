using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public class ResolveContext
    {
        public ResolveContext(string symbol,Func<System.Data.Common.DbParameter> createParameter)
        {
            this.Symbol = symbol;
            this.Parameters = new List<System.Data.Common.DbParameter>(8);
            //this.CanCTE = true;
            this.CreateParameter = createParameter;
        }

        int colIndex = 0;
        int setIndex = 0;
        int index = 0;
        /// <summary>
        /// 参数的符号，msssql-@  oracle-:
        /// </summary>
        public string Symbol { get;private set; }
        /// <summary>
        /// 本次请求的所有参数
        /// </summary>
        public List<System.Data.Common.DbParameter> Parameters { get;private set; }

        //public bool CanCTE { set; get; }

        public object Tag { set; get; }
        /// <summary>
        /// 创建DbParameter的方法
        /// </summary>
        public Func<System.Data.Common.DbParameter> CreateParameter { get; private set; }

        /// <summary>
        /// 列别名的序号
        /// </summary>
        /// <returns></returns>
        public int NextColIndex()
        {
            return this.colIndex++;
        }

        /// <summary>
        /// 表，CTE 的序号
        /// </summary>
        /// <returns></returns>
        public int NextTableIndex()
        {
            return this.setIndex++;
        }
        /// <summary>
        /// SQL参数的序号
        /// </summary>
        /// <returns></returns>
        public int NextParameterIndex()
        {

            return this.index++;
        }
    }
}
