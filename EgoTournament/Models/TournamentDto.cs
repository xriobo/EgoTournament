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

        public string OwnerId { get; set; }

        public Guid Uid { get; set; }

        public string Name { get; set; }

        public List<string> Rules { get; set; }

        public List<string> SummonerNames { get; set; }

        public bool HasReward { get; set; }
    }
}
