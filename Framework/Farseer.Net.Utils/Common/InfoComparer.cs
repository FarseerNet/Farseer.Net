﻿using System;
using System.Collections.Generic;

namespace FS.Utils.Common
{
    /// <summary>
    ///     对象比较的实现
    /// </summary>
    /// <typeparam name="TEntity"> </typeparam>
    /// <typeparam name="T"> </typeparam>
    public class InfoComparer<TEntity, T> : IEqualityComparer<TEntity> where TEntity : class
    {
        private readonly Func<TEntity, T> _keySelect;

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="keySelect"> </param>
        public InfoComparer(Func<TEntity, T> keySelect)
        {
            _keySelect = keySelect;
        }

        /// <summary>
        ///     比较
        /// </summary>
        /// <param name="x"> </param>
        /// <param name="y"> </param>
        /// <returns> </returns>
        public bool Equals(TEntity x, TEntity y) => EqualityComparer<T>.Default.Equals(x: _keySelect(arg: x), y: _keySelect(arg: y));

        /// <summary>
        ///     HashCode
        /// </summary>
        /// <param name="obj"> </param>
        /// <returns> </returns>
        public int GetHashCode(TEntity obj) => EqualityComparer<T>.Default.GetHashCode(obj: _keySelect(arg: obj));
    }
}