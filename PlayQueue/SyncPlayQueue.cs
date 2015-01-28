using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using SQLite.Net;
using SQLite.Net.Interop;
using SQLite.Net.Platform.WinRT;

namespace BackgroundAudio.PlayQueue
{
    public sealed class SyncPlayQueue
    {
        private const string DB_NAME = "PlayQueue.db";
        private readonly object _padlock = new object();

        private static SyncPlayQueue _instance;
        public static SyncPlayQueue PlayQueueInstance
        {
            get
            {
                if (_instance == null)
                    _instance = new SyncPlayQueue();
                return _instance;
            }
        }

        private SyncPlayQueue()
        {
            SQLite3.Config(ConfigOption.Serialized);
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
            using (var dbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DB_NAME))
            {
                dbConnection.CreateTable<BaseTrack>();   
            }
        }
        #endregion Private DB Init

        #region Public 

        public bool EmptyQueue()
        {
            using (var dbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DB_NAME))
            {
                try
                {
                    return dbConnection.DeleteAll<BaseTrack>() == 1;
                }
                catch (SQLiteException e)
                {
                    // not basetrack table
                    return true;
                }
            }
        }

        public void SetQueue(IEnumerable<BaseTrack> playlist)
        {
            EmptyQueue();
            using (var dbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DB_NAME))
            {
                dbConnection.InsertAll(playlist);
            }
        }

        public void AddToQueue(IEnumerable<BaseTrack> playlistAddition)
        {
            using (var dbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DB_NAME))
            {
                dbConnection.InsertAll(playlistAddition);
            }
        }

        public bool AddOneToQueue(BaseTrack track)
        {
            using (var dbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DB_NAME))
            {
                return dbConnection.Insert(track) == 1;
            }
        }

        public bool SetTrackPlaying(int id, bool isPlaying)
        {
            using (var dbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DB_NAME))
            {
                BaseTrack track = dbConnection.Get<BaseTrack>(id);
                track.IsPlaying = isPlaying;
                return dbConnection.Update(track) == 1;

            }
        }

        public BaseTrack GetCurrentTrack()
        {
            BaseTrack track = null;
            using (var dbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DB_NAME))
            {
                try
                {
                    track = dbConnection.Table<BaseTrack>().Where(t => t.IsPlaying).First();
                }
                catch (InvalidOperationException e)
                {
                    // no tracks in db keep track null
                }
            }
            return track;
        }

        public BaseTrack GetFirst()
        {
            using (var dbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DB_NAME))
            {
                return dbConnection.Table<BaseTrack>().First();
            }
        }

        public BaseTrack GetTrack(string id)
        {
            using (var dbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DB_NAME))
            {
                return dbConnection.Table<BaseTrack>().Where(t => t.Id == id).First();
            }
        }

        public BaseTrack GetNext(int dbId)
        {
            using (var dbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DB_NAME))
            {
                return dbConnection.Get<BaseTrack>(++dbId);
            }
        }

        public BaseTrack GetPrevious(int dbId)
        {
            using (var dbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DB_NAME))
            {
                return dbConnection.Get<BaseTrack>(--dbId);
            }
        }

        #endregion Public
    }
}
