using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgoTournament.Models.Firebase
{
    public class FirebaseErrorDetailDto
    {
        public string Message { get; set; }
        public string Domain { get; set; }
        public string Reason { get; set; }
    }
}
