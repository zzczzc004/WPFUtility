using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfUtility.Converters
{
    /// <summary>
    /// 用来在DataGrid前加上编号列
    /// 使用方式：
    /// <DataGridTextColumn Header="编号" IsReadOnly="True">
    ///     <DataGridTextColumn.Binding>
    ///         <MultiBinding Converter="{StaticResource RowNumberConverter}">
    ///             <Binding />
    ///             <Binding
    ///             RelativeSource="{RelativeSource FindAncestor, AncestorType={x:Type DataGrid}}" />
    ///         </MultiBinding>
    ///     </DataGridTextColumn.Binding>
    /// </DataGridTextColumn>
    /// </summary>
    public class RowNumberConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //get the grid and the item
            var item = values[0];
            var grid = values[1] as DataGrid;

            var index = grid.Items.IndexOf(item) + 1;

            return index.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
