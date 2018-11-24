using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace qpwakaba
{
    public class BiDictionary<TKey, TValue> :
        IBiDictionary<TKey, TValue>,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>>,
        ISerializable
    {
        private readonly Dictionary<TKey, TValue> normal;
        private readonly Dictionary<TValue, TKey> reverse;
        private readonly BiDictionary<TValue, TKey> reverseBiDictionary;

        #region private constructors
        private BiDictionary(BiDictionary<TValue, TKey> normal)
        {
            this.normal = normal.reverse;
            this.reverse = normal.normal;
            this.reverseBiDictionary = normal;
        }
        #endregion
        #region public constructors
        public BiDictionary()
        {
            this.normal = new Dictionary<TKey, TValue>();
            this.reverse = new Dictionary<TValue, TKey>();
            this.reverseBiDictionary = new BiDictionary<TValue, TKey>(this);
        }

        public BiDictionary(int capacity)
        {
            this.normal = new Dictionary<TKey, TValue>(capacity);
            this.reverse = new Dictionary<TValue, TKey>(capacity);
            this.reverseBiDictionary = new BiDictionary<TValue, TKey>(this);
        }

        public BiDictionary(IEqualityComparer<TKey> comparer)
        {
            this.normal = new Dictionary<TKey, TValue>(comparer);
            this.reverse = new Dictionary<TValue, TKey>();
            this.reverseBiDictionary = new BiDictionary<TValue, TKey>(this);
        }

        public BiDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.normal = new Dictionary<TKey, TValue>(dictionary.Count);
            this.reverse = new Dictionary<TValue, TKey>(dictionary.Count);
            foreach (var kv in dictionary)
            {
                this.Add(kv.Key, kv.Value);
            }
            this.reverseBiDictionary = new BiDictionary<TValue, TKey>(this);
        }

        public BiDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            this.normal = new Dictionary<TKey, TValue>(capacity, comparer);
            this.reverse = new Dictionary<TValue, TKey>(capacity);
            this.reverseBiDictionary = new BiDictionary<TValue, TKey>(this);
        }

        public BiDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            this.normal = new Dictionary<TKey, TValue>(dictionary.Count, comparer);
            this.reverse = new Dictionary<TValue, TKey>(dictionary.Count);
            foreach (var kv in dictionary)
            {
                this.Add(kv.Key, kv.Value);
            }
            this.reverseBiDictionary = new BiDictionary<TValue, TKey>(this);
        }

        public BiDictionary(IEqualityComparer<TValue> comparer)
        {
            this.normal = new Dictionary<TKey, TValue>();
            this.reverse = new Dictionary<TValue, TKey>(comparer);
            this.reverseBiDictionary = new BiDictionary<TValue, TKey>(this);
        }

        public BiDictionary(IEqualityComparer<TKey> comparerK, IEqualityComparer<TValue> comparerV)
        {
            this.normal = new Dictionary<TKey, TValue>(comparerK);
            this.reverse = new Dictionary<TValue, TKey>(comparerV);
            this.reverseBiDictionary = new BiDictionary<TValue, TKey>(this);
        }

        public BiDictionary(int capacity, IEqualityComparer<TKey> comparerK, IEqualityComparer<TValue> comparerV)
        {
            this.normal = new Dictionary<TKey, TValue>(capacity, comparerK);
            this.reverse = new Dictionary<TValue, TKey>(capacity, comparerV);
            this.reverseBiDictionary = new BiDictionary<TValue, TKey>(this);
        }

        public BiDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparerK, IEqualityComparer<TValue> comparerV)
        {
            this.normal = new Dictionary<TKey, TValue>(dictionary.Count, comparerK);
            this.reverse = new Dictionary<TValue, TKey>(dictionary.Count, comparerV);
            foreach (var kv in dictionary)
            {
                this.Add(kv.Key, kv.Value);
            }
            this.reverseBiDictionary = new BiDictionary<TValue, TKey>(this);
        }
        #endregion


        public BiDictionary<TValue, TKey> Reverse() => this.reverseBiDictionary;

        public TValue this[TKey key]
        {
            get => this.normal[key];
            set
            {
                this.Remove(key);
                this.Add(key, value);
            }
        }

        public ICollection<TKey> Keys => this.normal.Keys;
        public ICollection<TValue> Values => this.normal.Values;
        public int Count => this.normal.Count;
        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            if (this.normal.ContainsKey(key) || this.reverse.ContainsKey(value))
            {
                throw new ArgumentException();
            }
            this.reverse.Add(value, key);
            this.normal.Add(key, value);
        }

        public void Clear()
        {
            this.normal.Clear();
            this.reverse.Clear();
        }

        public bool ContainsKey(TKey key) => this.normal.ContainsKey(key);
        public bool ContainsValue(TValue value) => this.reverse.ContainsKey(value);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => this.normal.GetEnumerator();
        public bool Remove(TKey key)
        {
            if (this.ContainsKey(key))
            {
                this.reverse.Remove(this.normal[key]);
                this.normal.Remove(key);
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value) => this.normal.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => this.normal.GetEnumerator();
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => this.Add(item.Key, item.Value);
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)this.normal).Contains(item);
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)this.normal).CopyTo(array, arrayIndex);
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)this.normal).Remove(item);

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(this.normal), this.normal);
            info.AddValue(nameof(this.reverse), this.reverse);
        }
        protected BiDictionary(SerializationInfo info, StreamingContext context)
        {
            this.normal = (Dictionary<TKey, TValue>)info.GetValue(nameof(this.normal), typeof(Dictionary<TKey, TValue>));
            this.reverse = (Dictionary<TValue, TKey>)info.GetValue(nameof(this.reverse), typeof(Dictionary<TValue, TKey>));

            if (this.normal.Count == this.reverse.Count)
            {
                foreach (var item in this.normal)
                {
                    if (!this.reverse.ContainsKey(item.Value))
                        goto ILLEGAL_STATE;
                }
                return;
            }

            ILLEGAL_STATE:
            throw new SerializationException();
        }
    }
}
