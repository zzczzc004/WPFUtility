using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WpfUtility.Common
{
    /// <summary>
    /// 每项属性变化时，也支持通知
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableCollectionEx<T> : ObservableCollection<T>
        where T : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler ItemPropertyChanged;

        protected override void ClearItems()
        {
            foreach (var item in Items)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
            base.ClearItems();
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            item.PropertyChanged += Item_PropertyChanged;
        }

        protected override void RemoveItem(int index)
        {
            Items[index].PropertyChanged -= Item_PropertyChanged;
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = Items[index];
            var newItem = item;
            oldItem.PropertyChanged -= Item_PropertyChanged;
            newItem.PropertyChanged += Item_PropertyChanged;
            base.SetItem(index, item);
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ItemPropertyChanged?.Invoke(sender, e);
        }
    }
}