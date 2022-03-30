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

namespace PointOfSale.Controllers
{
    public class CompanyController : Controller
    {
        IRepository<CompanyModel> _companyRepo;
        private readonly IHostingEnvironment _hostingEnvironment;

        public CompanyController(IRepository<CompanyModel> companyRepo,
                                    IHostingEnvironment hostingEnvironment)
        {
            _companyRepo = companyRepo;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index() => View();

        public async Task<JsonResult> GetCompanies()
        {
            return Json(new { data = await _companyRepo.GetAll() });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Create(CompanyModel model, IFormFile file)
        {
            bool result; ModelState.Remove("CompanyId");

            if (ModelState.IsValid)
            {
                if (file != null) { model.Logo = UploadImage(file); }
                if (model.CompanyId > 0)
                {
                    model.UpdatedAt = DateTime.UtcNow.AddHours(3); ;
                    result = await _companyRepo.Update(model);
                }
                else
                {
                    model.CreateAt = DateTime.UtcNow.AddHours(3);
                    model.UpdatedAt = DateTime.UtcNow.AddHours(3);
                    result = await _companyRepo.Insert(model);
                }
                return Json(new { status = result, message = result ? "Record has been submitted successfully" : "Error occured please try again" });
            }
            return Json(new { status = false, message = "Invalid input please fill the form" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var company = await _companyRepo.GetById(id);
            return View("Create", company);
        }        

        private string UploadImage(IFormFile file)
        {
            var _folder = "CompanyLogo";
            var uniqueFileName = GetUniqueFileName(file.FileName);
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, _folder);
            var filePath = Path.Combine(uploads, uniqueFileName);
            file.CopyTo(new FileStream(filePath, FileMode.Create));
            file = null;
            return uniqueFileName;
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }

    }
}