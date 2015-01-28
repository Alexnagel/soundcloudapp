using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoundCloud.View
{
    public class RoundedButton : Button
    {
        public double PathWidth { get; set; }
        public double PathHeight { get; set; }
        public Thickness PathMargin { get; set; }

        public static readonly DependencyProperty PathWidthProperty = DependencyProperty.Register("PathWidth",
            typeof (double), typeof (RoundedButton), new PropertyMetadata(0));
        public static readonly DependencyProperty PathHeightProperty = DependencyProperty.Register("PathHeight",
            typeof(double), typeof(RoundedButton), new PropertyMetadata(0));
        public static readonly DependencyProperty PathMarginProperty = DependencyProperty.Register("PathMargin",
            typeof(double), typeof(RoundedButton), new PropertyMetadata(new Thickness(0)));

    }
}
