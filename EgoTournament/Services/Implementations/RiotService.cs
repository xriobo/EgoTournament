using EgoQ.Services;
using EgoTournament.Models;
using EgoTournament.Models.Enums;
using EgoTournament.Models.Riot;
using EgoTournament.Models.Riot.RawData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgoTournament.Services.Implementations
{
    public class RiotService : IRiotService
    {
        private List<RankDto> GetRanksBySummonerId(string summonerId) =>
                GenericService.GetAsync<List<RankDto>>(
                                        RiotConstants.EuwRiotUri + RiotConstants.EntriesBySummonerResource + summonerId,
                                        "SetSummonerRanksBySummonerId=" + summonerId, 1);

        public IEnumerable<SummonerDto> GetSummonersByParticipantsName(List<ParticipantDto> participantsDto)
        {
            List<SummonerDto> summonerDtos = new List<SummonerDto>();
            foreach (var participant in participantsDto)
            {
                if (participant != null)
                {
                    var summoner = GenericService.GetAsync<SummonerDto>(
                                        RiotConstants.EuwRiotUri + RiotConstants.SummonerByNameResource + participant.SummonerName,
                                        "GetParticipantSummonerDto=" + participant.SummonerName, 60);
                    if (summoner != null)
                    {
                        summonerDtos.Add(summoner);
                    }
                }
            }

            return summonerDtos;
        }

        public IEnumerable<PuuidDto> GetPuuidByParticipantsNameAndTagName(List<string> participants)
        {
            List<PuuidDto> puuids = new List<PuuidDto>();
            foreach (var participant in participants)
            {
                var participantSplit = participant.Split("#");
                if (participantSplit.Any())
                {
                    var summoner = GenericService.GetAsync<PuuidDto>(
                                        RiotConstants.EuropeApi + RiotConstants.AccountByRiotId + participantSplit[0] + "/" + participantSplit[1],
                                        "GetPuuidByParticipantsNameAndTagName=" + participantSplit[0] + "/" + participantSplit[1], 60);
                    if (summoner != null)
                    {
                        puuids.Add(summoner);
                    }
                }
            }

            return puuids;
        }

        public IEnumerable<SummonerDto> SetParticipantRanks(IEnumerable<SummonerDto> summonersRiot)
        {
            List<RankDto> ranksDto = new List<RankDto>();
            foreach (var summonerRiot in summonersRiot)
            {
                if (summonerRiot != null)
                {
                    ranksDto = GenericService.GetAsync<List<RankDto>>(
                                        RiotConstants.EuwRiotUri + RiotConstants.EntriesBySummonerResource + summonerRiot.Id,
                                        "SetParticipantRanks=" + summonerRiot.Id, 1);
                    summonerRiot.Ranks = ranksDto;
                    summonerRiot.RankSoloQ = summonerRiot.Ranks.FirstOrDefault(x => x.QueueType.Equals(QueueType.RANKED_SOLO_5x5.ToString(), StringComparison.OrdinalIgnoreCase));
                    summonerRiot.SetPropertiesRankDtoBySummonerDto();
                }
            }

            return summonersRiot;
        }

        public List<string> GetMatchIdListByPuuid(string puuid, int countMatches, bool rankeds = false) =>
            GenericService.GetAsync<List<string>>(rankeds ? RiotConstants.EuropeApi + RiotConstants.MatchesByPuuid + puuid + RiotConstants.ParameterCount + countMatches :
                                                                RiotConstants.EuropeApi + RiotConstants.MatchesByPuuid + puuid + RiotConstants.ParameterRankedWithCount + countMatches,
                                        "GetMatchIdListByPuuid=" + puuid, 1);

        public List<string> GetMatchIdListByPuuid(string puuid, int countMatches) =>
           GenericService.GetAsync<List<string>>(
                                       RiotConstants.EuropeApi + RiotConstants.MatchesByPuuid + puuid + RiotConstants.ParameterCount + countMatches,
                                       "GetMatchIdListByPuuid=" + puuid, 1);

        public List<Match> GetMatchesInfoByMatchIdList(List<string> matchIds)
        {
            List<Match> matches = new List<Match>();
            Parallel.ForEach(matchIds, (matchId) =>
            {
                var match = GenericService.GetAsync<Match>(
                                         RiotConstants.EuropeApi + RiotConstants.InfoMatchByMatchId + matchId,
                                         "GetMatchesInfoByMatchId=" + matchId, 60);
                if (match != null)
                {
                    matches.Add(match);
                }
            });

            return matches;
        }

        public SummonerWithMatchesDto GetSummonerWithMachesBySummonerNameAndTagLine(string summonerName, string tagLine, int countMatches)
        {
            SummonerWithMatchesDto summonerWithMatchesDto = new SummonerWithMatchesDto();
            SummonerDto summonerDto = new SummonerDto();
            List<Match> matches = new List<Match>();
            try
            {
                var summonerParticipant = ScheduleViewModel.SummonerDtos.FirstOrDefault(x => x.Name.Trim().Equals(summonerName.Trim(), StringComparison.InvariantCultureIgnoreCase));
                if (summonerParticipant != null)
                {
                    summonerDto = summonerParticipant;
                }
                else
                {
                    var account = GetPuuidBySummonerNameAndTagLine(summonerName, tagLine);
                    if (account != null)
                    {
                        summonerDto = GetSummonersByPuuid(new List<PuuidDto>() { new PuuidDto() { Puuid = account.Puuid, GameName = account.GameName } }).FirstOrDefault();
                    }
                }

                if (string.IsNullOrEmpty(summonerDto?.Id))
                {
                    summonerWithMatchesDto.SummonerDto = Mocks.GetSummonerNotFoundDto();
                    summonerWithMatchesDto.Matches = matches;
                }
                else
                {
                    summonerDto.Ranks = GetRanksBySummonerId(summonerDto.Id);
                    summonerDto.RankSoloQ = summonerDto.Ranks.Where(x => x.QueueType.Equals(QueueType.RANKED_SOLO_5x5.ToString(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    summonerDto.SetPropertiesRankDtoBySummonerDto();
                    summonerWithMatchesDto.SummonerDto = summonerDto;
                    summonerWithMatchesDto.Matches = GetMatchesInfo(summonerDto, countMatches);
                }
                // CUANDO SE SAQUE TODA LA INFO DE RIOT AÑADIR ESTE MAPEO DE AVERAGES.
                //summonerWithMatchesDto.SummonerDto = SetAverages(summonerDto, summonerWithMatchesDto.Matches);
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

        public List<Match> GetMatchesInfo(SummonerDto summonerDto, int countMatches, bool rankeds = false)
        {
            List<string> matchIds = new List<string>();
            List<Match> matches = new List<Match>();

            matchIds = GetMatchIdListByPuuid(summonerDto.Puuid, countMatches, rankeds);
            matches = GetMatchesInfoByMatchIdList(matchIds);

            return matches;
        }

        public IEnumerable<SummonerDto> GetSummonersByPuuid(IEnumerable<PuuidDto> puuids)
        {
            var summoners = new List<SummonerDto>();
            Parallel.ForEach(puuids, (participant) =>
            {
                var summoner = GenericService.GetAsync<SummonerDto>(
                                        RiotConstants.EuwRiotUri + RiotConstants.SummonerByPuuid + participant.Puuid,
                                        "GetSummonersByPuuid=" + participant.Puuid);
                if (summoner != null)
                {
                    summoner.Name = participant.GameName;
                    summoners.Add(summoner);
                }
            });

            return summoners;
        }

        private PuuidDto GetPuuidBySummonerNameAndTagLine(string summonerName, string tagLine)
        {
            PuuidDto puuid;
            puuid = GenericService.GetAsync<PuuidDto>(
                                RiotConstants.EuropeApi + RiotConstants.AccountByRiotId + summonerName + "/" + tagLine,
                                "GetPuuidByParticipantsNameAndTagName=" + summonerName, 60);

            return puuid;
        }
    }
}