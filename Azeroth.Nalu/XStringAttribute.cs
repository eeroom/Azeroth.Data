using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// 只是用于数据库的字符串的非空和长度校验，不做与业务有关的校验
    /// </summary>
    public class XStringAttribute:System.Attribute
    {
        public XStringAttribute(int max, bool canNull)
        {
            this.max = max;
            this.nullable = canNull;
        }

        int max;
        bool nullable;

        public bool Validate(string value, out string msg)
        {
            if (value == null && !nullable)
            {
                msg = "值不能为NULL";
                return false;
            }
            if (value != null && value.Length > max)
            {
                msg = string.Format("长度上限为{0},当前值为{1}",  max, value);
                return false;
            }
            msg = string.Empty;
            return true;
        }
    }
}
