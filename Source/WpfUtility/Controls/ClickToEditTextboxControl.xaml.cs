using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfUtility.Controls
{
    /// <summary>
    /// 默认显示成Textblock，当点击后，进入编辑Textbox状态
    /// </summary>
    public partial class ClickToEditTextboxControl : UserControl
    {
        public ClickToEditTextboxControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// TODO Esc时恢复设置未实现
        /// </summary>
        private string _oldText;

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ClickToEditTextboxControl), new UIPropertyMetadata());

        private void textBoxName_LostFocus(object sender, RoutedEventArgs e)
        {
            textBlockName.Visibility = Visibility.Visible;
            textBoxName.Visibility = Visibility.Collapsed;
        }

        private void textBlockName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (textBoxName.Visibility == Visibility.Collapsed)
            {
                _oldText = Text;
            }

            textBoxName.Visibility = Visibility.Visible;
            textBlockName.Visibility = Visibility.Collapsed;
        }

        private void textBoxName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                textBoxName_LostFocus(textBoxName, null);
            }
            else if (e.Key == Key.Escape)
            {
                Text = _oldText;
                e.Handled = true;
                textBoxName_LostFocus(textBoxName, null);
            }
        }
    }
}
