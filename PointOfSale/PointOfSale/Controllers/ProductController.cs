using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PointOfSale.Model;
using POS.DataAccessLayer;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models.Category;
using POS.DataAccessLayer.Models.Subscriptions;
using POS.DataAccessLayer.ViewModels;

namespace PointOfSale.Controllers
{
    public class ProductController : Controller
    {
        private int languageId;
        IProductServices _productServices;
        IDropdownsServices _dropdownsServices;
        public ProductController(IProductServices productServices, IDropdownsServices dropdownsServices)
        {
            languageId = 1;
            _productServices = productServices;
            _dropdownsServices = dropdownsServices;
            _dropdownsServices.LanguageId = 1;
            _productServices.CompanyId = 1;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetProducts(SearchFilter filter)
        {
            filter.LanguageId = languageId;
            var data = await _productServices.GetProducts(filter);
            return Json(new
            {
                sEcho = filter.Draw,
                iTotalRecords = data.FirstOrDefault().Total,
                iTotalDisplayRecords = data.FirstOrDefault().Total,
                aaData = data
            });
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _dropdownsServices.CategoriesDropdown();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Create(ProductViewModel model, string user)
        {
            bool result = false; ModelState.Remove("ProductId");

            if (ModelState.IsValid)
            {
                if (model.ProductId > 0)
                {
                    result = await _productServices.Update(model, user);
                }
                else
                {
                    result = await _productServices.Insert(model, user);
                }
                return Json(new { status = result, message = result ? "Record has been submitted successfully" : "Error occured please try again" });
            }
            return Json(new { status = false, message = "Invalid input please fill the form" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Categories = await _dropdownsServices.CategoriesDropdown();
            var product = await _productServices.GetProductById(id);
            return View("Create", product);
        }

        public async Task<JsonResult> Delete(int id)
        {
            var result = await _productServices.Delete(id);
            return Json(new { status = result, message = result ? "Record has been deleted successfully" : "Error occured please try again" });
        }
    }
}