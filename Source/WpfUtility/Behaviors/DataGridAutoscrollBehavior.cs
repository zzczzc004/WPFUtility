using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfUtility.Behaviors
{
    /// <summary>
    /// 自动scroll到选中的行
    /// </summary>
    public static class DataGridAutoscrollBehavior
    {
        public static readonly DependencyProperty AutoscrollProperty = DependencyProperty.RegisterAttached(
            "Autoscroll", typeof(bool), typeof(DataGridAutoscrollBehavior), new PropertyMetadata(default(bool), AutoscrollChangedCallback));

        private static readonly Dictionary<DataGrid, SelectionChangedEventHandler> handlersDict = new Dictionary<DataGrid, SelectionChangedEventHandler>();

        private static void AutoscrollChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var dataGrid = dependencyObject as DataGrid;
            if (dataGrid == null)
            {
                throw new InvalidOperationException("Dependency object is not DataGrid.");
            }

            if ((bool)args.NewValue)
            {
                Subscribe(dataGrid);
                dataGrid.Unloaded += DataGridOnUnloaded;
                dataGrid.Loaded += DataGridOnLoaded;
            }
            else
            {
                Unsubscribe(dataGrid);
                dataGrid.Unloaded -= DataGridOnUnloaded;
                dataGrid.Loaded -= DataGridOnLoaded;
            }
        }

        private static void Subscribe(DataGrid dataGrid)
        {
            if (handlersDict.ContainsKey(dataGrid))
            {
                return;
            }

            var handler = new SelectionChangedEventHandler((sender, eventArgs) => ScrollToSelect(dataGrid));
            handlersDict.Add(dataGrid, handler);
            dataGrid.SelectionChanged += handler;
            ScrollToSelect(dataGrid);
        }

        private static void Unsubscribe(DataGrid dataGrid)
        {
            SelectionChangedEventHandler handler;
            handlersDict.TryGetValue(dataGrid, out handler);
            if (handler == null)
            {
                return;
            }
            dataGrid.SelectionChanged -= handler;
            handlersDict.Remove(dataGrid);
        }

        private static void DataGridOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var dataGrid = (DataGrid)sender;
            if (GetAutoscroll(dataGrid))
            {
                Subscribe(dataGrid);
            }
        }

        private static void DataGridOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var dataGrid = (DataGrid)sender;
            if (GetAutoscroll(dataGrid))
            {
                Unsubscribe(dataGrid);
            }
        }

        private static void ScrollToSelect(DataGrid datagrid)
        {
            if (datagrid.Items.Count == 0)
            {
                return;
            }

            if (datagrid.SelectedItem == null)
            {
                return;
            }

            datagrid.ScrollIntoView(datagrid.SelectedItem);
        }

        public static void SetAutoscroll(DependencyObject element, bool value)
        {
            element.SetValue(AutoscrollProperty, value);
        }

        public static bool GetAutoscroll(DependencyObject element)
        {
            return (bool)element.GetValue(AutoscrollProperty);
        }
    }
}
