﻿using EgoTournament.Models.Enums;
using EgoTournament.Models.Riot;

namespace EgoTournament.Models
{
    public static class Mocks
    {
        public static RankDto GetDefaultRankDto(SummonerDto summoner)
        {
            return new RankDto()
            {
                Rank = "VIII",
                LeaguePoints = 0,
                HotStreak = false,
                Losses = 0,
                Wins = 0,
                Tier = "WOOD",
                TierType = TierEnum.WOOD,
                Division = RomanNumberEnum.VIII,
                FreshBlood = true,
                Inactive = true,
                QueueType = "RANKED_SOLO_5x5",
                SummonerName = summoner.Name,
                SummonerId = summoner.Id
            };
        }

        public static List<SummonerDto> GetSummonerErrorDto(string errorMessage)
        {
            return new List<SummonerDto>()
            {
                new SummonerDto()
                {
                    Ranks = new List<RankDto>() { new RankDto()
                    {
                        SummonerName = errorMessage,
                        QueueType = QueueType.RANKED_SOLO_5x5.ToString(),
                        LeaguePoints = -100,
                        Division = RomanNumberEnum.VIII,
                        TierType = TierEnum.WOOD
                    }},
                    RankSoloQ = new RankDto()
                    {
                        SummonerName = errorMessage,
                        QueueType = QueueType.RANKED_SOLO_5x5.ToString(),
                        LeaguePoints = -100,
                        Division = RomanNumberEnum.VIII,
                        TierType = TierEnum.WOOD
                    },
                    SummonerLevel = -100,
                    Name = "ERROR API"
                }
            };
        }

        public static SummonerDto GetSummonerNotFoundDto()
        {
            return new SummonerDto()
            {
                Ranks = new List<RankDto>() { new RankDto()
                    {
                        SummonerName = "NOT FOUND",
                        QueueType = QueueType.RANKED_SOLO_5x5.ToString(),
                        LeaguePoints = 0,
                        Division = RomanNumberEnum.VIII,
                        TierType = TierEnum.WOOD
                    }},
                RankSoloQ = new RankDto()
                {
                    SummonerName = "NOT FOUND",
                    QueueType = QueueType.RANKED_SOLO_5x5.ToString(),
                    LeaguePoints = 0,
                    Division = RomanNumberEnum.VIII,
                    TierType = TierEnum.WOOD
                },
                SummonerLevel = 0,
                Name = "NOT FOUND"
            };
        }
    }
}
