using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu.Excel
{
    public interface IMapHandler
    {
        void SetValueToInstance(object instance,NPOI.SS.UserModel.ICell cell);

        void SetValueToCell(object instance, NPOI.SS.UserModel.ICell cell);

        void SetEvaHandler(NPOI.SS.UserModel.IFormulaEvaluator eva);
    }
}
