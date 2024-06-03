namespace EgoTournament.Models
{
    public class TournamentDto
    {
        public Guid Uid { get; set; }

        public List<string> Rules { get; set; }

        public List<UserDto> Users { get; set; }

    }
}
