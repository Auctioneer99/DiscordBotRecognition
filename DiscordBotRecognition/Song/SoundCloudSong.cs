using System;
using System.Threading.Tasks;


namespace DiscordBotRecognition.Songs
{
    /*
    public class SoundCloudSong : ISong
    {
        public string Name => _track.label_name;
        public TimeSpan Duration { get; private set; }
        public DateTime BeginPlay { get; set; }

        private Track _track;

        public SoundCloudSong(Track track)
        {
            _track = track;
            Duration = TimeSpan.FromSeconds(_track.duration ?? 0);
        }

        public Task<SongStream> GetStream()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetStreamUrl()
        {
            return _track.uri;// Uri.AbsoluteUri;
        }
    }*/
}
