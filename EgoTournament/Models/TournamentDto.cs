using Newtonsoft.Json;

namespace EgoTournament.Models
{
    public class TournamentDto : ObservableObject
    {
        public TournamentDto()
        {
            Uid = Guid.NewGuid();
            Rules = new List<string>();
            SummonerNames = new List<string>();
        }

        [JsonProperty("ownerId")]
        public string OwnerId { get; set; }

        [JsonProperty("uid")]
        public Guid Uid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rules")]
        public List<string> Rules { get; set; }

        [JsonProperty("summonerNames")]
        public List<string> SummonerNames { get; set; }

        [JsonProperty("hasReward")]
        public bool HasReward { get; set; }

        [JsonProperty("finished")]
        public bool Finished { get; set; }

        [JsonProperty("finishDate")]
        public DateTime FinishDate { get; set; }
    }
}
