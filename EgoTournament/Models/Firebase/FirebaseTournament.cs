namespace EgoTournament.Models.Firebase
{
    public class FirebaseTournament
    {
        public bool HasReward { get; set; }
        public string Name { get; set; }
        public List<string> Rules { get; set; }
        public List<string> SummonerNames { get; set; }
        public string Uid { get; set; }
    }
}
