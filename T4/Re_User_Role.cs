using System;
using Azeroth.Nalu;

namespace T4
{
    public class Re_User_Role
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
        ///角色Id
        /// </summary>
        public String RoleId {set;get;}
        /// <summary>
        ///用户Id
        /// </summary>
        public String UserId {set;get;}
    }
}