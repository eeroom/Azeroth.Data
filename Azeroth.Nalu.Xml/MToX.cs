using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Xml.Linq;
namespace Azeroth.Nalu.Xml {
    public class MToX<T> {

        static string elName=typeof(T).Name;

        List<T> lstModel = new List<T>();

        Dictionary<string, Func<T, object>> lstHandler = new Dictionary<string, Func<T, object>>();

        public void AddModel(T m) {
            this.lstModel.Add(m);
        }

        public void AddModel(IEnumerable<T> lst) {
            this.lstModel.AddRange(lst);
        }

        public MToX<T> Element<S>(Expression<Func<T,S>> propertyToElement) {
            var me= propertyToElement as MemberExpression;
            var name = me.Member.Name;

            var body= Expression.Convert(propertyToElement.Body, typeof(object));

            var exp= Expression.Lambda<Func<T, object>>(body, propertyToElement.Parameters[0]);
            this.lstHandler.Add(name, exp.Compile());
            return this;
        }

        public List<XElement> ToList() {
            return this.lstModel.Select(x => this.Convert(x)).ToList();
        }

        public XElement Convert(T model) {
            var xe = new XElement(elName);
            foreach (var kv in lstHandler) {
                xe.Add(new XElement(kv.Key, kv.Value(model)));
            }
            return xe;
        }
    }
}
