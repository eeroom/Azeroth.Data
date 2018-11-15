using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    public interface ICud
    {
        int Execute(System.Data.Common.DbCommand cmd,ResolveContext context);

        bool Validate(out string msg);

    }
}
