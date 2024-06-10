using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgoTournament.Models.Riot
{
    public class PuuidDto
    {
        [JsonProperty("puuid")]
        public string Puuid { get; set; }

        [JsonProperty("gameName")]
        public string GameName { get; set; }

        [JsonProperty("tagLine")]
        public string TagLine { get; set; }
    }
}
