using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SoundCloud.Model;

namespace SoundCloud.View.StreamViews
{
    public class StreamTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TrackItem { get; set; }
        public DataTemplate PlaylistItem { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var streamItem = item as CollectionItem;
            if (streamItem != null)
            {
                return (streamItem.Type == "track" || streamItem.Type == "track-repost") ? TrackItem : PlaylistItem;
            }
            return null;
        }
    }
}
