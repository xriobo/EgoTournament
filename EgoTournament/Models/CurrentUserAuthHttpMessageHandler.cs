using Firebase.Auth;
using System.Net.Http.Headers;

namespace EgoTournament.Models
{
    public class CurrentUserAuthHttpMessageHandler : DelegatingHandler
    {
        private readonly CurrentUserStore _currentUserStore;

        public CurrentUserAuthHttpMessageHandler(CurrentUserStore currentUserStore)
        {
            _currentUserStore = currentUserStore;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string accessToken = await GetAccessToken();

            if (accessToken != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private Task<string> GetAccessToken()
        {
            User currentUser = _currentUserStore.CurrentUser;

            if (currentUser == null)
            {
                return null;
            }

            try
            {
                return currentUser.GetIdTokenAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}