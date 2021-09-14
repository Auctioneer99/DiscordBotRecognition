using System.Threading.Tasks;
using DiscordBotRecognition.Song;

namespace DiscordBotRecognition.MusicSearch
{
    public interface IMusicSearcher
    {
        Task<ISong> SearchSong(string query);
    }
}
