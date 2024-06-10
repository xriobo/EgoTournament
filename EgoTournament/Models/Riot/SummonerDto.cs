using Newtonsoft.Json;

namespace EgoTournament.Models.Riot
{
    public class SummonerDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("accountId")]
        public string AccountId { get; set; }

        [JsonProperty("puuid")]
        public string Puuid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("profileIconId")]
        public int ProfileIconId { get; set; }

        [JsonProperty("revisionDate")]
        public long RevisionDate { get; set; }

        [JsonProperty("summonerLevel")]
        public int SummonerLevel { get; set; }

        [JsonIgnore]
        public List<RankDto> Ranks { get; set; }

        [JsonIgnore]
        public RankDto RankSoloQ { get; set; }
    }
}
