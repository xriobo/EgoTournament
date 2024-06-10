using Newtonsoft.Json;

namespace EgoTournament.Models.Riot
{
    public class ParticipantDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("summonerName")]
        public string SummonerName { get; set; }

        [JsonProperty("tagLine")]
        public string TagLine { get; set; }

        [JsonProperty("puuid")]
        public string Puuid { get; set; }

        [JsonProperty("rewardCandidate")]
        public bool RewardCandidate { get; set; }
    }
}
