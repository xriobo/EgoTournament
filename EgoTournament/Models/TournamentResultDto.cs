using Newtonsoft.Json;

namespace EgoTournament.Models
{
    public class TournamentResultDto
    {
        [JsonProperty("ownerId")]
        public string OwnerId { get; set; }

        [JsonProperty("uid")]
        public Guid Uid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rules")]
        public List<string> Rules { get; set; }

        [JsonProperty("participantsResults")]
        public List<ParticipantResultDto> ParticipantsResults { get; set; }

        [JsonProperty("hadReward")]
        public bool HadReward { get; set; }

        [JsonProperty("finishDate")]
        public DateTime FinishDate { get; set; }
    }
}
