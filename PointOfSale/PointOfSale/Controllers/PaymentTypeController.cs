using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models.Company;
using POS.DataAccessLayer.Models.Payment;

namespace PointOfSale.Controllers
{
    public class PaymentTypeController : Controller
    {
        IRepository<PaymentTypeModel> _paymentTypeRepo;

        public PaymentTypeController(IRepository<PaymentTypeModel> paymentTypeRepo)
        {
            _paymentTypeRepo = paymentTypeRepo;
        }

        public IActionResult Index() => View();

        public async Task<JsonResult> GetPaymentTypes()
        {
            return Json(new { data = await _paymentTypeRepo.GetAll() });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Create(PaymentTypeModel model)
        {
            bool result; ModelState.Remove("PaymentId");

            if (ModelState.IsValid)
            {
                if (model.PaymentId > 0)
                {
                    result = await _paymentTypeRepo.Update(model);
                }
                else
                {
                    result = await _paymentTypeRepo.Insert(model);
                }
                return Json(new { status = result, message = result ? "Record has been submitted successfully" : "Error occured please try again" });
            }
            return Json(new { status = false, message = "Invalid input please fill the form" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var paymentType = await _paymentTypeRepo.GetById(id);
            return View("Create", paymentType);
        }

        public async Task<JsonResult> Delete(int id)
        {
            return Json(await _paymentTypeRepo.Delete(new PaymentTypeModel { PaymentId = id }));
        }
    }
}