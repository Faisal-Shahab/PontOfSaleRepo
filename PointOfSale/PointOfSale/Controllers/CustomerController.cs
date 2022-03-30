using Microsoft.AspNetCore.Mvc;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models.Customer;
using POS.DataAccessLayer.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.Controllers
{
    public class CustomerController : Controller
    {
        IRepository<CustomerModel> _customerRepo;
        private readonly string user;
        private readonly int companyId;
        private int languageId;        
        IDropdownsServices _dropdownsServices;
        public CustomerController(IRepository<CustomerModel> customerRepo, IDropdownsServices dropdownsServices)
        {
            _customerRepo = customerRepo;
            user = "test";
            languageId = 1;
            _dropdownsServices = dropdownsServices;
            _dropdownsServices.LanguageId = languageId;
            companyId = 1;
        }

        public IActionResult Index() => View();

        public async Task<JsonResult> GetCustomers(SearchFilter filter)
        {
            var customers = await _customerRepo.GetAll();
            customers = customers.Where(x => x.CompanyId == companyId);
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
                x.ContactNo,
                x.Address,
                CompanyName = (languageId == 1) ? x.Company.Name : x.Company.ArabicName
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
                if (model.CustomerId > 0)
                {
                    model.UpdatedAt = DateTime.Now;
                    model.UpdatedBy = user;
                    model.CompanyId = companyId;
                    result = await _customerRepo.Update(model);
                }
                else
                {
                    model.CreateAt = DateTime.Now;
                    model.UpdatedAt = DateTime.Now;
                    model.CreateBy = user;
                    model.CompanyId = companyId;
                    result = await _customerRepo.Insert(model);
                }
                return Json(new { status = result, message = result ? "Record has been submitted successfully" : "Error occured please try again" });
            }
            return Json(new { status = false, message = "Invalid input please fill the form" });
        }

        public async Task<IActionResult> Edit(int id)
        {           
            var customer = await _customerRepo.GetById(id);
            return View("Create", customer);
        }

        public async Task<JsonResult> Delete(int id)
        {
            var result = await _customerRepo.Delete(new CustomerModel { CustomerId = id });
            return Json(new { status = result, message = result ? "Record has been deleted successfully" : "Error occured please try again" });
        }
    }
}