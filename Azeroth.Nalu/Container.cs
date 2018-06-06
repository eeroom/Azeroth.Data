using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public abstract class Container:IContainer
    {
       
        protected DictionaryWrapper<string, IMapHandler> dictMapHandler;
        protected Func<ResovleContext, string> nameHandler;
        protected string nameNick;
        protected List<IComponentSELECT> lstSelectNode = new List<IComponentSELECT>();

        protected abstract RuntimeTypeHandle GetMetaInfo();
        protected abstract object CreateInstance(bool isCreateNull);

        DictionaryWrapper<string, IMapHandler> IContainer.DictMapHandler
        {
            get { return this.dictMapHandler; }
        }

        
        Func<ResovleContext, string> IContainer.NameHandler
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

       
        string IContainer.NameNick
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
        string IContainer.Name
        {
            get
            {
                return this.nameHandler(null);
            }
        }
      
        List<IComponentSELECT> IContainer.SelectNodes
        {
            get
            {
                return this.lstSelectNode;
            }
            set
            {
                this.lstSelectNode=value;
            }
        }

        object IContainer.CreateInstance(bool isCreateNull)
        {
            return this.CreateInstance(isCreateNull);
        }
    }
}
