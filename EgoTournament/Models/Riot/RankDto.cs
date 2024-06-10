using EgoTournament.Models.Enums;
using Newtonsoft.Json;

namespace EgoTournament.Models.Riot
{
    public class RankDto
    {
        [JsonProperty("leagueId")]
        public string LeagueId { get; set; }

        [JsonProperty("queueType")]
        public string QueueType { get; set; }

        [JsonProperty("tier")]
        public string Tier { get; set; }

        [JsonProperty("rank")]
        public string Rank { get; set; }

        [JsonProperty("summonerId")]
        public string SummonerId { get; set; }

        [JsonProperty("summonerName")]
        public string SummonerName { get; set; }

        [JsonProperty("leaguePoints")]
        public int LeaguePoints { get; set; }

        [JsonProperty("wins")]
        public int Wins { get; set; }

        [JsonProperty("losses")]
        public int Losses { get; set; }

        [JsonProperty("veteran")]
        public bool Veteran { get; set; }

        [JsonProperty("inactive")]
        public bool Inactive { get; set; }

        [JsonProperty("freshBlood")]
        public bool FreshBlood { get; set; }

        [JsonProperty("hotStreak")]
        public bool HotStreak { get; set; }

        [JsonIgnore]
        public TierEnum TierType { get; set; }

        [JsonIgnore]
        public QueueType QueueEnum { get; set; }

        [JsonIgnore]
        public RomanNumberEnum Division { get; set; }

        [JsonIgnore]
        public string Winrate { get; set; }

        [JsonIgnore]
        public decimal WinrateNum { get; set; }

        [JsonIgnore]
        public decimal AverageKills { get; set; }

        [JsonIgnore]
        public decimal AverageAssists { get; set; }

        [JsonIgnore]
        public decimal AverageDeaths { get; set; }

        [JsonIgnore]
        public string AverageKdaString { get; set; }

        public decimal AverageKda { get; set; }

        public bool AnyPentakill { get; set; }
    }
}
