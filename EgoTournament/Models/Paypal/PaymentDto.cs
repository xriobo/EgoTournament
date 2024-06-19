using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgoTournament.Models.Paypal
{
    public class PaymentDto
    {
        public string Amount { get; set; }

        public string Currency {  get; set; }
    }
}
