using System;
using Azeroth.Nalu;

namespace T4
{
    public class Tb_Role
    {
        /// <summary>
        ///创建时间
        /// </summary>
        public DateTime CreateTime {set;get;}
        /// <summary>
        ///创建人
        /// </summary>
        public String CreateUser {set;get;}
        /// <summary>
        ///主键
        /// </summary>
        public String Id {set;get;}
        /// <summary>
        ///修改时间
        /// </summary>
        public DateTime ModifyTime {set;get;}
        /// <summary>
        ///修改人
        /// </summary>
        public String ModifyUser {set;get;}
        /// <summary>
        ///名称
        /// </summary>
        public String Name {set;get;}
    }
}