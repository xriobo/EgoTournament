using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgoTournament.Services.Implementations
{
    public class LoginService : ILoginService
    {
        public bool LogOut()
        {
            throw new NotImplementedException();
        }

        public bool SignIn(string email, string password)
        {
            if (email.Equals("someemail", StringComparison.InvariantCulture) && password.Equals("password", StringComparison.InvariantCulture))
            return true;
            else return false;
        }

        public bool SignUp(string username, string password, string email)
        {
            throw new NotImplementedException();
        }
    }
}
