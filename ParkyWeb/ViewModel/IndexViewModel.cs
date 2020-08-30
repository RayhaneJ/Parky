using ParkyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb.ViewModel
{
    public class IndexViewModel
    {
        public IEnumerable<NationalPark> NationalParks { get; set; }
        public IEnumerable<Trail> Trails { get; set; }
    }
}
