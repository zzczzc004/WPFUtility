using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfUtility.Behaviors
{
    public class FrameworkElementFocusBehavior
    {
        private static readonly DependencyProperty FocusOnLoadProperty =
        DependencyProperty.RegisterAttached("FocusOnLoad",
                                            typeof(bool),
                                            typeof(FrameworkElementFocusBehavior),
                                            new UIPropertyMetadata(FocusOnLoadPropertyChanged));

        public static void FocusOnLoadPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)source;
            element.Loaded -= FrameworElementFocusHelperLoadedEvent;
            if ((bool)e.NewValue == true)
                element.Loaded += FrameworElementFocusHelperLoadedEvent;
        }

        public static void SetFocusOnLoad(DependencyObject element, bool value)
        {
            element.SetValue(FocusOnLoadProperty, value);
        }
        public static bool GetFocusOnLoad(DependencyObject element)
        {
            return (bool)element.GetValue(FocusOnLoadProperty);
        }

        public static void FrameworElementFocusHelperLoadedEvent(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

            var textbox = sender as TextBox;
            textbox?.SelectAll();
        }
    }
}
