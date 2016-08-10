using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WpfUtility.Common
{
  /// <summary>
  /// Class that provides extension methods to Collection
  /// 
  /// </summary>
  public static class CollectionExtensions
  {
    /// <summary>
    /// Add a range of items to a collection.
    /// 
    /// </summary>
    /// <typeparam name="T">Type of objects within the collection.</typeparam><param name="collection">The collection to add items to.</param><param name="items">The items to add to the collection.</param>
    /// <returns>
    /// The collection.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">An <see cref="T:System.ArgumentNullException"/> is thrown if <paramref name="collection"/> or <paramref name="items"/> is <see langword="null"/>.</exception>
    public static Collection<T> AddRange<T>(this Collection<T> collection, IEnumerable<T> items)
    {
      if (collection == null)
        throw new ArgumentNullException("collection");
      if (items == null)
        throw new ArgumentNullException("items");
      foreach (T obj in items)
        collection.Add(obj);
      return collection;
    }
  }
}
