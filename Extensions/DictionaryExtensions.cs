using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace stoogebag_MonuMental.stoogebag.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue TryGetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, Func<TValue> addNew)
        {
            if (dic.TryGetValue(key, out var val)) return val;
            else
            {
                var newVal = addNew();
                dic[key] = newVal;
                return newVal;
            }
        }

        public static TValue TryGetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key)
        {
            if (dic.TryGetValue(key, out var val)) return val;
            else return default;
        }

        public static TValue TryGetOrClosest<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, Func<TKey, TKey, float> distanceFunc)
        {
            if (dic.TryGetValue(key, out var val)) return val;
            if (distanceFunc == null) return default;
            return dic.MinItem(t => distanceFunc(t.Key, key)).Value;
        }



    }

    public abstract class UnitySerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector]
        private List<TKey> keyData = new List<TKey>();
	
        [SerializeField, HideInInspector]
        private List<TValue> valueData = new List<TValue>();

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.Clear();
            for (int i = 0; i < this.keyData.Count && i < this.valueData.Count; i++)
            {
                this[this.keyData[i]] = this.valueData[i];
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            this.keyData.Clear();
            this.valueData.Clear();

            foreach (var item in this)
            {
                this.keyData.Add(item.Key);
                this.valueData.Add(item.Value);
            }
        }
    }
}
