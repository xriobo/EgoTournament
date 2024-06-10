using EgoTournament.Models.Riot.RawData;

namespace EgoTournament.Models.Riot
{
    public class SummonerWithMatchesDto
    {
        public SummonerDto SummonerDto { get; set; }

        public List<Match> Matches { get; set; }
    }
}
