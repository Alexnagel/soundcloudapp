using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using SoundCloud.Interfaces;

namespace SoundCloud.Model
{
    public class StreamLoadingCollection : ObservableCollection<CollectionItem>, ISupportIncrementalLoading
    {
        private bool _hasMoreItems;
        private IDataManager _dataManager;

        public bool HasMoreItems
        {
            get { return _hasMoreItems; }
        }

        public StreamLoadingCollection(IDataManager dataManager)
        {
            _dataManager = dataManager;
            _hasMoreItems = true;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            var dispatcher = Window.Current.Dispatcher;
            return Task.Run<LoadMoreItemsResult>(
                async () =>
                {
                    uint resultcount = 0;

                    int lastId = -1;
                    if (Count > 0)
                        lastId = this[Count - 1].Pid;

                    var result = await _dataManager.GetNextPageStream(lastId);

                    if (result == null || result.Count == 0)
                        _hasMoreItems = false;
                    else
                    {
                        resultcount = (uint) result.Count;

                        await Task.WhenAll(Task.Delay(10), dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            foreach (var streamItem in result)
                            {
                                if (!this.Contains(streamItem) && (streamItem.Type != "playlist" || streamItem.Type != "playlist-repost"))
                                    this.Add(streamItem);
                            }
                        }).AsTask());
                    }
                    return new LoadMoreItemsResult() {Count = resultcount};
                }
                ).AsAsyncOperation<LoadMoreItemsResult>();
        }
    }
}
