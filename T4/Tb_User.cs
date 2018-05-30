using System;
using Azeroth.Nalu;

namespace T4
{
    public class Tb_User
    {
        /// <summary>
        ///账户
        /// </summary>
        public String Account {set;get;}
        /// <summary>
        ///头像
        /// </summary>
        [XString(200,true)]
        public String Avatar {set;get;}
        /// <summary>
        ///创建时间
        /// </summary>
        public DateTime CreateTime {set;get;}
        /// <summary>
        ///创建人
        /// </summary>
        public String CreateUser {set;get;}
        /// <summary>
        ///删除标记
        /// </summary>
        public Boolean DeleteMark {set;get;}
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
        ///昵称
        /// </summary>
        [XString(50,true)]
        public String NickName {set;get;}
        /// <summary>
        ///真实姓名
        /// </summary>
        public String RealName {set;get;}
        /// <summary>
        ///登陆密码
        /// </summary>
        public String UserPwd {set;get;}
        /// <summary>
        ///历史密码
        /// </summary>
        public String UserPwdHistory {set;get;}
        /// <summary>
        ///性别
        /// </summary>
        public Nullable<Int32> Gender {set;get;}
    }
}