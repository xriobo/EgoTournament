using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EgoTournament.Models.Firebase
{
    public class FirebaseErrorDataDto
    {
        public int Code { get; set; }
        public string Message { get; set; }

        [JsonPropertyName("errors")]
        public List<FirebaseErrorDetailDto> ErrorsDetail { get; set; }
    }
}
