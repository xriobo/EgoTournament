using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgoTournament.Models.Riot
{
    public static class SearchSummonerDto
    {
        public static IEnumerable<SummonerDto> SummonerDtos { get; set; }

        public static DateTime ExtractSummonersDateTime { get; set; }

        public static SummonerWithMatchesDto SummonerWithMatchesDto { get; set; }

        public static DateTime ExtractSummonerDateTime { get; set; }

    }
}
