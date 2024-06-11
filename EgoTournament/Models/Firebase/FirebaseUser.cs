namespace EgoTournament.Models.Firebase
{
    public class FirebaseUser
    {
        public string Email { get; set; }
        public int Role { get; set; }
        public string SummonerName { get; set; }
        public Dictionary<string, FirebaseTournament> Tournaments { get; set; }
    }
}
