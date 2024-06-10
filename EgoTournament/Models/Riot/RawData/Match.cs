namespace EgoTournament.Models.Riot.RawData
{
    public class Match
    {
        public Metadata metadata { get; set; }
        public Info info { get; set; }

        public class Metadata
        {
            public string dataVersion { get; set; }
            public string matchId { get; set; }
            public List<string> participants { get; set; }
        }

        public class StatPerks
        {
            public int defense { get; set; }
            public int flex { get; set; }
            public int offense { get; set; }
        }

        public class Selection
        {
            public int perk { get; set; }
            public int var1 { get; set; }
            public int var2 { get; set; }
            public int var3 { get; set; }
        }

        public class Style
        {
            public string description { get; set; }
            public List<Selection> selections { get; set; }
            public int style { get; set; }
        }

        public class Perks
        {
            public StatPerks statPerks { get; set; }
            public List<Style> styles { get; set; }
        }

        public class Participant
        {
            public int assists { get; set; }
            public int baronKills { get; set; }
            public int bountyLevel { get; set; }
            public int champExperience { get; set; }
            public int champLevel { get; set; }
            public int championId { get; set; }
            public string championName { get; set; }
            public int championTransform { get; set; }
            public int consumablesPurchased { get; set; }
            public int damageDealtToBuildings { get; set; }
            public int damageDealtToObjectives { get; set; }
            public int damageDealtToTurrets { get; set; }
            public int damageSelfMitigated { get; set; }
            public int deaths { get; set; }
            public int detectorWardsPlaced { get; set; }
            public int doubleKills { get; set; }
            public int dragonKills { get; set; }
            public bool firstBloodAssist { get; set; }
            public bool firstBloodKill { get; set; }
            public bool firstTowerAssist { get; set; }
            public bool firstTowerKill { get; set; }
            public bool gameEndedInEarlySurrender { get; set; }
            public bool gameEndedInSurrender { get; set; }
            public int goldEarned { get; set; }
            public int goldSpent { get; set; }
            public string individualPosition { get; set; }
            public int inhibitorKills { get; set; }
            public int inhibitorsLost { get; set; }
            public int item0 { get; set; }
            public int item1 { get; set; }
            public int item2 { get; set; }
            public int item3 { get; set; }
            public int item4 { get; set; }
            public int item5 { get; set; }
            public int item6 { get; set; }
            public int itemsPurchased { get; set; }
            public int killingSprees { get; set; }
            public int kills { get; set; }
            public string lane { get; set; }
            public int largestCriticalStrike { get; set; }
            public int largestKillingSpree { get; set; }
            public int largestMultiKill { get; set; }
            public int longestTimeSpentLiving { get; set; }
            public int magicDamageDealt { get; set; }
            public int magicDamageDealtToChampions { get; set; }
            public int magicDamageTaken { get; set; }
            public int neutralMinionsKilled { get; set; }
            public int nexusKills { get; set; }
            public int nexusLost { get; set; }
            public int objectivesStolen { get; set; }
            public int objectivesStolenAssists { get; set; }
            public int participantId { get; set; }
            public int pentaKills { get; set; }
            public Perks perks { get; set; }
            public int physicalDamageDealt { get; set; }
            public int physicalDamageDealtToChampions { get; set; }
            public int physicalDamageTaken { get; set; }
            public int profileIcon { get; set; }
            public string puuid { get; set; }
            public int quadraKills { get; set; }
            public string riotIdName { get; set; }
            public string riotIdTagline { get; set; }
            public string role { get; set; }
            public int sightWardsBoughtInGame { get; set; }
            public int spell1Casts { get; set; }
            public int spell2Casts { get; set; }
            public int spell3Casts { get; set; }
            public int spell4Casts { get; set; }
            public int summoner1Casts { get; set; }
            public int summoner1Id { get; set; }
            public int summoner2Casts { get; set; }
            public int summoner2Id { get; set; }
            public string summonerId { get; set; }
            public int summonerLevel { get; set; }
            public string summonerName { get; set; }
            public bool teamEarlySurrendered { get; set; }
            public int teamId { get; set; }
            public string teamPosition { get; set; }
            public int timeCCingOthers { get; set; }
            public int timePlayed { get; set; }
            public int totalDamageDealt { get; set; }
            public int totalDamageDealtToChampions { get; set; }
            public int totalDamageShieldedOnTeammates { get; set; }
            public int totalDamageTaken { get; set; }
            public int totalHeal { get; set; }
            public int totalHealsOnTeammates { get; set; }
            public int totalMinionsKilled { get; set; }
            public int totalTimeCCDealt { get; set; }
            public int totalTimeSpentDead { get; set; }
            public int totalUnitsHealed { get; set; }
            public int tripleKills { get; set; }
            public int trueDamageDealt { get; set; }
            public int trueDamageDealtToChampions { get; set; }
            public int trueDamageTaken { get; set; }
            public int turretKills { get; set; }
            public int turretsLost { get; set; }
            public int unrealKills { get; set; }
            public int visionScore { get; set; }
            public int visionWardsBoughtInGame { get; set; }
            public int wardsKilled { get; set; }
            public int wardsPlaced { get; set; }
            public bool win { get; set; }
        }

        public class Baron
        {
            public bool first { get; set; }
            public int kills { get; set; }
        }

        public class Champion
        {
            public bool first { get; set; }
            public int kills { get; set; }
        }

        public class Dragon
        {
            public bool first { get; set; }
            public int kills { get; set; }
        }

        public class Inhibitor
        {
            public bool first { get; set; }
            public int kills { get; set; }
        }

        public class RiftHerald
        {
            public bool first { get; set; }
            public int kills { get; set; }
        }

        public class Tower
        {
            public bool first { get; set; }
            public int kills { get; set; }
        }

        public class Objectives
        {
            public Baron baron { get; set; }
            public Champion champion { get; set; }
            public Dragon dragon { get; set; }
            public Inhibitor inhibitor { get; set; }
            public RiftHerald riftHerald { get; set; }
            public Tower tower { get; set; }
        }

        public class Team
        {
            public List<object> bans { get; set; }
            public Objectives objectives { get; set; }
            public int teamId { get; set; }
            public bool win { get; set; }
        }

        public class Info
        {
            public long gameCreation { get; set; }
            public int gameDuration { get; set; }
            public long gameId { get; set; }
            public string gameMode { get; set; }
            public string gameName { get; set; }
            public long gameStartTimestamp { get; set; }
            public string gameType { get; set; }
            public string gameVersion { get; set; }
            public int mapId { get; set; }
            public List<Participant> participants { get; set; }
            public string platformId { get; set; }
            public int queueId { get; set; }
            public List<Team> teams { get; set; }
        }
    }
}
