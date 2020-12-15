using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCTutorial.Controllers
{
    public class HomeController:Controller
    {
        public HomeController(IIpReflection ipinfo)
        {
            
        }
        public IActionResult Index()
        {
            return View("/Views/Home/Index.cshtml");
        }
    }
    
}
