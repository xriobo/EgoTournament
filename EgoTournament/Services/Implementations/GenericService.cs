using EgoTournament.Models.Riot;
using Newtonsoft.Json;
using System.Net.Http;

namespace EgoTournament.Services.Implementations
{
    public static class GenericService
    {

        public static T GetAsync<T>(string url, string key, int mins = 1, bool forceRefresh = false)
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
                    json = client.GetStringAsync(url).Result;
                }
                catch (Exception ex)
                {
                    return default(T);
                }
            }

            return JsonConvert.DeserializeObject<T>(json);
        }


        public static string GetStringAsync(string url)
        {
            var result = "ERROR:";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    result = client.GetStringAsync(url)?.Result;
                }
                catch (Exception ex)
                {
                    return result += ex.Message;
                }
            }

            return result;
        }
    }
}