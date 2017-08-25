﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackBarLabs.Extensions // Make user force extensions because this affects _every_ object
{
    public static class ObjectExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T onlyItem)
        {
            yield return onlyItem;
        }

        public static T OrIfDefault<T>(this T value, T alternative)
            where T : IComparable
        {
            if (value.IsDefault())
                return alternative;
            return value;
        }

        public static T OrIfDefaultOrNull<T>(this T value, T alternative)
            where T : class
        {
            if (value.IsDefaultOrNull())
                return alternative;
            return value;
        }

        public static bool IsDefault<T>(this T value)
            where T : IComparable
        {
            return EqualityComparer<T>.Default.Equals(value, default(T));
        }

        public static bool IsDefaultOrNull<T>(this T value)
           where T : class
        {
            return EqualityComparer<T>.Default.Equals(value, default(T));
        }

        public static TResult HasValue<T, TResult>(this T value, Func<T, TResult> hasValue, Func<TResult> nullOrEmptyValue)
           where T : class
        {
            if (value.IsDefaultOrNull())
                return nullOrEmptyValue();
            return hasValue(value);
        }

        public static bool IsDefaultOrEmpty(this Guid value)
        {
            return value.IsDefault() || value == Guid.Empty;
        }

        public static TResult HasValue<T, TResult>(this Nullable<T> value, Func<T, TResult> hasValue, Func<TResult> nullOrEmptyValue)
            where T : struct
        {
            if (!value.HasValue)
                return nullOrEmptyValue();
            return hasValue(value.Value);
        }

        public static bool Equals<T>(this Nullable<T> value1, Nullable<T> value2)
           where T : struct
        {
            if(value1.HasValue)
            {
                if (value2.HasValue)
                    return ValueType.Equals(value1.Value, value2.Value);
                return false;
            }
            return !value2.HasValue;
        }

        public static TResult HasValue<TResult>(this Guid value, Func<Guid, TResult> hasValue, Func<TResult> nullOrEmptyValue)
        {
            if (value.IsDefaultOrEmpty())
                return nullOrEmptyValue();
            return hasValue(value);
        }

        public static TResult HasValue<TResult>(this DateTime value, Func<DateTime, TResult> hasValue, Func<TResult> nullOrEmptyValue)
        {
            if (value.IsDefault())
                return nullOrEmptyValue();
            return hasValue(value);
        }

        public static T? ToOptional<T>(this T value)
            where T : struct
        {
            return (T?)value;
        }

        public static bool IsDefault(this Uri value)
        {
            return value.IsDefaultOrNull();
        }
        
        public static Task<T> ToTask<T>(this T value)
        {
            return Task.FromResult(value);
        }

        public static KeyValuePair<TKey, TValue> PairWithKey<TKey, TValue>(this TValue value, TKey key)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }

        public static KeyValuePair<TKey, TValue> PairWithValue<TKey, TValue>(this TKey key, TValue value)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }

        public static RecursiveTuple<TKey> RecurseWithValue<TKey>(this TKey key, RecursiveTuple<TKey> value)
        {
            return new RecursiveTuple<TKey>()
            {
                item1 = key,
                next = () => value,
            };
        }
    }
}
