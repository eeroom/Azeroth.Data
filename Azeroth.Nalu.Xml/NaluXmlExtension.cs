using Azeroth.Nalu.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Linq.Expressions;
namespace System.Linq {
    public static class NaluXmlExtension {

        public static MToX<T> ToElement<T>(this IEnumerable<T> lst) {
            var mtx = new Azeroth.Nalu.Xml.MToX<T>();
            mtx.AddModel(lst);
            return mtx;
        }

        public static XToM<T> ToModel<T>(this IEnumerable<XElement> lst) where T :class,new() {
            var xtm = new Azeroth.Nalu.Xml.XToM<T>(lst);
            return xtm;
        }

        public static IEnumerable<XElement> WhereByElementValue<T>(this IEnumerable<XElement> lst,Expression<Func<T,object>> getValueHandler,Func<string,bool> predicate) {
            return lst;
        }

        public static XElement SetElementValue<T>(this XElement xe,Expression<Func<T,object>> getValueHandler) {
            return xe;
        }
    }
}
