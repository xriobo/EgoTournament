using EgoTournament.Models.Riot;

namespace EgoTournament.Models.Views
{
    public class SummonerView
    {
        public string Name { get; set; }

        public List<RankDto> Ranks { get; set; }

        public RankDto RankSoloQ { get; set; }
    }
}
