using Microsoft.AspNetCore.Mvc.Rendering;
using ParkyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb.ViewModel
{
    public class TrailViewModel
    {
        public IEnumerable<SelectListItem> NationalParkList { get; set; }
        public Trail Trail { get; set; }
    }
}
