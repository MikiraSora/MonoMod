﻿using System.Collections.Generic;
using Mono.Collections.Generic;
using System.Collections;

namespace MonoMod.Utils {
    public static partial class Extensions {

        /// <summary>
        /// See <see cref="List{T}.AddRange(IEnumerable{T})"/>
        /// </summary>
        public static void AddRange<T>(this Collection<T> list, IEnumerable<T> other) {
            Helpers.ThrowIfArgumentNull(list);
            foreach (T entry in Helpers.ThrowIfNull(other))
                list.Add(entry);
        }
        /// <summary>
        /// See <see cref="List{T}.AddRange(IEnumerable{T})"/>
        /// </summary>
        public static void AddRange(this IDictionary dict, IDictionary other) {
            Helpers.ThrowIfArgumentNull(dict);
            foreach (DictionaryEntry entry in Helpers.ThrowIfNull(other))
                dict.Add(entry.Key, entry.Value);
        }
        /// <summary>
        /// See <see cref="List{T}.AddRange(IEnumerable{T})"/>
        /// </summary>
        public static void AddRange<K, V>(this IDictionary<K, V> dict, IDictionary<K, V> other) {
            Helpers.ThrowIfArgumentNull(dict);
            foreach (KeyValuePair<K, V> entry in Helpers.ThrowIfNull(other))
                dict.Add(entry.Key, entry.Value);
        }
        /// <summary>
        /// See <see cref="List{T}.AddRange(IEnumerable{T})"/>
        /// </summary>
        public static void AddRange<K, V>(this Dictionary<K, V> dict, Dictionary<K, V> other) where K : notnull {
            Helpers.ThrowIfArgumentNull(dict);
            foreach (KeyValuePair<K, V> entry in Helpers.ThrowIfNull(other))
                dict.Add(entry.Key, entry.Value);
        }

        /// <summary>
        /// See <see cref="List{T}.InsertRange(int, IEnumerable{T})"/>
        /// </summary>
        public static void InsertRange<T>(this Collection<T> list, int index, IEnumerable<T> other) {
            Helpers.ThrowIfArgumentNull(list);
            foreach (T entry in Helpers.ThrowIfNull(other))
                list.Insert(index++, entry);
        }

    }
}
