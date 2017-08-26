﻿/*
 * This file is part of the CatLib package.
 *
 * (c) Yu Bin <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: http://catlib.io/
 */

using System;
using System.Collections.Generic;

namespace CatLib
{
    /// <summary>
    /// 数组
    /// </summary>
    public static class Arr
    {
        /// <summary>
        /// 合并数组
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="sources">需要合并的数组</param>
        /// <returns>合并后的数组</returns>
        public static T[] Merge<T>(params T[][] sources)
        {
            var length = 0;
            foreach (var source in sources)
            {
                length += source.Length;
            }

            var merge = new T[length];
            var current = 0;
            foreach (var source in sources)
            {
                Array.Copy(source, 0, merge, current, source.Length);
                current += source.Length;
            }

            return merge;
        }

        /// <summary>
        /// 从数组中获取一个或者指定数量的随机值
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">源数据</param>
        /// <param name="number">随机的数量</param>
        /// <returns>随机后的元素</returns>
        public static T[] Random<T>(T[] source, int number = 1)
        {
            number = Math.Max(number, 1);
            source = Shuffle(source);
            var requested = new T[number];
            var i = 0;
            foreach (var result in source)
            {
                if (i >= number)
                {
                    break;
                }
                requested[i++] = result;
            }

            return requested;
        }

        /// <summary>
        /// 将数组中的元素打乱
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="sources">源数据</param>
        /// <param name="seed">种子</param>
        /// <returns>打乱后的数据</returns>
        public static T[] Shuffle<T>(T[] sources, int? seed = null)
        {
            var requested = new List<T>(sources);

            var random = new Random(seed.GetValueOrDefault(Guid.NewGuid().GetHashCode()));
            for (var i = 0; i < requested.Count; i++)
            {
                var index = random.Next(0, requested.Count - 1);
                if (index == i)
                {
                    continue;
                }
                var temp = requested[i];
                requested[i] = requested[index];
                requested[index] = temp;
            }

            return requested.ToArray();
        }

        /// <summary>
        /// 从数组中移除选定的元素，并用新元素从<paramref name="start"/>位置开始插入
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">源数组</param>
        /// <param name="start">删除元素的开始位置</param>
        /// <param name="length">移除的元素个数，也是被返回数组的长度</param>
        /// <param name="replSource">在start位置插入的数组</param>
        /// <returns>被删除的数组</returns>
        public static T[] Splice<T>(ref T[] source, int start, int? length = null, T[] replSource = null)
        {
            Guard.Requires<ArgumentNullException>(source != null);

            start = (start >= 0) ? Math.Min(start, source.Length) : Math.Max(source.Length + start, 0);

            length = (length == null)
                ? Math.Max(source.Length - start, 0)
                : (length >= 0)
                    ? Math.Min(length.Value, source.Length - start)
                    : Math.Max(source.Length + length.Value - start, 0);

            var requested = new T[length.Value];

            if (length.Value == source.Length)
            {
                // 现在移除所有旧的元素，然后用新的元素替换。
                Array.Copy(source, requested, source.Length);
                source = replSource ?? new T[] { };
                return requested;
            }

            Array.Copy(source, start, requested, 0, length.Value);

            if (replSource == null || replSource.Length == 0)
            {
                var newSource = new T[source.Length - length.Value];
                // 现在只删除不插入
                if (start > 0)
                {
                    Array.Copy(source, 0, newSource, 0, start);
                }
                Array.Copy(source, start + length.Value, newSource, start, source.Length - (start + length.Value));
                source = newSource;
            }
            else
            {
                var newSource = new T[source.Length - length.Value + replSource.Length];
                // 删除并且插入
                if (start > 0)
                {
                    Array.Copy(source, 0, newSource, 0, start);
                }
                Array.Copy(replSource, 0, newSource, start, replSource.Length);
                Array.Copy(source, start + length.Value, newSource, start + replSource.Length,
                    source.Length - (start + length.Value));
                source = newSource;
            }

            return requested;
        }

        /// <summary>
        /// 将数组分为新的数组块
        /// <para>其中每个数组的单元数目由 size 参数决定。最后一个数组的单元数目可能会少几个。</para>
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">源数据</param>
        /// <param name="size">每个分块的大小</param>
        /// <returns></returns>
        public static T[][] Chunk<T>(T[] source, int size)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            size = Math.Max(1, size);
            var requested = new T[source.Length / size + (source.Length % size == 0 ? 0 : 1)][];

            T[] chunk = null;
            for (var i = 0; i < source.Length; i++)
            {
                var pos = i / size;
                if (i % size == 0)
                {
                    if (chunk != null)
                    {
                        requested[pos - 1] = chunk;
                    }
                    chunk = new T[(i + size) <= source.Length ? size : source.Length - i];
                }
                chunk[i - (pos * size)] = source[i];
            }
            requested[requested.Length - 1] = chunk;

            return requested;
        }

        /// <summary>
        /// 填充数组
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="start">起始下标</param>
        /// <param name="length">填充长度</param>
        /// <param name="value">填充的值</param>
        /// <param name="source">以这个数组作为源</param>
        /// <returns>填充后的数组</returns>
        public static T[] Fill<T>(int start, int length, T value, T[] source = null)
        {
            Guard.Requires<ArgumentOutOfRangeException>(start >= 0);
            Guard.Requires<ArgumentOutOfRangeException>(length > 0);
            var count = start + length;
            var requested = new T[source == null ? count : source.Length + count];

            if (start > 0 && source != null)
            {
                Array.Copy(source, requested, Math.Min(source.Length, start));
            }

            for (var i = start; i < count; i++)
            {
                requested[i] = value;
            }

            if (source != null && start < source.Length)
            {
                Array.Copy(source, start, requested, count, source.Length - start);
            }

            return requested;
        }

        /// <summary>
        /// 输入数组中的每个值传给回调函数,如果回调函数返回 true，则把输入数组中的当前值加入结果数组中
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">数组源</param>
        /// <param name="predicate">回调函数</param>
        /// <returns></returns>
        public static T[] Filter<T>(T[] source, Predicate<T> predicate)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<ArgumentNullException>(predicate != null);
            var elements = new T[source.Length];

            var i = 0;
            foreach (var result in source)
            {
                if (predicate.Invoke(result))
                {
                    elements[i++] = result;
                }
            }

            Array.Resize(ref elements, i);
            return elements;
        }

        /// <summary>
        /// 将数组值传入用户自定义函数，自定义函数返回的值作为新的数组值
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">数组源</param>
        /// <param name="callback">自定义函数</param>
        /// <returns>处理后的数组</returns>
        public static T[] Map<T>(T[] source, Func<T, T> callback)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<ArgumentNullException>(callback != null);

            for (var i = 0; i < source.Length; i++)
            {
                source[i] = callback.Invoke(source[i]);
            }

            return source;
        }

        /// <summary>
        /// 删除数组中的最后一个元素
        /// </summary>
        /// <typeparam name="T">删除数组中的最后一个元素</typeparam>
        /// <param name="source">数组源</param>
        /// <returns>被删除的元素</returns>
        public static T Pop<T>(ref T[] source)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<InvalidOperationException>(source.Length > 0);

            T result = source[source.Length - 1];
            Array.Resize(ref source, source.Length - 1);
            return result;
        }

        /// <summary>
        /// 将一个或多个元素加入数组尾端
        /// </summary>
        /// <typeparam name="T">数组诶型</typeparam>
        /// <param name="source">数组源</param>
        /// <param name="elements">要加入的元素</param>
        public static void Push<T>(ref T[] source, params T[] elements)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<InvalidOperationException>(elements != null);

            Array.Resize(ref source, source.Length + elements.Length);
            Array.Copy(elements, 0, source, source.Length - elements.Length, elements.Length);
        }
    }
}