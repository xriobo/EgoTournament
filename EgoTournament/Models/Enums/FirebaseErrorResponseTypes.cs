using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgoTournament.Models.Enums
{
    public enum FirebaseErrorResponseTypes
    {
        EMAIL_NOT_FOUND,
        INVALID_PASSWORD,
        USER_DISABLED,
        INVALID_EMAIL,
        TOO_MANY_ATTEMPTS_TRY_LATER,
        EMAIL_EXISTS,
        WEAK_PASSWORD,
        OPERATION_NOT_ALLOWED
    }
}
