using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public abstract class Table:ITable
    {
       
        protected DictionaryWrapper<string, IMapHandler> dictMapHandler;
        protected Func<ResolveContext, string> nameHandler;
        protected string nameNick;
        protected List<INodeSelect> lstSelect = new List<INodeSelect>();

        protected abstract RuntimeTypeHandle GetMetaInfo();
        protected abstract object CreateInstance(bool isCreateNull);

        DictionaryWrapper<string, IMapHandler> ITable.DictMapHandler
        {
            get { return this.dictMapHandler; }
        }

        
        Func<ResolveContext, string> ITable.NameHandler
        {
            get
            {
                return this.nameHandler;
            }
            set
            {
                this.nameHandler=value;
            }
        }

       
        string ITable.NameNick
        {
            get
            {
                return this.nameNick;
            }
            set
            {
                this.nameNick=value;
            }
        }
        string ITable.Name
        {
            get
            {
                return this.nameHandler(null);
            }
        }
      
        List<INodeSelect> ITable.Select
        {
            get
            {
                return this.lstSelect;
            }
            set
            {
                this.lstSelect=value;
            }
        }

        object ITable.CreateInstance(bool isCreateNull)
        {
            return this.CreateInstance(isCreateNull);
        }
    }
}
