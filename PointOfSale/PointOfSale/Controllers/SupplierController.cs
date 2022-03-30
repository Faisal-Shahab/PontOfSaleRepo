using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models;
using POS.DataAccessLayer.Models.Company;
using POS.DataAccessLayer.Models.Customer;
using POS.DataAccessLayer.Models.Subscriptions;
using POS.DataAccessLayer.ViewModels;

namespace PointOfSale.Controllers
{
    public class SupplierController : Controller
    {
        IRepository<SupplierModel> _supplierRepo;
        private readonly string user;
        private int languageId;
        private int companyId;
        IDropdownsServices _dropdownsServices;
        public SupplierController(IRepository<SupplierModel> supplierRepo, IDropdownsServices dropdownsServices)
        {
            _supplierRepo = supplierRepo;
            user = "test";
            languageId = 1;

            _dropdownsServices = dropdownsServices;
            _dropdownsServices.LanguageId = languageId;
            companyId = 1;
        }

        public IActionResult Index() => View();

        public async Task<JsonResult> GetSuppliers(SearchFilter filter)
        {
            var suppliers = await _supplierRepo.GetAll();
            suppliers = suppliers.Where(x => x.CompanyId == companyId);
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
                x.Address,
                CompanyName = (languageId == 1) ? x.Company.Name : x.Company.ArabicName
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

            if (ModelState.IsValid)
            {
                if (model.SupplierId > 0)
                {
                    model.UpdatedAt = DateTime.Now;
                    model.UpdatedBy = user;
                    result = await _supplierRepo.Update(model);
                }
                else
                {
                    model.CreateAt = DateTime.Now;
                    model.UpdatedAt = DateTime.Now;
                    model.CreateBy = user;
                    model.CompanyId = companyId;
                    result = await _supplierRepo.Insert(model);
                }
                return Json(new { status = result, message = result ? "Record has been submitted successfully" : "Error occured please try again" });
            }
            return Json(new { status = false, message = "Invalid input please fill the form" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var suppliers = await _supplierRepo.GetAll();
            var supplier = suppliers.FirstOrDefaultAsync(x => x.CompanyId == companyId &&
                                                              x.SupplierId == id);

            return View("Create", supplier);
        }

        public async Task<JsonResult> Delete(int id)
        {
            var result = await _supplierRepo.Delete(new SupplierModel { SupplierId = id });
            return Json(new { status = result, message = result ? "Record has been deleted successfully" : "Error occured please try again" });
        }
    }
}