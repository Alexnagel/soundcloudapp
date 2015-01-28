using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Interop;
using SQLite.Net.Platform.WinRT;

namespace BackgroundAudio.PlayQueue
{
    public sealed class PlayQueueDB
    {
        private const string DB_NAME = "PlayQueue.db";
        private SQLiteAsyncConnection _dbConnection;
        private readonly object _padlock = new object();

        private static PlayQueueDB _instance;
        public static PlayQueueDB PlayQueueInstance
        {
            get
            {
                if (_instance == null)
                    _instance = new PlayQueueDB();
                return _instance;
            }
        }

        private PlayQueueDB()
        {
            SQLite3.Config(ConfigOption.Serialized);
            var dbWithLock = new Func<SQLiteConnectionWithLock>(
                () =>
                        new SQLiteConnectionWithLock(new SQLitePlatformWinRT(), new SQLiteConnectionString(DB_NAME, true))
                );

            _dbConnection = new SQLiteAsyncConnection(dbWithLock);
            InitDb();
        }

        #region Private DB Init
        private async void InitDb()
        {
            // Create Db if not exist
            bool dbExist = await CheckDbAsync();
            if (!dbExist)
            {
               CreateDatabase();
            }
        }

        private async Task<bool> CheckDbAsync()
        {
            bool dbExist = true;

            try
            {
                StorageFile sf = await ApplicationData.Current.LocalFolder.GetFileAsync(DB_NAME);
            }
            catch (Exception)
            {
                dbExist = false;
            }

            return dbExist;
        }

        private void CreateDatabase()
        {
            lock (_padlock)
            {
                _dbConnection.CreateTableAsync<BaseTrack>();
            }
        }
        #endregion Private DB Init

        #region Public 

        public IAsyncOperation<bool> EmptyQueue()
        {
            return EmptyQueueTask().AsAsyncOperation();
        }

        public async void SetQueue(IEnumerable<BaseTrack> playlist)
        {
            await EmptyQueueTask();
            await _dbConnection.InsertAllAsync(playlist);
        }

        public async void AddToQueue(IEnumerable<BaseTrack> playlistAddition)
        {
            await _dbConnection.InsertAllAsync(playlistAddition);
        }

        public IAsyncOperation<bool> AddOneToQueue(BaseTrack track)
        {
            return addOneToQueueTask(track).AsAsyncOperation();
        }

        public IAsyncOperation<bool> SetTrackPlaying(int id, bool isPlaying)
        {
            return SetTrackPlayingTask(id, isPlaying).AsAsyncOperation();
        }

        public IAsyncOperation<BaseTrack> GetCurrentTrack()
        {
            return getCurrentTrackTask().AsAsyncOperation();
        }

        public IAsyncOperation<BaseTrack> GetFirst()
        {
            return GetFirstTrack().AsAsyncOperation();
        }

        public IAsyncOperation<BaseTrack> GetTrack(string id)
        {
            return GetTrackByUUID(id).AsAsyncOperation();
        }

        public IAsyncOperation<BaseTrack> GetNext(int dbId)
        {
            return GetTrackById(++dbId).AsAsyncOperation();
        }

        public IAsyncOperation<BaseTrack> GetPrevious(int dbId)
        {
            return GetTrackById(--dbId).AsAsyncOperation();
        }

        #endregion Public

        #region Private

        private async Task<bool> EmptyQueueTask()
        {
            int i = -1;

            try
            {
                i = await _dbConnection.DeleteAllAsync<BaseTrack>();
            }
            catch (SQLiteException e)
            {
                // No table to delete
            }

            if (i > 0)
                return true;
            else
                return false;
        }

        private async Task<bool> addOneToQueueTask(BaseTrack track)
        {
            return (await _dbConnection.InsertAsync(track) == 1);
        }

        private async Task<BaseTrack> getCurrentTrackTask()
        {
            return await _dbConnection.Table<BaseTrack>().Where(t => t.IsPlaying == true).FirstAsync();
        }

        private async Task<BaseTrack> GetFirstTrack()
        {
            return await _dbConnection.Table<BaseTrack>().FirstAsync();
        }

        private async Task<bool> SetTrackPlayingTask(int dbId, bool isPlaying)
        {
            BaseTrack track = await _dbConnection.GetAsync<BaseTrack>(dbId);
            track.IsPlaying = isPlaying;
            return (await _dbConnection.UpdateAsync(track) == 1);
        }

        private async Task<BaseTrack> GetTrackByUUID(string id)
        {
            var query = _dbConnection.Table<BaseTrack>().Where(t => t.Id == id);

            var list = await query.ToListAsync();

            if (list.Count == 1)
                return list[0];
            else
                return null;
        }

        private async Task<BaseTrack> GetTrackById(int id)
        {
            return await _dbConnection.FindAsync<BaseTrack>(id);
        }
        #endregion Private
    }
}
