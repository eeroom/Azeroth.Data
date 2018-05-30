using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Azeroth.Nalu
{
    public abstract class DbSet:IDbSet
    {
       
        protected DictionaryWrapper<string, IMapHandler> dictMapHandler;
        protected Func<ResovleContext, string> nameHandler;
        protected string nameNick;
        protected List<ISelectNode> lstSelectNode = new List<ISelectNode>();

        protected abstract RuntimeTypeHandle GetMetaInfo();
        protected abstract object CreateInstance(bool isCreateNull);

        DictionaryWrapper<string, IMapHandler> IDbSet.DictMapHandler
        {
            get { return this.dictMapHandler; }
        }

        
        Func<ResovleContext, string> IDbSet.NameHandler
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

       
        string IDbSet.NameNick
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
        string IDbSet.Name
        {
            get
            {
                return this.nameHandler(null);
            }
        }
      
        List<ISelectNode> IDbSet.SelectNodes
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

        object IDbSet.CreateInstance(bool isCreateNull)
        {
            return this.CreateInstance(isCreateNull);
        }
    }
}
