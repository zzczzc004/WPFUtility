using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace WpfUtility.Common
{
    /// <summary>
    /// 可用来当combobox需要空白项表示未选择时使用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SyncCollectionWithExtraItem<T> : ObservableCollection<T>
    {
        public SyncCollectionWithExtraItem(ObservableCollection<T> objectCollection, T extraItem)
        {
            if (extraItem == null)
            {
                throw new ArgumentNullException(nameof(extraItem));
            }

            Add(extraItem);

            this.AddRange(objectCollection);

            objectCollection.CollectionChanged += OnSyncCollection;
        }

        private void OnSyncCollection(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var index = e.NewStartingIndex;
                    foreach (var item in e.NewItems)
                    {
                        InsertItem(index++, (T)item);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        Remove((T)item);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
