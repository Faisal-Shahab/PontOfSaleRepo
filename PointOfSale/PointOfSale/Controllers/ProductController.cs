using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Model;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models.Security;
using POS.DataAccessLayer.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private int languageId;
        IProductServices _productServices;
        IDropdownsServices _dropdownsServices;
        UserManager<User> _userManager;
        public ProductController(IProductServices productServices, IDropdownsServices dropdownsServices, UserManager<User> userManager)
        {
            languageId = 1;
            _productServices = productServices;
            _dropdownsServices = dropdownsServices;
            _dropdownsServices.LanguageId = languageId;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetProducts(SearchFilter filter)
        {
            filter.LanguageId = languageId;
            var user = await _userManager.GetUserAsync(User);
            _productServices.CompanyId = (int)user.CompanyId;
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
            var user = await _userManager.GetUserAsync(User);
            _dropdownsServices.CompanyId = (int)user.CompanyId;
            ViewBag.Categories = await _dropdownsServices.CategoriesDropdown();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Create(ProductViewModel model)
        {
            bool result = false; ModelState.Remove("ProductId");

            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    _productServices.CompanyId = (int)user.CompanyId;
                    if (model.ProductId > 0)
                    {
                        result = await _productServices.Update(model, user.UserName);
                    }
                    else
                    {
                        result = await _productServices.Insert(model, user.UserName);
                    }
                    return Json(new { success = result, message = result ? "Record has been submitted successfully" : "Error occured please try again" });
                }
                return Json(new { success = false, message = "Invalid input please fill the form" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Operation failed" });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            _dropdownsServices.CompanyId = (int)user.CompanyId;
            _productServices.CompanyId = (int)user.CompanyId;
            ViewBag.Categories = await _dropdownsServices.CategoriesDropdown();
            var product = await _productServices.GetProductById(id);
            return View("Create", product);
        }

        public async Task<JsonResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            _productServices.CompanyId = (int)user.CompanyId;
            var result = await _productServices.Delete(id);
            return Json(new { status = result, message = result ? "Record has been deleted successfully" : "Error occured please try again" });
        }
    }
}