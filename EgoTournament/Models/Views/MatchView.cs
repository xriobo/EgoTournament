using EgoTournament.Models.Enums;

namespace EgoTournament.Models.Views
{
    public class MatchView
    {
        public bool Win { get; set; }

        public int Kills { get; set; }

        public int Deaths { get; set; }

        public int Assists { get; set; }

        public string Champion { get; set; }

        public string Lane { get; set; }

        public MatchMode MatchMode { get; set; }
    }
}
