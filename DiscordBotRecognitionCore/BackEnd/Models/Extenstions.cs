using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DiscordBotRecognitionCore.BackEnd.Models
{
    public static class Extensions
    {
        public static List<T> MapListJSON<T>(string data)
        {
            return JsonSerializer.Deserialize<List<T>>(data);
        }
    }
}
