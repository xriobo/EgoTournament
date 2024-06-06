namespace EgoTournament.Models
{
    public class TournamentDto
    {
        public TournamentDto(string name, List<string> rules, List<string> summonerNames, bool hasReward)
        {
            Uid = Guid.NewGuid();
            Name = name;
            Rules = rules;
            SummonerNames = summonerNames;
            HasReward = hasReward;
        }

        public Guid Uid { get; set; }

        public string Name { get; set; }

        public List<string> Rules { get; set; }

        public List<string> SummonerNames { get; set; }

        public bool HasReward { get; set; }
    }
}
