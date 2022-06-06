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
using POS.DataAccessLayer.Models.Supplier;
using POS.DataAccessLayer.ViewModels;

namespace PointOfSale.Controllers
{

    [Authorize(Roles = "Admin")]
    // [AuthorizedAction]
    public class SupplierController : Controller
    {
        IRepository<SupplierModel> _supplierRepo;
        IRepository<SupplierTransaction> _supplierTrans;
        private int languageId;
        System.Globalization.CultureInfo info;
        IDropdownsServices _dropdownsServices;
        UserManager<User> _userManager;
        public SupplierController(IRepository<SupplierModel> supplierRepo, IRepository<SupplierTransaction> supplierTrans,
        IDropdownsServices dropdownsServices, UserManager<User> userManager)
        {
            _supplierRepo = supplierRepo;
            info = System.Globalization.CultureInfo.CurrentCulture;
            languageId = info.TwoLetterISOLanguageName == "ar" ? 2 : 1;
            _dropdownsServices = dropdownsServices;
            _dropdownsServices.LanguageId = languageId;
            _userManager = userManager;
            _supplierTrans = supplierTrans;
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
                x.Balance,
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
                        if (model.Balance > 0)
                        {
                            SupplierTransaction transaction = new SupplierTransaction { Balance = model.Balance };
                            result = await _supplierTrans.Insert(transaction);
                        }
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

        public async Task<IActionResult> SupplierPayment(int id)
        {
            var supplier = await _supplierRepo.GetById(id);
            var balance = supplier?.Balance ?? 0.00m;
            return View(new SupplierTransaction { SupplierId = id, Balance = balance, Debit = balance });
        }

        //public async Task<JsonResult> GetSupTransactionBalace(int id)
        //{
        //    var sup = await _supplierRepo.GetById(id);
        //    return Json(sup.Balance);
        //}

        public async Task<JsonResult> GetSupTransactions(int id)
        {
            var sup = await _supplierTrans.GetAll();
            return Json(new
            {
                data = sup.Where(x => x.SupplierId == id).OrderByDescending(x => x.Id)
                                    .Select(x => new
                                    {
                                        x.Debit,
                                        x.Credit,
                                        x.Balance,
                                        CreatedAt = x.CreateAt.ToString("yyyy-MMM-dd")
                                    }).ToList()
            });
        }

        public async Task<JsonResult> CreateTransaction(SupplierTransaction model)
        {
            try
            {
                model.PurchaseOrderId = null;
                var user = await _userManager.GetUserAsync(User);
                model.CompanyId = (int)user.CompanyId;
                model.CreateBy = user.UserName;
                var result = await _supplierTrans.Insert(model);
                var sup = await _supplierRepo.GetById(model.SupplierId);
                sup.Balance = model.Balance;
                sup.UpdatedBy = user.UserName;
                sup.UpdatedAt = DateTime.Now;
                result = await _supplierRepo.Update(sup);
                return Json(new { status = result });
            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }

        }
    }
}