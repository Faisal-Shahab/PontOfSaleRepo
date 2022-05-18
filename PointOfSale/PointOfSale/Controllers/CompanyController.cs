using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Model;
using PointOfSale.Utility;
using POS.DataAccessLayer;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models.Company;
using POS.DataAccessLayer.Models.Security;

namespace PointOfSale.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CompanyController : Controller
    {
        IRepository<CompanyModel> _companyRepo;
        private readonly IHostingEnvironment _hostingEnvironment;
        UserManager<User> _userManager;
        RoleManager<IdentityRole> _roleManager;
        AppDbContext _appDbContext;
        int language = 1;
        System.Globalization.CultureInfo info;
        public CompanyController(IRepository<CompanyModel> companyRepo,
                                    IHostingEnvironment hostingEnvironment,
                                     UserManager<User> userManager,
                            RoleManager<IdentityRole> roleManager,
                              AppDbContext appDbContext)
        {
            _companyRepo = companyRepo;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _roleManager = roleManager;
            _appDbContext = appDbContext;
            info = System.Globalization.CultureInfo.CurrentCulture;
            language = info.TwoLetterISOLanguageName == "ar" ? 2 : 1;

        }

        public IActionResult Index() => View();

        public async Task<JsonResult> GetCompanies()
        {
            var user = await _userManager.GetUserAsync(User);
            var companies = await _companyRepo.GetAll();
            if (user.CompanyId > 0)
            {
                companies = companies.Where(x => x.CompanyId == user.CompanyId);
            }

            return Json(new
            {
                data = companies.Select(x => new
                {
                    x.CompanyId,
                    Name = language == 1 ? x.Name : x.ArabicName,
                    x.Email,
                    x.ContactNo,
                    x.FaxNo
                }).ToList()
            });
        }

        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.CompanyId > 0)
            {
                return RedirectToAction("index");
            }
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Create(CompanyViewModel viewModel, IFormFile avatar)
        {
            try
            {
                bool result;
                ModelState.Remove("CompanyId");
                if (viewModel.CompanyId == 0) ModelState.Remove("CurrentPassword");

                if (ModelState.IsValid)
                {
                    var loginUser = await _userManager.GetUserAsync(User);
                    var model = PosAutoMaper.Map<CompanyModel, CompanyViewModel>(new CompanyModel(), viewModel);

                    if (viewModel.Image != "none") { model.Logo = UploadImage(viewModel.Image); }
                    if (model.CompanyId > 0)
                    {                        
                        model.UpdatedAt = DateTime.UtcNow.AddHours(3);
                        model.UpdatedBy = loginUser.UserName;
                        result = await _companyRepo.Update(model);

                        if (result)
                        {
                            var user = await _userManager.FindByEmailAsync(viewModel.UserEmail);
                            user.Email = model.Email;
                            user.Name = model.Name;
                            user.UpdatedBy = loginUser.UserName;
                            user.DateUpdated = DateTime.Now.AddHours(3);
                            var status = await _userManager.UpdateAsync(user);

                            if (status.Succeeded && !string.IsNullOrEmpty(viewModel.Password))
                            {
                                var updatedUser = await _userManager.FindByEmailAsync(viewModel.Email);
                                var res = await _userManager.ChangePasswordAsync(updatedUser, viewModel.CurrentPassword, viewModel.Password);
                                return Json(new { status = res.Succeeded, message = res.Succeeded ? "Record has been submitted successfully" : "Password does not match" });
                            }
                        }

                    }
                    else
                    {
                        model.Printer = "Thermal";
                        model.CreateAt = DateTime.UtcNow.AddHours(3);
                        model.UpdatedAt = DateTime.UtcNow.AddHours(3);
                        _appDbContext.Companies.Add(model);
                        result = _appDbContext.SaveChanges() > 0;
                        if (result)
                        {
                            var isReg = await _userManager.CreateAsync(new User { Email = model.Email, Name = model.Name, UserName = model.Email, CompanyId = model.CompanyId, IsActive = true }, viewModel.Password);
                            if (isReg.Succeeded)
                            {
                                var currentUser = await _userManager.FindByEmailAsync(model.Email);
                                await _userManager.AddToRoleAsync(currentUser, "ADMIN");
                                result = true;
                            }
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
            CompanyModel company = new CompanyModel();
            if (user.CompanyId == null)
            {
                company = await _companyRepo.GetById(id);
            }
            else
            {
                company = await _companyRepo.GetById((int)user.CompanyId);
            }
            var model = PosAutoMaper.Map<CompanyViewModel, CompanyModel>(new CompanyViewModel(), company);
            model.UserEmail = company.Email;
            return View("Create", model);
        }

        private string UploadImage(string image)
        {

            var imageStr = image.Split("base64,");

            byte[] bytes = Convert.FromBase64String(imageStr[1]);

            MemoryStream stream = new MemoryStream(bytes);
            IFormFile file = new FormFile(stream, 0, bytes.Length, "company_logo", "company_logo." + imageStr[0].Split("/")[1].Replace(";", ""));
            //
            var _folder = "CompanyLogo";
            var uniqueFileName = GetUniqueFileName(file.FileName);
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, _folder);
            var filePath = Path.Combine(uploads, uniqueFileName);
            file.CopyTo(new FileStream(filePath, FileMode.Create));
            stream.Dispose();
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