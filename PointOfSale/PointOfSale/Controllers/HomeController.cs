using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.DataAccessLayer;

namespace PointOfSale.Controllers
{
    [Authorize]
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


        //[HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            culture = string.IsNullOrEmpty(culture) ? "en-US" : culture;
            HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            return RedirectToAction("index", "pilgrams");
        }
    }
}