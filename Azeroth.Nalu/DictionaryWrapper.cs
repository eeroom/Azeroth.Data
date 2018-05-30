using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// 因为字典可以添加移除修改里面的值，避免字典内的值被修改，做一层wrapper，
    /// 相当于只读的字典
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class DictionaryWrapper<K,V> : IEnumerable<KeyValuePair<K,V>>
    {
        Dictionary<K,V> dictPropWrapper;

        public DictionaryWrapper(Dictionary<K,V> dict)
        {
            this.dictPropWrapper = dict;
        }

        /// <summary>
        /// 只暴露get，避免dictPropWrapper内的值被修改
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public V this[K key] 
        {
            get {
                return this.dictPropWrapper[key];
            }
        }

        public Dictionary<K, V>.KeyCollection Keys
        {
            get
            {
                return this.dictPropWrapper.Keys;
            }
        }

        public IEnumerator<KeyValuePair<K,V>> GetEnumerator()
        {
            return this.dictPropWrapper.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.dictPropWrapper.GetEnumerator();
        }
    }
}
