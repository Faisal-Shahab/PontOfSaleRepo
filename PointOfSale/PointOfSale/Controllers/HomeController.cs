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
using POS.DataAccessLayer.ViewModels;

namespace PointOfSale.Controllers
{
    //[Authorize]
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

        public async Task<JsonResult> GetSales(SearchFilter filter)
        {
            var fromDate = Convert.ToDateTime($"{filter.FromDate} 00:00:00");
            var toDate = Convert.ToDateTime($"{filter.ToDate} 23:59:59");
            try
            {

                var query = _app.SaleOrder.Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate); //.ToListAsync();
                var sale = await query.Select(x => new
                {
                    Label = (filter.FromDate == filter.ToDate) ? "Hour " + x.CreatedAt.ToString("HH")
                                                               : x.CreatedAt.ToString("yyyy-MM-dd")
                    ,
                    x.Total
                }).ToListAsync();
                var sales = sale.GroupBy(x => x.Label).Select(s => new { Label = s.Key, Total = s.Sum(x => x.Total), Orders = s.Count() }).ToList();

                var cat = await query.SelectMany(x => x.SaleOrderDetails
                .Select(x => new
                {
                    CatId = x.Product.Category.CategoryId,
                    CatName = x.Product.Category.Name,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name
                })).ToListAsync();

                var categories = cat
                    .GroupBy(x => x.CatId)
                    .Select(c => new { SoldCat = c.Count(), CatName = c.FirstOrDefault().CatName })
                    .OrderByDescending(x => x.SoldCat).Take(10).ToList();

                var products = cat.GroupBy(x => x.ProductId)
                     .Select(c => new { ProductsSold = c.Count(), ProductName = c.FirstOrDefault().ProductName })
                     .OrderByDescending(x => x.ProductsSold).Take(10).ToList();

                return Json(new { sales = sales, categories = categories, products = products });
            }
            catch (Exception e)
            {
                throw;
            }
        }


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
            return RedirectToAction("index", "home");
        }
    }
}