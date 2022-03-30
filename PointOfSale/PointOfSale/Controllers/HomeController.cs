using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.DataAccessLayer;

namespace PointOfSale.Controllers
{
    public class HomeController : Controller
    {
        AppDbContext _app;
        public HomeController(AppDbContext app)
        {
            _app = app;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}