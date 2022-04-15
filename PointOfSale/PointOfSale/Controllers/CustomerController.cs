using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models.Customer;
using POS.DataAccessLayer.Models.Security;
using POS.DataAccessLayer.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        IRepository<CustomerModel> _customerRepo;
        private int languageId;
        IDropdownsServices _dropdownsServices;
        UserManager<User> _userManager;
        public CustomerController(IRepository<CustomerModel> customerRepo, IDropdownsServices dropdownsServices, UserManager<User> userManager)
        {
            _customerRepo = customerRepo;
            languageId = 1;
            _dropdownsServices = dropdownsServices;
            _dropdownsServices.LanguageId = languageId;
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        public async Task<JsonResult> GetCustomers(SearchFilter filter)
        {
            var user = await _userManager.GetUserAsync(User);
            var customers = await _customerRepo.GetAll();
            customers = customers.Where(x => x.CompanyId == user.CompanyId);
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var isNumber = double.TryParse(filter.SearchTerm, out double numericValue);
                if (isNumber)
                {
                    customers = customers.Where(x => x.CustomerId == Convert.ToInt32(filter.SearchTerm));
                }
                else
                {
                    customers = customers.Where(x => x.Name.ToLower().Contains(filter.SearchTerm.ToLower()));
                }
            }
            var total = customers.Count();
            var data = customers.Select(x => new
            {
                x.CustomerId,
                x.Name,
                x.Email,
                x.ContactNo,
                x.Address                
            });
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
        public async Task<JsonResult> Create(CustomerModel model)
        {
            bool result; ModelState.Remove("CustomerId");

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (model.CustomerId > 0)
                {
                    model.UpdatedAt = DateTime.Now;
                    model.UpdatedBy = user.UserName;
                    model.CompanyId = (int)user.CompanyId;
                    result = await _customerRepo.Update(model);
                }
                else
                {
                    model.CreateAt = DateTime.Now;
                    model.UpdatedAt = DateTime.Now;
                    model.CreateBy = user.UserName;
                    model.CompanyId = (int)user.CompanyId;
                    result = await _customerRepo.Insert(model);
                }
                return Json(new { status = result, message = result ? "Record has been submitted successfully" : "Error occured please try again" });
            }
            return Json(new { status = false, message = "Invalid input please fill the form" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);            
            
            var customer = await _customerRepo.GetById(id);
            if (customer.CompanyId == user.CompanyId)
                return View("Create", customer);
            else
                return View("Create", new CustomerModel());
        }

        public async Task<JsonResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            bool result = false;
            var customer = await _customerRepo.GetById(id);
            if (customer.CompanyId == user.CompanyId)
            {
                 result = await _customerRepo.Delete(customer);              
            }
            return Json(new { status = result, message = result ? "Record has been deleted successfully" : "Error occured please try again" });
        }
    }
}