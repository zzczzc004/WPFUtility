using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfUtility.Controls
{
    public class CanvasAutoSize : Canvas
    {
        protected override Size MeasureOverride(Size constraint)
        {
            var tmp = base.MeasureOverride(constraint);

            if (InternalChildren.Count == 0)
            {
                return tmp;
            }

            var width = InternalChildren
                .OfType<UIElement>()
                .Max(i => i.DesiredSize.Width + (double) i.GetValue(LeftProperty));

            var height = InternalChildren
                .OfType<UIElement>()
                .Max(i => i.DesiredSize.Height + (double) i.GetValue(TopProperty));

            return new Size(width, height);
        }
    }
}