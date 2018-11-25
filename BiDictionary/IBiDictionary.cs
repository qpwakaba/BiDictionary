using System.Collections.Generic;

namespace qpwakaba
{
    public interface IBiDictionary<TKey, TValue> : 
        ICollection<KeyValuePair<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>
    {
        TValue this[TKey key] { get; set; }
        ICollection<TKey> Keys { get; }
        ICollection<TValue> Values { get; }
        void Add(TKey key, TValue value);
        bool ContainsKey(TKey key);
        bool ContainsValue(TValue key);
        bool Remove(TKey key);
        bool TryGetValue(TKey key, out TValue value);

        BiDictionary<TValue, TKey> Inverse();

    }
}
