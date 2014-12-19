using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;

namespace BackgroundAudioTask
{
    public sealed class PlayQueueDB
    {
        private const string DB_NAME = "PlayQueue.db";
        private SQLiteConnection _dbConnection;

        public PlayQueueDB()
        {
            _dbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DB_NAME);
            InitDb();
        }

        #region Private DB Init
        private async void InitDb()
        {
            // Create Db if not exist
            bool dbExist = await CheckDbAsync();
            if (!dbExist)
            {
                CreateDatabaseAsync();
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

        private void CreateDatabaseAsync()
        {
            _dbConnection.CreateTable<BaseTrack>();
        }
        #endregion Private DB Init

        #region Public 

        public void SetQueue(IEnumerable<BaseTrack> playlist)
        {
            var queue = _dbConnection.Table<BaseTrack>().ToList();
            foreach (var track in queue)
            {
                _dbConnection.Delete(track);
            }

            _dbConnection.InsertAll(playlist);
        }

        public void AddToQueue(IEnumerable<BaseTrack> playlistAddition)
        {
            _dbConnection.InsertAll(playlistAddition);
        }

        public void AddOneToQueue(BaseTrack track)
        {
            _dbConnection.Insert(track);
        }

        public BaseTrack GetFirst()
        {
            return _dbConnection.Table<BaseTrack>().First();
        }

        public BaseTrack GetTrack(string id)
        {
            var query = _dbConnection.Table<BaseTrack>().Where(t => t.Id == id);

            var list = query.ToList();

            if (list.Count == 1)
                return list[0];
            else
                return null;
        }

        public BaseTrack GetNext(int dbId)
        {
            return _dbConnection.Find<BaseTrack>(++dbId);
        }

        public BaseTrack GetPrevious(int dbId)
        {
            return _dbConnection.Find<BaseTrack>(--dbId);
        }

        #endregion Public
    }
}
