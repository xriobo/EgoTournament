using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgoTournament.Services
{
    public interface ILoginService
    {
        bool SignIn(string email, string password);

        bool SignUp(string username, string password, string email);

        bool LogOut();
    }
}
