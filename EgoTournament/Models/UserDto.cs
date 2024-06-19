using EgoTournament.Models.Enums;
using Newtonsoft.Json;

namespace EgoTournament.Models
{
    public class UserDto
    {
        public UserDto() 
        {
            Uid = Guid.NewGuid().ToString();
        }

        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("summonerName")]
        public string SummonerName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("role")]
        public RoleType Role { get; set; }

        [JsonProperty("tournamentUids")]
        public List<Guid> TournamentUids { get; set; }

        [JsonProperty("tournaments")]
        public List<TournamentDto> Tournaments { get; set; }
    }
}
