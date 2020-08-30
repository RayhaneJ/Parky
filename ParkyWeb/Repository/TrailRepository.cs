using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ParkyWeb.Repository
{
    public class TrailRepository:Repository<Trail>, ITrailRepository
    {

        public TrailRepository(IHttpClientFactory clientFactory):base(clientFactory)
        {
        }
    }
}
