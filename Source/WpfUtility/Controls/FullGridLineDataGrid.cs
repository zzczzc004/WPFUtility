using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfUtility.Controls
{
    public static class Visual_ExtensionMethods
    {
        public static T FindDescendant<T>(this Visual @this, Predicate<T> predicate = null) where T : Visual
        {
            return @this.FindDescendant(v => v is T && (predicate == null || predicate((T)v))) as T;
        }

        public static Visual FindDescendant(this Visual @this, Predicate<Visual> predicate)
        {
            if (@this == null)
                return null;

            var frameworkElement = @this as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.ApplyTemplate();
            }

            Visual child = null;
            for (int i = 0, count = VisualTreeHelper.GetChildrenCount(@this); i < count; i++)
            {
                child = VisualTreeHelper.GetChild(@this, i) as Visual;
                if (predicate(child))
                    return child;

                child = child.FindDescendant(predicate);
                if (child != null)
                    return child;

            }
            return child;
        }
    }

    public class GridAdorner : Adorner
    {
        private FullGridLineDataGrid _dataGrid;
        private List<double> _columnsWidth;
        private double _lastRowBottomOffset;
        private double _remainingSpace;
        private static double TOLERANCE = 1e-6;

        public GridAdorner(FullGridLineDataGrid dataGrid)
            : base(dataGrid)
        {
            _dataGrid = dataGrid;
            dataGrid.LayoutUpdated += new EventHandler(dataGrid_LayoutUpdated);
        }

        void dataGrid_LayoutUpdated(object sender, EventArgs e)
        {
            if (_columnsWidth == null)
            {
                _columnsWidth = new List<double>(_dataGrid.Columns.Count);

                int i = 0;
                foreach (var dataGridColumn in _dataGrid.Columns)
                {
                    _columnsWidth.Add(dataGridColumn.ActualWidth);
                    ++i;
                }
            }
            else
            {
                bool isChanged = false;
                int i = 0;
                foreach (var dataGridColumn in _dataGrid.Columns)
                {
                    if (Math.Abs(_columnsWidth[i] - dataGridColumn.Width.DesiredValue) > TOLERANCE)
                    {
                        _columnsWidth[i] = dataGridColumn.Width.DesiredValue;
                        isChanged = true;
                    }
                    
                    ++i;

                }

                if (Math.Abs(_lastRowBottomOffset - _dataGrid.LastRowBottomOffset) > TOLERANCE)
                {
                    _lastRowBottomOffset = _dataGrid.LastRowBottomOffset;
                    isChanged = true;
                }

                if (Math.Abs(_remainingSpace - (_dataGrid.RenderSize.Height - _lastRowBottomOffset)) > TOLERANCE)
                {
                    _remainingSpace = (_dataGrid.RenderSize.Height - _lastRowBottomOffset);
                    isChanged = true;
                }
                
                if (isChanged)
                {
                    InvalidateVisual();
                }
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var myDataGrid = AdornedElement as FullGridLineDataGrid;
            if (myDataGrid == null)
                throw new InvalidOperationException();

            // Draw Horizontal lines
//             var lastRowBottomOffset = myDataGrid.LastRowBottomOffset;
//             var remainingSpace = myDataGrid.RenderSize.Height - lastRowBottomOffset;
            var lastRowBottomOffset = _lastRowBottomOffset;
            var remainingSpace = _remainingSpace;
            var placeHolderRowHeight = myDataGrid.PlaceHolderRowHeight;
            var lineNumber = (int)(Math.Floor(remainingSpace / placeHolderRowHeight));

            for (int i = 1; i <= lineNumber; i++)
            {
                Rect rectangle = new Rect(new Size(myDataGrid.RenderSize.Width, 1)) { Y = lastRowBottomOffset + (i * placeHolderRowHeight) };
                drawingContext.DrawRectangle(myDataGrid.HorizontalGridLinesBrush, null, rectangle);
            }

            // Draw vertical lines
            var reorderedColumns = myDataGrid.Columns.OrderBy(c => c.DisplayIndex);
            double verticalLineOffset = -myDataGrid.ScrollViewer.HorizontalOffset;
            foreach (var column in reorderedColumns)
            {
                if (column.Visibility == Visibility.Visible)
                {
                    verticalLineOffset += column.ActualWidth;
                }

                Rect rectangle = new Rect(new Size(1, Math.Max(0, remainingSpace))) { X = verticalLineOffset, Y = lastRowBottomOffset };
                drawingContext.DrawRectangle(myDataGrid.VerticalGridLinesBrush, null, rectangle);
            }
        }
    }

    /// <summary>
    /// DataGrid空白区域画网格
    /// </summary>
    public class FullGridLineDataGrid : DataGrid
    {
        private Action _unloadAction;
        private GridAdorner _gridAdorner;
        public FullGridLineDataGrid()
        {
            Loaded += new RoutedEventHandler(MyDataGrid_Loaded);
            Unloaded += OnUnloaded;
            PlaceHolderRowHeight = 25.0D; // random value, can be changed
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var dataGrid = sender as FullGridLineDataGrid;
            if (dataGrid == null)
                throw new InvalidOperationException();

            _unloadAction?.Invoke();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (ReferenceEquals(e.Source, this))
            {
                CommitEdit();
            }
        }
        
        private void MyDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var dataGrid = sender as FullGridLineDataGrid;
            if (dataGrid == null)
                throw new InvalidOperationException();

            // Add the adorner that will be responsible for drawing grid lines
            var adornerLayer = AdornerLayer.GetAdornerLayer(dataGrid);
            if (adornerLayer != null)
            {
                _unloadAction = new Action(() =>
                {
                    adornerLayer.Remove(_gridAdorner);
                });
                _gridAdorner = new GridAdorner(dataGrid);
                adornerLayer.Add(_gridAdorner);
            }

            // Find DataGridRowsPresenter and set alignment to top to easily retrieve last row vertical offset
            dataGrid.DataGridRowsPresenter.VerticalAlignment = System.Windows.VerticalAlignment.Top;
        }

        public double PlaceHolderRowHeight
        {
            get;
            set;
        }

        public double LastRowBottomOffset
        {
            get
            {
                return DataGridColumnHeadersPresenter.RenderSize.Height + DataGridRowsPresenter.RenderSize.Height;
            }
        }

        public DataGridColumnHeadersPresenter DataGridColumnHeadersPresenter
        {
            get
            {
                if (dataGridColumnHeadersPresenter == null)
                {
                    dataGridColumnHeadersPresenter = this.FindDescendant<DataGridColumnHeadersPresenter>();
                    if (dataGridColumnHeadersPresenter == null)
                        throw new InvalidOperationException();
                }
                return dataGridColumnHeadersPresenter;
            }
        }

        public DataGridRowsPresenter DataGridRowsPresenter
        {
            get
            {
                if (dataGridRowsPresenter == null)
                {
                    dataGridRowsPresenter = this.FindDescendant<DataGridRowsPresenter>();
                    if (dataGridRowsPresenter == null)
                        throw new InvalidOperationException();
                }
                return dataGridRowsPresenter;
            }
        }

        public ScrollViewer ScrollViewer
        {
            get
            {
                if (scrollViewer == null)
                {
                    scrollViewer = this.FindDescendant<ScrollViewer>();
                    if (scrollViewer == null)
                        throw new InvalidOperationException();
                }
                return scrollViewer;
            }
        }

        private DataGridRowsPresenter dataGridRowsPresenter;
        private DataGridColumnHeadersPresenter dataGridColumnHeadersPresenter;
        private ScrollViewer scrollViewer;
    }
}
