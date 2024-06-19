using EgoTournament.Adapters;
using EgoTournament.Models;
using EgoTournament.Models.Enums;
using EgoTournament.Models.Riot;
using EgoTournament.Models.Riot.RawData;

namespace EgoTournament.Services.Implementations
{
    public class RiotService : IRiotService
    {
        public async Task<Tuple<IEnumerable<PuuidDto>, List<string>>> GetPuuidByParticipantsNameAndTagName(List<string> participants)
        {
            var tasks = new List<Task<Tuple<PuuidDto, string>>>();
            List<string> participantsNotFound = new List<string>();

            foreach (var participant in participants)
            {
                var participantSplit = participant.Split("#");
                if (participantSplit.Length == 2)
                {
                    var url = RiotConstants.EuropeApi + RiotConstants.AccountByRiotId + participantSplit[0] + "/" + participantSplit[1];
                    var key = "GetPuuidByParticipantsNameAndTagName=" + participantSplit[0] + "/" + participantSplit[1];
                    tasks.Add(GetSummonerAsync(url, key, participant));
                }
            }

            var results = await Task.WhenAll(tasks);

            var puuids = results.Where(r => r.Item1 != null).Select(r => r.Item1).ToList();
            participantsNotFound.AddRange(results.Where(r => r.Item1 == null).Select(r => r.Item2));

            return new Tuple<IEnumerable<PuuidDto>, List<string>>(puuids, participantsNotFound);
        }

        public async Task<IEnumerable<SummonerDto>> SetParticipantRanks(IEnumerable<SummonerDto> summonersRiot)
        {
            var tasks = summonersRiot.Select(async summonerRiot =>
            {
                if (summonerRiot != null)
                {
                    var ranksDto = await GenericService.GetAsync<List<RankDto>>(
                                        RiotConstants.EuwRiotUri + RiotConstants.EntriesBySummonerResource + summonerRiot.Id,
                                        "SetParticipantRanks=" + summonerRiot.Id, 1);
                    if (ranksDto != null)
                    {
                        summonerRiot.Ranks = ranksDto;
                        summonerRiot.RankSoloQ = summonerRiot.Ranks.FirstOrDefault(x => x.QueueType.Equals(QueueType.RANKED_SOLO_5x5.ToString(), StringComparison.OrdinalIgnoreCase));
                        summonerRiot.SetPropertiesRankDtoBySummonerDto();
                    }
                }
                return summonerRiot;
            }).ToList();

            var summoners = await Task.WhenAll(tasks);

            return GetOrdererSummonersByTierDivisionAndLeaguePointsDescending(summoners);
        }

        public async Task<List<string>> GetMatchIdListByPuuid(string puuid, int countMatches, bool rankeds = false) =>
            await GenericService.GetAsync<List<string>>(rankeds ? RiotConstants.EuropeApi + RiotConstants.MatchesByPuuid + puuid + RiotConstants.ParameterCount + countMatches :
                                                                RiotConstants.EuropeApi + RiotConstants.MatchesByPuuid + puuid + RiotConstants.ParameterRankedWithCount + countMatches,
                                        "GetMatchIdListByPuuid=" + puuid, 1);

        public async Task<List<Match>> GetMatchesByMatchesId(List<string> matchIds)
        {
            var tasks = matchIds.Select(async matchId =>
            {
                var match = await GenericService.GetAsync<Match>(
                                RiotConstants.EuropeApi + RiotConstants.InfoMatchByMatchId + matchId,
                                "GetMatchesInfoByMatchId=" + matchId, 60);
                return match;
            }).ToList();

            var matches = await Task.WhenAll(tasks);

            return matches.Where(m => m != null).ToList();
        }

        public async Task<SummonerWithMatchesDto> GetSummonerWithMachesBySummonerNameAndTagLine(string summonerName, string tagLine, int countMatches)
        {
            SummonerWithMatchesDto summonerWithMatchesDto = new SummonerWithMatchesDto();
            SummonerDto summonerDto = new SummonerDto();
            List<Match> matches = new List<Match>();
            try
            {
                var account = await GetPuuidBySummonerNameAndTagLine(summonerName, tagLine);
                if (account != null)
                {
                    var summoners = await GetSummonersByPuuid(new List<PuuidDto>() { new PuuidDto() { Puuid = account.Puuid, GameName = account.GameName } });
                    summonerDto = summoners.FirstOrDefault();
                }

                if (string.IsNullOrEmpty(summonerDto?.Id))
                {
                    summonerWithMatchesDto.SummonerDto = Mocks.GetSummonerNotFoundDto();
                    summonerWithMatchesDto.Matches = matches;
                }
                else
                {
                    summonerDto.Ranks = await GetRanksBySummonerId(summonerDto.Id);
                    summonerDto.RankSoloQ = summonerDto.Ranks.Where(x => x.QueueType.Equals(QueueType.RANKED_SOLO_5x5.ToString(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    summonerDto.SetPropertiesRankDtoBySummonerDto();
                    summonerWithMatchesDto.SummonerDto = summonerDto;
                    summonerWithMatchesDto.Matches = await GetMatchesInfo(summonerDto, countMatches);
                }
            }
            catch (Exception ex)
            {
                return new SummonerWithMatchesDto()
                {
                    SummonerDto = new SummonerDto()
                    {
                        Ranks = new List<RankDto>() {
                            new RankDto()
                            {
                                SummonerName = ex.Message,
                                QueueType = QueueType.RANKED_SOLO_5x5.ToString(),
                                LeaguePoints = -100,
                                Division = RomanNumberEnum.VIII
                            }
                        },
                        SummonerLevel = -100,
                        Name = "ERROR API"
                    }
                };
            }

            return summonerWithMatchesDto;
        }

        public async Task<List<Match>> GetMatchesInfo(SummonerDto summonerDto, int countMatches, bool rankeds = false)
        {
            List<string> matchIds = new List<string>();
            List<Match> matches = new List<Match>();

            matchIds = await GetMatchIdListByPuuid(summonerDto.Puuid, countMatches, rankeds);
            matches = await GetMatchesByMatchesId(matchIds);

            return matches;
        }

        public async Task<IEnumerable<SummonerDto>> GetSummonersByPuuid(IEnumerable<PuuidDto> puuids)
        {
            var summoners = new List<SummonerDto>();
            foreach (var puuid in puuids)
            {
                var summoner = await GenericService.GetAsync<SummonerDto>(
                                        RiotConstants.EuwRiotUri + RiotConstants.SummonerByPuuid + puuid.Puuid,
                                        "GetSummonersByPuuid=" + puuid.Puuid);
                if (summoner != null)
                {
                    summoner.Name = puuid.GameName;
                    summoners.Add(summoner);
                }
            }

            return summoners;
        }

        private async Task<List<RankDto>> GetRanksBySummonerId(string summonerId) =>
                await GenericService.GetAsync<List<RankDto>>(
                                        RiotConstants.EuwRiotUri + RiotConstants.EntriesBySummonerResource + summonerId,
                                        "SetSummonerRanksBySummonerId=" + summonerId, 1);

        private List<SummonerDto> GetOrdererSummonersByTierDivisionAndLeaguePointsDescending(IEnumerable<SummonerDto> summonerDtos)
        {
            return summonerDtos.OrderByDescending(x => (int)x.RankSoloQ.TierType).ThenByDescending(x => (int)x.RankSoloQ.Division).ThenByDescending(x => x.RankSoloQ.LeaguePoints).ToList();
        }

        private async Task<PuuidDto> GetPuuidBySummonerNameAndTagLine(string summonerName, string tagLine)
        {
            PuuidDto puuid;
            puuid = await GenericService.GetAsync<PuuidDto>(
                                RiotConstants.EuropeApi + RiotConstants.AccountByRiotId + summonerName + "/" + tagLine,
                                "GetPuuidByParticipantsNameAndTagName=" + summonerName, 60);

            return puuid;
        }

        private async Task<Tuple<PuuidDto, string>> GetSummonerAsync(string url, string key, string participant)
        {
            var summoner = await GenericService.GetAsync<PuuidDto>(url, key, 60);

            if (summoner != null)
            {
                return new Tuple<PuuidDto, string>(summoner, null);
            }
            else
            {
                return new Tuple<PuuidDto, string>(null, participant);
            }
        }
    }
}