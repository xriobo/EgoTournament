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
    }
}
