using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models.Company;
using POS.DataAccessLayer.Models.Security;
using POS.DataAccessLayer.Models.Subscriptions;

namespace PointOfSale.Controllers
{
    public class SubscriptionController : Controller
    {
        IRepository<SubscriptionModel> _subscriptionRepo;
        private readonly IRepository<CompanySubscriptionModel> _companySubscriptionRepo;
        IDropdownsServices _dropdownsServices;        
        private int languageId;
        System.Globalization.CultureInfo info;
        UserManager<User> _userManager;
        public SubscriptionController(IDropdownsServices dropdownsServices, IRepository<SubscriptionModel> subscriptionRepo,
            IRepository<CompanySubscriptionModel> companySubscriptionRepo, UserManager<User> userManager)
        {
            _subscriptionRepo = subscriptionRepo;
            _companySubscriptionRepo = companySubscriptionRepo;
            _dropdownsServices = dropdownsServices;           
            languageId = info.TwoLetterISOLanguageName == "ar" ? 2 : 1;
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        public async Task<JsonResult> GetSubscriptions()
        {
            var subscriptions = await _subscriptionRepo.GetAll();
            return Json(new { data = subscriptions.Select(x => new { x.SubscriptionId, Name = (languageId == 1 ? x.Name : x.ArabicName), x.MaxAccounts, x.MaxOrders }) });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Create(SubscriptionModel model)
        {
            bool result; ModelState.Remove("SubscriptionId");

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (model.SubscriptionId > 0)
                {
                    model.UpdatedAt = DateTime.Now;
                    model.UpdatedBy = user.UserName;
                    result = await _subscriptionRepo.Update(model);
                }
                else
                {
                    model.CreateAt = DateTime.Now;
                    model.UpdatedAt = DateTime.Now;
                    model.CreateBy = user.UserName;
                    result = await _subscriptionRepo.Insert(model);
                }
                return Json(new { status = result, message = result ? "Record has been submitted successfully" : "Error occured please try again" });
            }
            return Json(new { status = false, message = "Invalid input please fill the form" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var subcription = await _subscriptionRepo.GetById(id);
            return View("Create", subcription);
        }

        public async Task<JsonResult> Delete(int id)
        {
            var result = await _subscriptionRepo.Delete(new SubscriptionModel { SubscriptionId = id });
            return Json(new { status = result, message = result ? "Record has been deleted successfully" : "Error occured please try again" });
        }

        public async Task<IActionResult> CompanySubscription()
        {
            ViewBag.Companies = await _dropdownsServices.CompaniesDropdown();
            ViewBag.Subscriptions = await _dropdownsServices.SubscriptionsDropdown("");
            return View();
        }

        public async Task<JsonResult> GetCompanySubscriptions()
        {
            var subscriptions = await _companySubscriptionRepo.GetAll();
            return Json(new { data = subscriptions.Select(x => new { x.CompanySubscriptionId, companyName = (languageId == 1 ? x.Company.Name : x.Company.ArabicName), subscriptionName = (languageId == 1 ? x.Subscription.Name : x.Subscription.ArabicName), StartDate = x.StartDate.ToString("yyyy-MM-dd"), EndDate = x.EndDate.ToString("yyyy-MM-dd") }) });
        }

        [HttpPost]
        public async Task<JsonResult> CreateCompanySubscription(CompanySubscriptionModel model)
        {
            bool result = false; ModelState.Remove("CompanySubscriptionId");

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (model.SubscriptionId > 0)
                {
                    model.UpdatedAt = DateTime.Now;
                    model.UpdatedBy = user.UserName;
                    result = await _companySubscriptionRepo.Update(model);
                }
                else
                {
                    model.CreateAt = DateTime.Now;
                    model.UpdatedAt = DateTime.Now;
                    model.CreateBy = user.UserName;
                    result = await _companySubscriptionRepo.Insert(model);
                }
            }
            return Json(new { status = result, message = result ? "Record has been submitted successfully" : "Invalid input please fill the form" });
        }

        public async Task<IActionResult> EditCompanySubscription(int id)
        {
            var subcription = await _companySubscriptionRepo.GetById(id);
            return View("Create", subcription);
        }
    }
}