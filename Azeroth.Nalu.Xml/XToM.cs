using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Linq.Expressions;
namespace Azeroth.Nalu.Xml {
    public class XToM<T> where T : class, new() {

        IEnumerable<XElement> lstElement;

        Dictionary<string, Action<T, string>> dictHandler = new Dictionary<string, Action<T, string>>();
        public XToM(IEnumerable<XElement> lst) {
            this.lstElement = lst;
        }

        public XToM<T> Map<S>(Expression<Func<T, S>> el) {
            var me = el.Body as MemberExpression;
            var xname = me.Member.Name;
            Expression<Func<string, S>> convertHandler = x => (S)System.Convert.ChangeType(x, typeof(S));
            var body = Expression.Assign(el.Body, convertHandler.Body);
            var handler = Expression.Lambda<Action<T, string>>(body, el.Parameters[0], convertHandler.Parameters[0]).Compile();
            this.dictHandler.Add(xname, handler);
            return this;
        }

        public XToM<T> Map<S>(Expression<Func<T, S>> el, Expression<Func<string, S>> transfer) {
            var me = el.Body as MemberExpression;
            var xname = me.Member.Name;
            var body = Expression.Assign(el.Body, transfer.Body);
            var handler = Expression.Lambda<Action<T, string>>(body, el.Parameters[0], transfer.Parameters[0]).Compile();
            this.dictHandler.Add(xname, handler);
            return this;
        }

        public List<T> ToList() {
            return this.lstElement.Select(x => this.Convert(x)).ToList();
        }

        private T Convert(XElement x) {
            var obj = new T();
            foreach (var kv in this.dictHandler) {
                kv.Value(obj, x.Element(kv.Key).Value);
            }
            return obj;
        }
    }
}
