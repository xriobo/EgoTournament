using EgoTournament.Models.Riot;
using Newtonsoft.Json;

namespace EgoTournament.Services.Implementations
{
    public static class GenericService
    {

        public async static Task<T> GetAsync<T>(string url, string key, int mins = 1, bool forceRefresh = false)
        {
            var json = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                if (url.Contains(RiotConstants.HeaderRiotUrl))
                {
                    client.DefaultRequestHeaders.Add(RiotConstants.HeaderRiotToken, RiotConstants.Token);
                }

                try
                {
                    json = await client.GetStringAsync(url);
                }
                catch (Exception)
                {
                    return default(T);
                }
            }

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}