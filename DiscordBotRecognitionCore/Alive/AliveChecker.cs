using System;
using System.Threading;

namespace DiscordBotRecognition.Alive
{
    public enum EAliveState
    {
        NotStarted,
        Alive,
        Dead
    }

    public class AliveChecker : IDisposable
    {
        public const long IDLE_TIME_MILLISECONDS = 15 * 60 * 1000;

        public event Action<EAliveState> StateChanged;

        public EAliveState State
        {
            get => _state;
            private set
            {
                _state = value;
                StateChanged?.Invoke(value);
            }
        }
        private EAliveState _state;

        private Timer _timer;
        private long _lastUpdate;
        private bool _disposed;

        public AliveChecker()
        {
        }

        public void Start()
        {
            TimerCallback callback = new TimerCallback(Callback);
            _timer = new Timer(callback, null, 0, IDLE_TIME_MILLISECONDS);
            State = EAliveState.Alive;
            Update();
        }

        public void Update()
        {
            _lastUpdate = DateTime.Now.Ticks;
        }

        private void Callback(object? target)
        {
            var now = DateTime.Now.Ticks;
            if (_lastUpdate + IDLE_TIME_MILLISECONDS <= now)
            {
                State = EAliveState.Dead;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing)
                {

                }
                _timer.Dispose();
                _disposed = true;
            }
        }

        ~AliveChecker()
        {
            Dispose(false);
        }
    }
}
