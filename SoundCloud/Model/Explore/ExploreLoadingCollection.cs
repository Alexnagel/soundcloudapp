using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using SoundCloud.Interfaces;

namespace SoundCloud.Model.Explore
{
    class ExploreLoadingCollection : ObservableCollection<Track>, ISupportIncrementalLoading
    {
        private IDataManager _dataManager;
        private bool _hasMoreItems;

        public bool HasMoreItems
        {
            get { return _hasMoreItems; }
        }

        public ExploreLoadingCollection(IDataManager dataManager)
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
                    uint resultCount = 0;
                    var result = await _dataManager.GetNextCatgoryTracks();

                    if (result == null || result.Count == 0)
                        _hasMoreItems = false;
                    else
                    {
                        resultCount = (uint)result.Count;
                        await Task.WhenAll(Task.Delay(10), dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            for (int i = 0; i < result.Count; i++)
                            {
                                foreach (var exploreItem in result)
                                {
                                    if (!this.Contains(exploreItem))
                                        this.Add(exploreItem);
                                }
                            }
                        }).AsTask());
                    }
                    return new LoadMoreItemsResult() { Count = resultCount };
                }
                ).AsAsyncOperation<LoadMoreItemsResult>();
        }
    }
}
