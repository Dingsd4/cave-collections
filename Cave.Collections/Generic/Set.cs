#region CopyRight 2018
/*
    Copyright (c) 2003-2018 Andreas Rohleder (andreas@rohleder.cc)
    All rights reserved
*/
#endregion
#region License LGPL-3
/*
    This program/library/sourcecode is free software; you can redistribute it
    and/or modify it under the terms of the GNU Lesser General Public License
    version 3 as published by the Free Software Foundation subsequent called
    the License.

    You may not use this program/library/sourcecode except in compliance
    with the License. The License is included in the LICENSE file
    found at the installation directory or the distribution package.

    Permission is hereby granted, free of charge, to any person obtaining
    a copy of this software and associated documentation files (the
    "Software"), to deal in the Software without restriction, including
    without limitation the rights to use, copy, modify, merge, publish,
    distribute, sublicense, and/or sell copies of the Software, and to
    permit persons to whom the Software is furnished to do so, subject to
    the following conditions:

    The above copyright notice and this permission notice shall be included
    in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion
#region Authors & Contributors
/*
   Author:
     Andreas Rohleder <andreas@rohleder.cc>

   Contributors:

 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Cave.Collections.Generic
{
    /// <summary>
    /// Provides a generic typed set of objects
    /// </summary>
    [DebuggerDisplay("Count={Count}")]
#if NET20
    public sealed class Set<T> : IItemSet<T>
#else
    public sealed class Set<T> : IItemSet<T>
#endif
    {
        #region operators

        /// <summary>
        /// Obtains a <see cref="Set{T}"/> containing all objects part of one of the specified sets
        /// </summary>
        /// <param name="set1">The first set used to calculate the result</param>
        /// <param name="set2">The second set used to calculate the result</param>
        /// <returns>Returns a new <see cref="Set{T}"/> containing the result.</returns>
        public static Set<T> operator |(Set<T> set1, Set<T> set2)
        {
            return BitwiseOr(set1, set2);
        }

        /// <summary>
        /// Obtains a <see cref="Set{T}"/> containing only objects part of both of the specified sets
        /// </summary>
        /// <param name="set1">The first set used to calculate the result</param>
        /// <param name="set2">The second set used to calculate the result</param>
        /// <returns>Returns a new <see cref="Set{T}"/> containing the result.</returns>
        public static Set<T> operator &(Set<T> set1, Set<T> set2)
        {
            return BitwiseAnd(set1, set2);
        }

        /// <summary>
        /// Obtains a <see cref="Set{T}"/> containing all objects part of the first set after removing all objects present at the second set.
        /// </summary>
        /// <param name="set1">The first set used to calculate the result</param>
        /// <param name="set2">The second set used to calculate the result</param>
        /// <returns>Returns a new <see cref="Set{T}"/> containing the result.</returns>
        public static Set<T> operator -(Set<T> set1, Set<T> set2)
        {
            return Subtract(set1, set2);
        }

        /// <summary>
        /// Builds a new <see cref="Set{T}"/> containing only the items found exclusivly in one of both specified sets.
        /// </summary>
        /// <param name="set1">The first set used to calculate the result</param>
        /// <param name="set2">The second set used to calculate the result</param>
        /// <returns>Returns a new <see cref="Set{T}"/> containing the result.</returns>
        public static Set<T> operator ^(Set<T> set1, Set<T> set2)
        {
            return Xor(set1, set2);
        }

        /// <summary>
        /// Checks two sets for equality
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        public static bool operator ==(Set<T> set1, Set<T> set2)
        {
            if (ReferenceEquals(set1, null)) return (ReferenceEquals(set2, null));
            if (ReferenceEquals(set2, null)) return false;
            return set1.Equals(set2);
        }

        /// <summary>
        /// Checks two sets for inequality
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        public static bool operator !=(Set<T> set1, Set<T> set2)
        {
            return !(set1 == set2);
        }
        #endregion

        #region static Member

        /// <summary>
        /// Builds the union of two specified <see cref="Set{T}"/>s
        /// </summary>
        /// <param name="set1">The first set used to calculate the result</param>
        /// <param name="set2">The second set used to calculate the result</param>
        /// <returns>Returns a new <see cref="Set{T}"/> containing the result.</returns>
        public static Set<T> BitwiseOr(IEnumerable<T> set1, IEnumerable<T> set2)
        {
            if (set1 == null) throw new ArgumentNullException("set1");
            if (set2 == null) throw new ArgumentNullException("set2");
            Set<T> result = new Set<T>();
            result.IncludeRange(set1);
            result.IncludeRange(set2);
            return result;
        }

        /// <summary>
        /// Builds the intersection of two specified <see cref="Set{T}"/>s
        /// </summary>
        /// <param name="set1">The first set used to calculate the result</param>
        /// <param name="set2">The second set used to calculate the result</param>
        /// <returns>Returns a new <see cref="Set{T}"/> containing the result.</returns>
        public static Set<T> BitwiseAnd(IEnumerable<T> set1, IEnumerable<T> set2)
        {
            if (set1 == null) throw new ArgumentNullException("set1");
            if (set2 == null) throw new ArgumentNullException("set2");
#if NET20
            var s2 = new Set<T>(set2); 
            var result = new Set<T>();
            foreach (var item in set1)
            {
                if (s2.Contains(item)) result.Add(item);
            }
            return result;
#else
            Set<T> result = new Set<T>();
            result.m_List.UnionWith(set1);
            result.m_List.IntersectWith(set2);
            return result;
#endif
        }

        /// <summary>
        /// Subtracts the specified <see cref="Set{T}"/> from this one and returns a new <see cref="Set{T}"/> containing the result
        /// </summary>
        /// <param name="set1">The first set used to calculate the result</param>
        /// <param name="set2">The second set used to calculate the result</param>
        /// <returns>Returns a new <see cref="Set{T}"/> containing the result.</returns>
        public static Set<T> Subtract(IEnumerable<T> set1, IEnumerable<T> set2)
        {
            if (set1 == null) throw new ArgumentNullException("set1");
            if (set2 == null) throw new ArgumentNullException("set2");
#if NET20
            var s2 = new Set<T>(set2); 
            Set<T> result = new Set<T>();
            foreach (T item in set1)
            {
                if (!s2.Contains(item))
                {
                    result.Add(item);
                }
            }
            return result;
#else
            Set<T> result = new Set<T>();
            result.m_List.UnionWith(set1);
            result.m_List.ExceptWith(set2);
            return result;
#endif
        }

        /// <summary>
        /// Builds a new <see cref="Set{T}"/> containing only items found exclusivly in one of both specified sets.
        /// </summary>
        /// <param name="set1">The first set used to calculate the result</param>
        /// <param name="set2">The second set used to calculate the result</param>
        /// <returns>Returns a new <see cref="Set{T}"/> containing the result.</returns>
        public static Set<T> Xor(IEnumerable<T> set1, IEnumerable<T> set2)
        {
            if (set1 == null) throw new ArgumentNullException("set1");
            if (set2 == null) throw new ArgumentNullException("set2");
#if NET20
            Dictionary<T, int> counter = new Dictionary<T, int>();
            foreach(var item in set1) 
            {
                int count;
                counter.TryGetValue(item, out count);
                counter[item] = ++count;
            }
            Set<T> result = new Set<T>();
            foreach(var item in counter)
            {
                if (item.Value == 0) result.Add(item.Key);
            }
            return result;
#else
            Set<T> result = new Set<T>();
            result.m_List.UnionWith(set1);
            result.m_List.SymmetricExceptWith(set2);
            return result;
#endif
        }

        #endregion

        #region private Member

#if NET20
        Dictionary<T, byte> m_List = new Dictionary<T, byte>();
#else 
        HashSet<T> m_List = new HashSet<T>();
#endif

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new empty set
        /// </summary>
        public Set() { }

        /// <summary>
        /// Creates a new set with the specified items, items specified may contain elements multiple times.
        /// </summary>
        public Set(params T[] items)
            : this()
        {
            IncludeRange(items);
        }

        /// <summary>
        /// Creates a new set with the specified items, items specified may contain elements multiple times.
        /// </summary>
        public Set(IEnumerable<T> items)
            : this()
        {
            IncludeRange(items);
        }

        /// <summary>
        /// Creates a new set with the specified items, items specified may contain elements multiple times.
        /// </summary>
        public Set(params IEnumerable<T>[] blocks)
            : this()
        {
            foreach (var items in blocks) IncludeRange(items);
        }

        /// <summary>
        /// Creates a new set with the specified items, items specified may contain elements multiple times.
        /// </summary>
        public Set(T item, params IEnumerable<T>[] blocks)
            : this()
        {
            Include(item);
            foreach (var items in blocks) IncludeRange(items);
        }

        #endregion

        #region public Member

        /// <summary>Checks whether all specified items are part of the set</summary>
        /// <param name="items"></param>
        /// <returns>Returns true if all items are present.</returns>
        public bool ContainsRange(IEnumerable<T> items)
        {
#if NET20
            if (items == null) throw new ArgumentNullException("items");
            bool allFound = true;
            foreach (T obj in items)
            {
                allFound &= Contains(obj);
            }
            return allFound;
#else
            return m_List.IsSubsetOf(items);
#endif
        }

        /// <summary>
        /// Returns true if the set is empty
        /// </summary>
        public bool IsEmpty
        {
            get { return m_List.Count == 0; }
        }

        /// <summary>Checks whether a specified item is part of the set</summary>
        /// <param name="item">The item to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
#if NET20
            return m_List.ContainsKey(item);
#else
            return m_List.Contains(item);
#endif
        }

        /// <summary>Adds a specified item to the set</summary>
        /// <param name="item">The item to be added to the set</param>
        /// <exception cref="ArgumentNullException">item</exception>
        /// <exception cref="ArgumentException">Item already present!</exception>
        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException("item");
#if NET20
            m_List.Add(item, 0);
#else
            if (!m_List.Add(item)) throw new ArgumentException("Item already present!");
#endif
        }

        /// <summary>Adds a range of items to the set</summary>
        /// <param name="items">The objects to be added to the list</param>
        public void AddRange(IEnumerable<T> items)
        {
#if NET20
            if (items == null) throw new ArgumentNullException("items");
            foreach (T obj in items) { Add(obj); }
#else
            foreach (T obj in items) { Add(obj); }
#endif
        }

        /// <summary>
        /// Adds a range of items to the set
        /// </summary>
        /// <param name="items">The items to be added to the list</param>
        public void AddRange(params T[] items) { AddRange((IEnumerable<T>)items); }

        /// <summary>Includes an item that is not already present in the set (others are ignored).</summary>
        /// <param name="item">The item to be included</param>
        /// <returns>Returns true if the item was added, false if it was present already.</returns>
        public bool Include(T item)
        {
#if NET20
            bool addNew = !m_List.ContainsKey(item);
            if (addNew) m_List[item] = 0;
            return addNew;
#else
            return m_List.Add(item);
#endif
        }

        /// <summary>Includes items that are not already present in the set (others are ignored).</summary>
        /// <param name="items">The items to be included</param>
        /// <returns>Returns the number of items added.</returns>
        public int IncludeRange(IEnumerable<T> items)
        {
#if NET20
            if (items == null) throw new ArgumentNullException("items");
			int count = 0;
            foreach (T item in items) 
            { 
                if (Include(item)) count++;
            }
            return count;
#else
            int oldCount = m_List.Count;
            m_List.UnionWith(items);
            return m_List.Count - oldCount;
#endif
        }

        /// <summary>Adds a range of items to the set</summary>
        /// <param name="items">The items to be added to the list</param>
        /// <returns></returns>
        public int IncludeRange(params T[] items)
        {
            return IncludeRange((IEnumerable<T>)items);
        }

        /// <summary>Tries the remove the specified value.</summary>
        /// <param name="value">The value to remove.</param>
        /// <returns></returns>
        public bool TryRemove(T value)
        {
#if NET20
            if (Contains(value))
            {
                Remove(value);
                return true;
            }
            return false;
#else
            return m_List.Remove(value);
#endif
        }

        /// <summary>Removes objects from the set</summary>
        /// <param name="items"></param>
        /// <returns>Returns the numer of items removed.</returns>
        public int TryRemoveRange(IEnumerable<T> items)
        {
            int count = 0;
            foreach (T item in items) if (TryRemove(item)) count++;
            return count;
        }

        /// <summary>Removes an object from the set</summary>
        /// <param name="item">The item to be removed</param>
        /// <exception cref="KeyNotFoundException">Key not found!</exception>
        public void Remove(T item)
        {
            if (!m_List.Remove(item)) throw new KeyNotFoundException();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        bool ICollection<T>.Remove(T item)
        {
            return TryRemove(item);
        }

        /// <summary>Removes objects from the set</summary>
        /// <param name="items"></param>
        /// <exception cref="ArgumentNullException">items</exception>
        /// <exception cref="KeyNotFoundException">Key not found!</exception>
        public void RemoveRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            foreach (T obj in items) { Remove(obj); }
        }

        /// <summary>
        /// Clears the set
        /// </summary>
        public void Clear()
        {
            m_List.Clear();
#if !NET20
            m_List.TrimExcess();
#endif
        }


        #endregion

        #region ICollection Member

#if NET20
        /// <summary>
        /// Copies all items present at the set to the specified array, starting at a specified index
        /// </summary>
        /// <param name="array">one-dimensional array to copy to</param>
        /// <param name="arrayIndex">the zero-based index in array at which copying begins</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            m_List.Keys.CopyTo(array, arrayIndex);
        }
#else
        /// <summary>
        /// Copies all items present at the set to the specified array, starting at a specified index
        /// </summary>
        /// <param name="array">one-dimensional array to copy to</param>
        /// <param name="index">the zero-based index in array at which copying begins</param>
        public void CopyTo(T[] array, int index)
        {
            m_List.CopyTo(array, index);
        }
#endif

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException">array</exception>
        public void CopyTo(Array array, int index)
        {
            if (array == null) throw new ArgumentNullException("array");
            foreach (object item in this)
            {
                array.SetValue(item, index++);
            }
        }

        /// <summary>
        /// Obtains the number of items present at the set
        /// </summary>
        public int Count
        {
            get { return m_List.Count; }
        }

        /// <summary>
        /// Returns false
        /// </summary>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Returns this
        /// </summary>
        public object SyncRoot
        {
            get { return this; }
        }
        #endregion

        #region IEnumerable Member

        /// <summary>
        /// Obtains an <see cref="IEnumerator"/> for this set
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
#if NET20
            return m_List.Keys.GetEnumerator();
#else
            return m_List.GetEnumerator();
#endif
        }

        #endregion

        #region ICloneable Member

        /// <summary>
        /// Creates a copy of this set
        /// </summary>
        public object Clone()
        {
#if NET20
            return new Set<T>(m_List.Keys);
#else
            return new Set<T>(m_List);
#endif
        }

        #endregion

        #region ICollection<T> Member

        /// <summary>
        /// Returns false
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region IEnumerable<T> Member

        /// <summary>
        /// Obtains an <see cref="IEnumerator"/> for this set
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
#if NET20
            return m_List.Keys.GetEnumerator();
#else
            return m_List.GetEnumerator();
#endif
        }
        #endregion

        /// <summary>
        /// Obtains an array of all elements present
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            T[] result = new T[Count];
            CopyTo(result, 0);
            return result;
        }

        /// <summary>
        /// Checks another Set{T} instance for equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Set<T> other = obj as Set<T>;
            if (null == other) return false;
            return Equals(other);
        }

        /// <summary>
        /// Checks another Set{T} instance for equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Set<T> other)
        {
#if NET20
            if (other == null) return false;
            if (other.Count != Count) return false;
            return ContainsRange(other);
#else
            return m_List.SetEquals(other);
#endif
        }

        /// <summary>
        /// Obtains the hash code of the base list.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return m_List.GetHashCode();
        }
    }

    /// <summary>
    /// Provides a typed 2D set. (Set A may only contain each value once. List B may contain any value multiple times. If typeof(A) == typeof(B)
    /// a value may be present once at set A and multiple times at set B. Each value in set a is linked to a value in list b via its index)
    /// This class is very similar to Dictionary{A, B}, in fact it uses one. Additionally to the fast Name to value lookup it provides indexing like a list.
    /// </summary>
    
    [DebuggerDisplay("Count={Count}")]
    public sealed class Set<TKey, TValue> : IItemSet<TKey, TValue>
    {
        Dictionary<TKey, ItemPair<TKey, TValue>> m_LookupA = new Dictionary<TKey, ItemPair<TKey, TValue>>();
        List<ItemPair<TKey, TValue>> m_List = new List<ItemPair<TKey, TValue>>();

        /// <summary>
        /// Adds an item pair to the end of the List.
        /// This is an O(1) operation.
        /// </summary>
        /// <param name="A">The A object to be added</param>
        /// <param name="B">The B object to be added</param>
        public void Add(TKey A, TValue B)
        {
            ItemPair<TKey, TValue> node = new ItemPair<TKey, TValue>(A, B);
            m_LookupA.Add(A, node);
            m_List.Add(node);
        }

        /// <summary>
        /// Clears the set.
        /// </summary>
        public void Clear()
        {
            m_List.Clear();
            m_LookupA.Clear();
        }

        /// <summary>
        /// Obtains the index of the specified A object.
        /// This is an O(n) operation.
        /// </summary>
        /// <param name="A">'A' object to be found.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOfA(TKey A)
        {
            if (!m_LookupA.ContainsKey(A)) return -1;
            return m_List.IndexOf(m_LookupA[A]);
        }

        /// <summary>
        /// Not supported. Use UniqueSet instead.
        /// </summary>
        /// <param name="B"></param>
        /// <exception cref="NotSupportedException">Thrown if the set does not support indexing</exception>
        /// <returns></returns>
        [Obsolete("Not supported. Use UniqueSet instead.")]
        public int IndexOfB(TValue B)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Obtains a read only collection for the A elements of the Set.
        /// This method is an O(1) operation;
        /// </summary>
        public IList<TKey> ItemsA
        {
            get
            {
                return new ReadOnlyListA<TKey, TValue>(this);
            }
        }

        /// <summary>
        /// Obtains a read only collection for the B elements of the Set.
        /// This method is an O(1) operation;
        /// </summary>
        public IList<TValue> ItemsB
        {
            get
            {
                return new ReadOnlyListB<TKey, TValue>(this);
            }
        }

        /// <summary>
        /// Gets the number of elements actually present at the Set.
        /// </summary>
        public int Count
        {
            get { return m_List.Count; }
        }

        /// <summary>
        /// Returns false
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the set.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the set.</returns>
        public IEnumerator<ItemPair<TKey, TValue>> GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the set.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the set.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        /// <summary>
        /// Obtains the index of the specified ItemPair.
        /// This is an O(1) operation.
        /// </summary>
        /// <param name="item">The ItemPair to search for.</param>
        /// <returns>The index of the ItemPair if found in the list; otherwise, -1.</returns>
        public int IndexOf(ItemPair<TKey, TValue> item)
        {
            return m_List.IndexOf(item);
        }

        /// <summary>
        /// Obtains the index of the specified ItemPair.
        /// This is an O(1) operation.
        /// </summary>
        /// <param name="A">The A value of the ItemPair to search for.</param>
        /// <param name="B">The B value of the ItemPair to search for.</param>
        /// <returns>The index of the ItemPair if found in the list; otherwise, -1.</returns>
        public int IndexOf(TKey A, TValue B)
        {
            return IndexOf(new ItemPair<TKey, TValue>(A, B));
        }

        /// <summary>
        /// Inserts a new ItemPair at the specified index.
        /// This method needs a full index rebuild and is an O(n) operation, where n is Count.
        /// </summary>
        /// <param name="index">The index to insert the item at.</param>
        /// <param name="A">The A value of the ItemPair to insert.</param>
        /// <param name="B">The B value of the ItemPair to insert.</param>
        public void Insert(int index, TKey A, TValue B)
        {
            Insert(index, new ItemPair<TKey, TValue>(A, B));
        }

        /// <summary>
        /// Inserts a new ItemPair at the specified index.
        /// This method needs a full index rebuild and is an O(n) operation, where n is Count.
        /// </summary>
        /// <param name="index">The index to insert the item at.</param>
        /// <param name="item">The ItemPair to insert.</param>
        public void Insert(int index, ItemPair<TKey, TValue> item)
        {
            if (item == null) throw new ArgumentNullException("item");
            try
            {
                m_LookupA.Add(item.A, item);
                m_List.Insert(index, item);
            }
            catch
            {
                Clear();
                throw;
            }
        }

        /// <summary>
        /// Removes the ItemPair at the specified index.
        /// This method needs a full index rebuild and is an O(n) operation, where n is Count.
        /// </summary>
        /// <param name="index">The index to remove the item.</param>
        public void RemoveAt(int index)
        {
            try
            {
                ItemPair<TKey, TValue> item = m_List[index];
                m_List.RemoveAt(index);
                if (!m_LookupA.Remove(item.A)) throw new KeyNotFoundException();
            }
            catch
            {
                Clear();
                throw;
            }
        }

        /// <summary>
        /// Accesses the ItemPair at the specified index. The getter is a O(1) operation.
        /// The setter needs a full index rebuild and is an O(n) operation, where n is Count.
        /// </summary>
        /// <param name="index">The index of the ItemPair to be accessed.</param>
        /// <returns></returns>
        public ItemPair<TKey, TValue> this[int index]
        {
            get
            {
                return m_List[index];
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                ItemPair<TKey, TValue> old = m_List[index];
                if (!m_LookupA.Remove(old.A)) throw new KeyNotFoundException();
                try { m_LookupA.Add(value.A, value); }
                catch { m_LookupA.Add(old.A, old); throw; }
                m_List[index] = value;
            }
        }

        /// <summary>
        /// Adds an itempair to the set
        /// </summary>
        /// <param name="item"></param>
        public void Add(ItemPair<TKey, TValue> item)
        {
            if (item == null) throw new ArgumentNullException("item");
            Add(item.A, item.B);
        }

        /// <summary>
        /// Checks whether an itempair is part of the set or not
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ItemPair<TKey, TValue> item)
        {
            if (item == null) throw new ArgumentNullException("item");
            return m_LookupA.ContainsKey(item.A) && Equals(m_LookupA[item.A].B , item.B);
        }

        /// <summary>
        /// Checks whether an itempair is part of the set or not
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public bool Contains(TKey A, TValue B)
        {
            return Contains(new ItemPair<TKey, TValue>(A, B));
        }

        /// <summary>Checks whether a specified A key is present</summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public bool ContainsA(TKey A)
        {
            return m_LookupA.ContainsKey(A);
        }

        /// <summary>Not supported. Use UniqueSet.</summary>
        /// <param name="B"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        [Obsolete("Not Supported. Use UniqueSet!")]
        public bool ContainsB(TValue B)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not Supported
        /// </summary>
        /// <param name="B">The B index.</param>
        /// <returns></returns>
        [Obsolete("Not supported. Use UniqueSet instead!")]
        public ItemPair<TKey, TValue> GetB(TValue B)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Obtains the A element that is assigned to the specified B element.
        /// This method is an O(1) operation;
        /// </summary>
        /// <param name="A">The A index.</param>
        /// <returns></returns>
        public ItemPair<TKey, TValue> GetA(TKey A)
        {
            return m_LookupA[A];
        }

        /// <summary>
        /// Copies all item of the set to the specified array starting at the specified index
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ItemPair<TKey, TValue>[] array, int arrayIndex)
        {
            m_List.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes an itempair from the set
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(ItemPair<TKey, TValue> item)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (!m_LookupA.Remove(item.A)) throw new KeyNotFoundException();
            return m_List.Remove(item);
        }

        /// <summary>
        /// Removes an itempair from the set
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public bool Remove(TKey A, TValue B)
        {
            return Remove(new ItemPair<TKey, TValue>(A, B));
        }

        /// <summary>Removes the item with the specified A key</summary>
        /// <param name="A">The A key</param>
        /// <exception cref="KeyNotFoundException">The exception that is thrown when the key specified for accessing an element in a collection does not match any key in the collection.</exception>
        public void RemoveA(TKey A)
        {
            Remove(GetA(A));
        }

        /// <summary>Not supported</summary>
        /// <param name="B">The A key</param>
        /// <exception cref="KeyNotFoundException">The exception that is thrown when the key specified for accessing an element in a collection does not match any key in the collection.</exception>
        [Obsolete("Not supported. Use UniqueSet instead!")]
        public void RemoveB(TValue B)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reverses the index of the set
        /// </summary>
        public void Reverse()
        {
            m_List.Reverse();
        }
    }
}

