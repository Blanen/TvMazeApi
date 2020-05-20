using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using VideolandAssignment.DTOModels;

namespace VideolandAssignment { 
    public class ShowsClient
    {
        private static readonly HttpClient Client = new HttpClient();
        //public async Show[] GetShows()
        //{
            
        //}

        public static async Task<List<ShowDTO>> GetAllShows()
        {
            var done = false;
            var page = 0;
            List<ShowDTO> showDtos = new List<ShowDTO>();
            while (!done)
            {
                var response = await Client.GetAsync($"http://api.tvmaze.com/shows?page={page}");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var showDtoPageList = JsonConvert.DeserializeObject<List<ShowDTO>>(json);
                    showDtos.AddRange(showDtoPageList);
                    page++;
                }
                else
                {
                    done = true;
                }
            }

            return showDtos;
        }

        public static async Task<Dictionary<int, long>> GetUpdates()
        {
            Dictionary<int, long> updatesDict = null;
            var response = await Client.GetAsync("http://api.tvmaze.com/updates/shows");
            if (response.IsSuccessStatusCode)
            {
                updatesDict = await response.Content.ReadAsAsync<Dictionary<int, long>>();
            }

            return updatesDict;
        }

        public static async Task<List<CastDto>> GetCast(long id)
        {
            while (true)
            {
                var response = await Client.GetAsync($"http://api.tvmaze.com/shows/{id}/cast");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<List<CastDto>>();
                }
                else
                {
                    await Task.Delay(10000);
                }
                
            }
        }
    }
}
