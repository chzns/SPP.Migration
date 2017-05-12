using System;
using System.Collections.Generic;
using System.Linq;

namespace SPP.Migration
{
    #region ------------------------- Add by Rock 2015/11/28 去除集合重复项-------------------
    /// <summary>
    /// 對照 Helper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ComparisonHelper<T>
    {
        /// <summary>
        /// 創建對照表
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="keySelector">Key Selector</param>
        /// <returns></returns>
        public static IEqualityComparer<T> CreateComparer<V>(Func<T, V> keySelector)
        {
            return new CommonEqualityComparer<V>(keySelector);
        }

        /// <summary>
        /// 創建對照表
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="keySelector">Key Selector</param>
        /// <param name="comparer">Comparer</param>
        /// <returns></returns>
        public static IEqualityComparer<T> CreateComparer<V>(Func<T, V> keySelector, IEqualityComparer<V> comparer)
        {
            return new CommonEqualityComparer<V>(keySelector, comparer);
        }

        /// <summary>
        /// 相等對照 Class
        /// </summary>
        /// <typeparam name="V"></typeparam>
        class CommonEqualityComparer<V> : IEqualityComparer<T>
        {
            /// keySelector
            private Func<T, V> keySelector;
            /// <summary>
            /// 表示要為索引鍵的類型使用預設
            /// </summary>
            private IEqualityComparer<V> comparer;
            /// <summary>
            /// 相同對比 Function
            /// </summary>
            /// <param name="keySelector">keySelector</param>
            /// <param name="comparer">comparer</param>
            public CommonEqualityComparer(Func<T, V> keySelector, IEqualityComparer<V> comparer)
            {
                this.keySelector = keySelector;
                this.comparer = comparer;
            }
            /// <summary>
            /// 相同對比 Function
            /// </summary>
            /// <param name="keySelector">keySelector</param>
            public CommonEqualityComparer(Func<T, V> keySelector)
                : this(keySelector, EqualityComparer<V>.Default)
            { }

            /// <summary>
            /// 相等 Function
            /// </summary>
            /// <param name="x">X值</param>
            /// <param name="y">Y值</param>
            /// <returns></returns>
            public bool Equals(T x, T y)
            {
                return comparer.Equals(keySelector(x), keySelector(y));
            }
            /// <summary>
            /// GetHashCode
            /// </summary>
            /// <param name="obj">物件</param>
            /// <returns></returns>
            public int GetHashCode(T obj)
            {
                return comparer.GetHashCode(keySelector(obj));
            }
        }
    }
    #endregion------------
    /// <summary>
    /// CommonEqualityComparer Class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class CommonEqualityComparer<T, V> : IEqualityComparer<T>
    {
        /// keySelector
        private Func<T, V> keySelector;

        /// <summary>
        /// CommonEqualityComparer
        /// </summary>
        /// <param name="keySelector">keySelector</param>
        public CommonEqualityComparer(Func<T, V> keySelector)
        {
            this.keySelector = keySelector;
        }

        /// <summary>
        /// 相等Function
        /// </summary>
        /// <param name="x">X值</param>
        /// <param name="y">Y值</param>
        /// <returns></returns>
        public bool Equals(T x, T y)
        {
            return EqualityComparer<V>.Default.Equals(keySelector(x), keySelector(y));
        }

        /// <summary>
        /// Get HashCode
        /// </summary>
        /// <param name="obj">物件</param>
        /// <returns></returns>
        public int GetHashCode(T obj)
        {
            return EqualityComparer<V>.Default.GetHashCode(keySelector(obj));
        }
    }

    /// <summary>
    /// 獲取Distinct值
    /// </summary>
    public static class DistinctExtensions
    {
        /// <summary>
        /// Distinct Function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="source">Source</param>
        /// <param name="keySelector">keySelector</param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector)
        {
            return source.Distinct(new CommonEqualityComparer<T, V>(keySelector));
        }
    }
}
