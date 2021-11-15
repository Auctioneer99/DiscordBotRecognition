using DiscordBotRecognition.AudioPlayer;
using DiscordBotRecognitionCore.Connection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DiscordBotRecognitionCore.Alive
{
    public class AliveChecker
    {
        public const long IDLE_TIME_MILLISECONDS = 15 * 60 * 1000;

        private ConnectionPool _connectionPool;
        private List<AudioGroup> _groupsToDelete;
        private Timer _timer;

        public AliveChecker(ConnectionPool connectionPool)
        {
            _connectionPool = connectionPool;
            _groupsToDelete = new List<AudioGroup>();
        }

        public void Start()
        {
            _connectionPool.Get += OnGroupGet;
            TimerCallback callback = new TimerCallback(Callback);
            _timer = new Timer(callback, null, 0, IDLE_TIME_MILLISECONDS);
        }

        private void OnGroupGet(AudioGroup group)
        {
            _groupsToDelete.Remove(group);
        }

        private void Callback(object target)
        {
            foreach(var group in _groupsToDelete)
            {
                _connectionPool.Leave(group.Id);
            }
            var groupsToDelete = new List<AudioGroup>();
            foreach(var pair in _connectionPool.AudioGroups)
            {
                if (pair.Value.IsPlaying == false)
                {
                    groupsToDelete.Add(pair.Value);
                }
            }
            _groupsToDelete = groupsToDelete;
        }

        public void Stop()
        {
            _connectionPool.Get -= OnGroupGet;
            _timer.Dispose();
        }
    }
}
