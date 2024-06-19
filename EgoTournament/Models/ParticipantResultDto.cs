using EgoTournament.Models.Enums;
using Newtonsoft.Json;

namespace EgoTournament.Models
{
    public class ParticipantResultDto
    {
        [JsonProperty("tierType")]
        public TierEnum TierType { get; set; }

        [JsonProperty("rank")]
        public string Rank { get; set; }

        [JsonProperty("leaguePoints")]
        public int LeaguePoints { get; set; }

        [JsonProperty("summonerName")]
        public string SummonerName { get; set; }

        [JsonProperty("wins")]
        public int Wins { get; set; }

        [JsonProperty("losses")]
        public int Losses { get; set; }

        [JsonProperty("winrate")]
        public string Winrate { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }
    }
}
