namespace EgoTournament.Models.Riot
{
    public static class RiotConstants
    {
        public const string EuwRiotUri = "https://euw1.api.riotgames.com";

        public const string EunRiotUri = "https://eun1.api.riotgames.com";

        public const string JpRiotUri = "https://jp1.api.riotgames.com";

        public const string KRRiotUri = "https://kr.api.riotgames.com";

        public const string NaRiotUri = "https://na1.api.riotgames.com";

        public const string La1RiotUri = "https://la1.api.riotgames.com";

        public const string La2RiotUri = "https://la2.api.riotgames.com";

        public const string EuropeApi = "https://europe.api.riotgames.com";

        public const string EntriesBySummonerResource = "/lol/league/v4/entries/by-summoner/";

        public const string SummonerByNameResource = "/lol/summoner/v4/summoners/by-name/";

        public const string SummonerByPuuid = "/lol/summoner/v4/summoners/by-puuid/";

        public const string AccountByRiotId = "/riot/account/v1/accounts/by-riot-id/";

        public const string MatchesByPuuid = "/lol/match/v5/matches/by-puuid/";

        public const string ParameterCount = "/ids?count=";

        public const string ParameterRankedWithCount = "/ids?type=ranked&count=";

        public const string InfoMatchByMatchId = "/lol/match/v5/matches/";

        public const string HeaderRiotToken = "X-Riot-Token";

        public const string HeaderRiotUrl = "riotgames";

        public const string NotFoundMessage = "404 (Not Found)";

        public const string Token = "RGAPI-3e7c495e-5fbc-4b4d-b97a-b31060f0c7ac";
    }
}