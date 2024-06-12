using EgoTournament.Models.Riot;
using EgoTournament.Models.Riot.RawData;

namespace EgoTournament.Services
{
    public interface IRiotService
    {
        IEnumerable<SummonerDto> SetParticipantRanks(IEnumerable<SummonerDto> summonersRiot);

        List<Match> GetMatchesInfoByMatchIdList(List<string> matchIds);

        SummonerWithMatchesDto GetSummonerWithMachesBySummonerNameAndTagLine(string summonerName, string tagLine, int countMatches);

        List<Match> GetMatchesInfo(SummonerDto summonerDto, int countMatches, bool rankeds = false);

        /// <summary>
        /// Gets the summoners by puuid.
        /// </summary>
        /// <param name="puuids">The puuids.</param>
        /// <returns>A list of <see cref="SummonerDto"/>.</returns>
        IEnumerable<SummonerDto> GetSummonersByPuuid(IEnumerable<PuuidDto> puuids);

        /// <summary>
        /// Gets the name of the puuid by participants name and tag.
        /// </summary>
        /// <param name="participants">The participants.</param>
        /// <returns>A tuple list of <see cref="PuuidDto"/> and list of string.</returns>
        Tuple<IEnumerable<PuuidDto>, List<string>> GetPuuidByParticipantsNameAndTagName(List<string> participants);
    }
}
