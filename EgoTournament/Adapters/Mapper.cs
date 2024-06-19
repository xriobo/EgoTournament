using EgoTournament.Common;
using EgoTournament.Models;
using EgoTournament.Models.Enums;
using EgoTournament.Models.Riot;
using EgoTournament.Models.Riot.RawData;
using EgoTournament.Models.Views;


namespace EgoTournament.Adapters
{
    public static class Mapper
    {
        public static MatchView ToMatchViewModel(this Match match, string puuid)
        {
            MatchMode matchType;
            var participant = match.info.participants.Where(x => x.puuid.Equals(puuid, System.StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (participant == null)
            {
                return null;
            }

            if (Enum.IsDefined(typeof(MatchMode), match.info.queueId))
            {
                matchType = (MatchMode)match.info.queueId;
            }
            else
            {
                matchType = MatchMode.OTHER;
            }


            return new MatchView()
            {
                Assists = participant.assists,
                Champion = participant.championName,
                Deaths = participant.deaths,
                Kills = participant.kills,
                Win = participant.win,
                Lane = participant.teamPosition,
                MatchMode = matchType
            };
        }

        public static List<MatchView> ToListMatchViewModel(this List<Match> matches, string puuid)
        {
            List<MatchView> matchesViewModel = new List<MatchView>();
            if (matches == null || matches.Count < 1)
            {
                return matchesViewModel;
            }

            foreach (var match in matches)
            {
                matchesViewModel.Add(match.ToMatchViewModel(puuid));
            }

            return matchesViewModel;
        }

        public static SummonerView ToSummonerViewModel(this SummonerDto summonerDto)
        {
            if (summonerDto == null)
            {
                return null;
            }

            return new SummonerView()
            {
                Name = summonerDto.Name,
                Ranks = summonerDto.Ranks,
                RankSoloQ = summonerDto.RankSoloQ
            };
        }

        public static Tuple<SummonerView, List<MatchView>> ToSearchSummonerViewModels(this SummonerWithMatchesDto summonerWithMatchesDto)
        {
            var matchsView = new List<MatchView>();
            return new Tuple<SummonerView, List<MatchView>>(summonerWithMatchesDto.SummonerDto.ToSummonerViewModel(),
                                                            summonerWithMatchesDto.Matches != null ? summonerWithMatchesDto.Matches.ToList().ToListMatchViewModel(summonerWithMatchesDto.SummonerDto.Puuid) : matchsView);
        }

        public static RomanNumberEnum ToRomanNumber(this int number)
        {
            switch (number)
            {
                case 1:
                    return RomanNumberEnum.I;
                case 2:
                    return RomanNumberEnum.II;
                case 3:
                    return RomanNumberEnum.III;
                case 4:
                    return RomanNumberEnum.IV;
                case 5:
                    return RomanNumberEnum.V;
                case 6:
                    return RomanNumberEnum.VI;
                case 7:
                    return RomanNumberEnum.VII;
                default:
                    return RomanNumberEnum.VIII;
            }
        }

        public static TournamentResultDto ToTournamentResult(this TournamentDto tournament)
        {
            if (tournament == null) return null;

            return new TournamentResultDto()
            {
                FinishDate = tournament.FinishDate,
                HadReward = tournament.HasReward,
                Name = tournament.Name,
                OwnerId = tournament.OwnerId,
                Rules = tournament.Rules,
                Uid = tournament.Uid,
            };
        }

        private static ParticipantResultDto ToParticipantResult(this SummonerDto summoner, string position)
        {
            if (summoner?.RankSoloQ == null) return null;

            return new ParticipantResultDto()
            {
                LeaguePoints = summoner.RankSoloQ.LeaguePoints,
                Losses = summoner.RankSoloQ.Losses,
                Wins = summoner.RankSoloQ.Wins,
                Rank = summoner.RankSoloQ.Rank,
                SummonerName = summoner.Name,
                TierType = summoner.RankSoloQ.TierType,
                Winrate = summoner.RankSoloQ.Winrate,
                Position = position,
            };
        }

        public static IEnumerable<ParticipantResultDto> ToParticipantsResult(this IEnumerable<SummonerDto> summoners)
        {
            var participantsResult = new List<ParticipantResultDto>();
            if (summoners != null && summoners.Count() > 0)
            {
                string position;
                for (int i = 0; i < summoners.Count(); i++)
                {
                    var summoner = summoners.ToList()[i];
                    if (i == 0)
                    {
                        position = Globals.CHAMPION_POSITION;
                    }
                    else if (i == 1)
                    {
                        position = Globals.SECOND_POSITION;
                    }
                    else if (i == 2)
                    {
                        position = Globals.THIRD_POSITION;
                    }
                    else
                    {
                        position = string.Empty;
                    }

                    participantsResult.Add(summoner.ToParticipantResult(position));
                }
            }

            return participantsResult;
        }

        public static RankDto SetPropertiesRankDtoBySummonerDto(this SummonerDto summonerDto)
        {
            if (summonerDto.RankSoloQ == null)
            {
                summonerDto.RankSoloQ = Mocks.GetDefaultRankDto(summonerDto);
            }
            else
            {// SI NO SE PONE ESTE SET APARECE EL NOMBRE ORIGINAL NO EL CAMBIADO.
                summonerDto.RankSoloQ.SummonerName = summonerDto.Name;
                if (!string.IsNullOrEmpty(summonerDto.RankSoloQ.QueueType))
                {
                    summonerDto.RankSoloQ.QueueEnum = (QueueType)Enum.Parse(typeof(QueueType), summonerDto.RankSoloQ.QueueType);
                }

                if (!string.IsNullOrEmpty(summonerDto.RankSoloQ?.Tier))
                {
                    summonerDto.RankSoloQ.TierType = (TierEnum)Enum.Parse(typeof(TierEnum), summonerDto.RankSoloQ.Tier);
                }

                if (!string.IsNullOrEmpty(summonerDto.RankSoloQ?.Rank))
                {
                    summonerDto.RankSoloQ.Division = (RomanNumberEnum)Enum.Parse(typeof(RomanNumberEnum), summonerDto.RankSoloQ.Rank);
                }

                if (summonerDto.RankSoloQ.Wins != 0)
                {
                    summonerDto.RankSoloQ.WinrateNum = Math.Round((Convert.ToDecimal(summonerDto.RankSoloQ.Wins) / Convert.ToDecimal(summonerDto.RankSoloQ.Wins + summonerDto.RankSoloQ.Losses) * 100), MidpointRounding.ToEven);
                    summonerDto.RankSoloQ.Winrate = summonerDto.RankSoloQ.WinrateNum + "%";
                }
            }

            return summonerDto.RankSoloQ;
        }
    }
}
