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
        IRepository<CustomerTransaction> _customerTrans;
        private int languageId;
        IDropdownsServices _dropdownsServices;
        UserManager<User> _userManager;
        System.Globalization.CultureInfo info;
        public CustomerController(IRepository<CustomerModel> customerRepo, IRepository<CustomerTransaction> customerTrans, IDropdownsServices dropdownsServices, UserManager<User> userManager)
        {
            _customerRepo = customerRepo;
            info = System.Globalization.CultureInfo.CurrentCulture;
            languageId = info.TwoLetterISOLanguageName == "ar" ? 2 : 1;
            _dropdownsServices = dropdownsServices;
            _dropdownsServices.LanguageId = languageId;
            _userManager = userManager;
            _customerTrans = customerTrans;
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
                    if (model.Balance > 0)
                    {
                        CustomerTransaction transaction = new CustomerTransaction { Balance = model.Balance };
                        result = await _customerTrans.Insert(transaction);
                    }
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

        public async Task<IActionResult> CustomerPayment()
        {
            var user = await _userManager.GetUserAsync(User);
            var query = await _customerRepo.GetAll();
            ViewBag.Customers = query.Where(x => x.CompanyId == user.CompanyId).ToList();
            return View();
        }

        public async Task<JsonResult> getCustTransactionById(int id)
        {
            var cust = await _customerRepo.GetById(id);
            return Json(cust.Balance);
        }

        public async Task<JsonResult> CreateTransaction(CustomerTransaction model)
        {
            try
            {
                model.SaleOrderId = null;
                var user = await _userManager.GetUserAsync(User);
                model.CompanyId = (int)user.CompanyId;
                model.CreateBy = user.UserName;
                var result = await _customerTrans.Insert(model);
                var cust = await _customerRepo.GetById(model.CustomerId);
                cust.Balance = model.Balance;
                cust.UpdatedBy = user.UserName;
                cust.UpdatedAt = DateTime.Now;
                result = await _customerRepo.Update(cust);
                return Json(new { status = result });
            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }

        }
    }
}