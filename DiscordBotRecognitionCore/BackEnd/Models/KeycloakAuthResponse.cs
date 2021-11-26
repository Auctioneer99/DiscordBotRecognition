using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DiscordBotRecognitionCore.BackEnd.Models
{
    public class KeycloakAuthResponse
    {
        public string AuthorizationHeader => $"{TokenType} {AccessToken}";

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonPropertyName("refresh_expires_in")]
        public long RefreshExpiersIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }
}
