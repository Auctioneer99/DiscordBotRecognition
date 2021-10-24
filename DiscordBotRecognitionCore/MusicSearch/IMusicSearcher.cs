using System.Threading.Tasks;
using DiscordBotRecognition.Songs;

namespace DiscordBotRecognition.MusicSearch
{
    public interface IMusicSearcher
    {
        Task<ISong> SearchSong(string query);
    }
}
