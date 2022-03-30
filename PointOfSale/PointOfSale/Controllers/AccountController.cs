using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.DataAccessLayer;

namespace PointOfSale.Controllers
{
    public class AccountController : Controller
    {
        AppDbContext _appContext;
        public AccountController(AppDbContext appContext)
        {
            _appContext = appContext;
        }

        public IActionResult Accounts()
        {
            return View();
        }

        //public JsonResult GetAccounts()
        //{
        //    return Json(0);
        //}

        //public IActionResult Registration()
        //{
        //    return View();
        //}

        //public IActionResult CreateRegistration()
        //{
        //    return View();
        //}

        //public IActionResult EditRegistration()
        //{
        //    return View();
        //}

        //public IActionResult EditRegistration()
        //{
        //    return View();
        //}

        //public JsonResult DisableAccount()
        //{
        //    return Json(0);
        //}
    }
}