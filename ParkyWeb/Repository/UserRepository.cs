using Newtonsoft.Json;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ParkyWeb.Repository
{
    public class UserRepository:Repository<User>, IUserRepository
    {
        private readonly IHttpClientFactory clientFactory;

        public UserRepository(IHttpClientFactory clientFactory):base(clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public async Task<User> LoginAsync(string url, User obj)
        {
            if(obj == null)
            {
                return null;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            var httpClient = clientFactory.CreateClient();
            var response = await httpClient.SendAsync(request);

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<User>(jsonString);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> RegisterAsync(string url, User obj)
        {
            if(obj == null)
            {
                return false;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

            var httpClient = clientFactory.CreateClient();

            var response = await httpClient.SendAsync(request);

            if(response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
