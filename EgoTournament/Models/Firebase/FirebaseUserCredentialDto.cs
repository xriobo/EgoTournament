using Firebase.Auth;

namespace EgoTournament.Models.Firebase
{
    public class FirebaseUserCredentialDto
    {
        public string Uid { get; set; }
        public bool IsAnonymous { get; set; }
        public UserInfo Info { get; set; }
        public FirebaseCredential Credential { get; set; }
    }
}
