using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu.Excel
{
    public class MapHandler<T,M>:IMapHandler
    {
        Func<NPOI.SS.UserModel.ICell,NPOI.SS.UserModel.IFormulaEvaluator, M> getvalueFromCell;

        Func<T, M> getvalueFromInstance;

        Action<T, M> setvalueToInstance;

        Action<NPOI.SS.UserModel.ICell,M> setvalueToCell;

        NPOI.SS.UserModel.IFormulaEvaluator eva;

        public MapHandler(System.Linq.Expressions.Expression<Func<T, M>> selector)
        {
            this.getvalueFromInstance = selector.Compile();
            System.Type metaM = typeof(M);
            //x,value=>x.Name=value
            var valueParameter = Expression.Parameter(metaM);
            var body = Expression.Assign(selector.Body, valueParameter);
            this.setvalueToInstance = Expression.Lambda<Action<T, M>>(body, selector.Parameters[0], valueParameter).Compile();
            this.setvalueToCell = (Action<NPOI.SS.UserModel.ICell,M>)CellValueHandlerFactory.CreateSetValueHandler<M>();
            this.getvalueFromCell = (Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, M>)CellValueHandlerFactory.CreateGetValueHandler<M>();
        }

        public MapHandler(System.Linq.Expressions.Expression<Func<T, M>> selector, Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, M> getvalueFromCellHandler)
        {
            this.getvalueFromInstance = selector.Compile();
            System.Type metaM = typeof(M);
            //x,value=>x.Name=value
            var valueParameter = Expression.Parameter(metaM);
            var body = Expression.Assign(selector.Body, valueParameter);
            this.setvalueToInstance = Expression.Lambda<Action<T, M>>(body, selector.Parameters[0], valueParameter).Compile();
            this.setvalueToCell = (Action<NPOI.SS.UserModel.ICell, M>)CellValueHandlerFactory.CreateSetValueHandler<M>();
            this.getvalueFromCell = getvalueFromCellHandler;
        }

        public MapHandler(System.Linq.Expressions.Expression<Func<T, M>> selector, Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, M> getvalueFromCellHandler, Action<NPOI.SS.UserModel.ICell, M> setvalueToCellHandler)
        {
            this.getvalueFromInstance = selector.Compile();
            System.Type metaM = typeof(M);
            //x,value=>x.Name=value
            var valueParameter = Expression.Parameter(metaM);
            var body = Expression.Assign(selector.Body, valueParameter);
            this.setvalueToInstance = Expression.Lambda<Action<T, M>>(body, selector.Parameters[0], valueParameter).Compile();
            this.setvalueToCell = setvalueToCellHandler;
            this.getvalueFromCell = getvalueFromCellHandler;
        }

        public MapHandler(System.Linq.Expressions.Expression<Func<T, M>> selector, Action<NPOI.SS.UserModel.ICell, M> setvalueToCellHandler)
        {
            this.getvalueFromInstance = selector.Compile();
            System.Type metaM = typeof(M);
            //x,value=>x.Name=value
            var valueParameter = Expression.Parameter(metaM);
            var body = Expression.Assign(selector.Body, valueParameter);
            this.setvalueToInstance = Expression.Lambda<Action<T, M>>(body, selector.Parameters[0], valueParameter).Compile();
            this.setvalueToCell = setvalueToCellHandler;
            this.getvalueFromCell = (Func<NPOI.SS.UserModel.ICell, NPOI.SS.UserModel.IFormulaEvaluator, M>)CellValueHandlerFactory.CreateGetValueHandler<M>();
        }

        public void SetValueToInstance(object instance, NPOI.SS.UserModel.ICell cell)
        {
            M value= getvalueFromCell(cell,eva);
            setvalueToInstance((T)instance,value);

        }

        public void SetValueToCell(object instance, NPOI.SS.UserModel.ICell cell)
        {
            M value= getvalueFromInstance((T)instance);
            setvalueToCell(cell,value);
        }

        void IMapHandler.SetEvaHandler(NPOI.SS.UserModel.IFormulaEvaluator eva)
        {
            this.eva = eva;
        }
    }
}
