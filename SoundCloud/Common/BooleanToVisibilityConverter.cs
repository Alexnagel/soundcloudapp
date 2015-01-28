using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SoundCloud.Common
{
    class BooleanToVisibilityConverter : IValueConverter
    {
        public bool VisibleOnTrue { get; set; }
        public static readonly DependencyProperty VisibleOnTrueProperty = DependencyProperty.Register("VisibleOnTrue",
            typeof(double), typeof(bool), new PropertyMetadata(new Thickness(0)));

        public object Convert(object value, System.Type targetType, object parameter, string language)
        {
            bool isVisible = (bool) value;
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, string language)
        {
            return (Visibility) value == Visibility.Visible;
        }
    }
}
