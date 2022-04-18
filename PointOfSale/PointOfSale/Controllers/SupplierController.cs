using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Admin.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models;
using POS.DataAccessLayer.Models.Company;
using POS.DataAccessLayer.Models.Customer;
using POS.DataAccessLayer.Models.Security;
using POS.DataAccessLayer.Models.Subscriptions;
using POS.DataAccessLayer.ViewModels;

namespace PointOfSale.Controllers
{

    [Authorize(Roles = "Admin")]
   // [AuthorizedAction]
    public class SupplierController : Controller
    {
        IRepository<SupplierModel> _supplierRepo;       
        private int languageId;
        IDropdownsServices _dropdownsServices;
        UserManager<User> _userManager;
        public SupplierController(IRepository<SupplierModel> supplierRepo,
                                IDropdownsServices dropdownsServices, UserManager<User> userManager)
        {
            _supplierRepo = supplierRepo;            
            languageId = 1;

            _dropdownsServices = dropdownsServices;
            _dropdownsServices.LanguageId = languageId;
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        public async Task<JsonResult> GetSuppliers(SearchFilter filter)
        {
            var user = await _userManager.GetUserAsync(User);
            var suppliers = await _supplierRepo.GetAll();
            suppliers = suppliers.Where(x => x.CompanyId == user.CompanyId);
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var isNumber = double.TryParse(filter.SearchTerm, out double numericValue);
                if (isNumber)
                {
                    suppliers = suppliers.Where(x => x.SupplierId == Convert.ToInt32(filter.SearchTerm));
                }
                else
                {
                    suppliers = suppliers.Where(x => x.Name.ToLower().Contains(filter.SearchTerm.ToLower()));
                }
            }
            var total = suppliers.Count();
            var data = suppliers.Select(x => new
            {
                x.SupplierId,
                x.Name,
                x.ContactNo,
                x.Email,
                x.Address
            }).OrderByDescending(x => x.SupplierId).Skip(filter.Start).Take(filter.PageLength);
            return Json(new
            {
                sEcho = filter.Draw,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = data
            });
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<JsonResult> Create(SupplierModel model)
        {
            bool result; ModelState.Remove("SupplierId");
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (model.SupplierId > 0)
                    {
                        model.UpdatedAt = DateTime.Now;
                        model.UpdatedBy = user.UserName;
                        model.CompanyId = (int)user.CompanyId;
                        result = await _supplierRepo.Update(model);
                    }
                    else
                    {
                        model.CreateAt = DateTime.Now;
                        model.UpdatedAt = DateTime.Now;
                        model.CreateBy = user.UserName;
                        model.CompanyId = (int)user.CompanyId;
                        result = await _supplierRepo.Insert(model);
                    }
                    return Json(new { status = result, message = result ? "Record has been submitted successfully" : "Error occured please try again" });
                }
                return Json(new { status = false, message = "Invalid input please fill the form" });
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var suppliers = await _supplierRepo.GetAll();
            var supplier = await suppliers.FirstOrDefaultAsync(x => x.CompanyId == user.CompanyId &&
                                                              x.SupplierId == id);

            return View("Create", supplier);
        }

        public async Task<JsonResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var suppliers = await _supplierRepo.GetAll();
            var supplier = suppliers.FirstOrDefault(x => x.SupplierId == id && x.CompanyId == user.CompanyId);
            var result = await _supplierRepo.Delete(supplier);
            return Json(new { status = result, message = result ? "Record has been deleted successfully" : "Error occured please try again" });
        }
    }
}