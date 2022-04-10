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
using Microsoft.EntityFrameworkCore;
using PointOfSale.Model;
using POS.DataAccessLayer;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models.Category;
using POS.DataAccessLayer.Models.Security;
using POS.DataAccessLayer.Models.Subscriptions;

namespace PointOfSale.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        IRepository<CategoryModel> _categoryRepo;
        IRepository<CategoryDescriptionModel> _categoryDescRepo;
        AppDbContext _context;
        private int languageId;
        UserManager<User> _userManager;
        public CategoryController(IRepository<CategoryModel> categoryRepo,
            IRepository<CategoryDescriptionModel> categoryDescRepo, AppDbContext context,
           UserManager<User> userManager)
        {
            _categoryRepo = categoryRepo;
            _categoryDescRepo = categoryDescRepo;
            _context = context;
            languageId = 1;
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        public async Task<JsonResult> GetCategories()
        {
            var user = await _userManager.GetUserAsync(User);
            return Json(new
            {
                data = _context.Categories.Where(x => x.CompanyId == user.CompanyId).Select(x => new
                {
                    x.CategoryId,
                    CategoryName = x.CategoryDescriptions.FirstOrDefault(x => x.LanguageId == languageId).Name
                })
            });
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Create(CategoryViewModel model)
        {
            bool result = false; ModelState.Remove("CategoryId");

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (model.CategoryId > 0)
                {

                    var data = _context.Categories.Where(x => x.CompanyId == user.CompanyId && x.CategoryId == model.CategoryId).Select(x => new
                    {
                        Category = x,
                        EngData = x.CategoryDescriptions.FirstOrDefault(x => x.LanguageId == 1),
                        ArData = x.CategoryDescriptions.FirstOrDefault(x => x.LanguageId == 2)
                    }).FirstOrDefault();

                    // data.Category.CompanyId = model.CompanyId;
                    data.Category.UpdatedAt = DateTime.UtcNow.AddHours(3);
                    data.Category.UpdatedBy = user.UserName;
                    result = await _categoryRepo.Update(data.Category);
                    if (result)
                    {
                        data.EngData.Name = model.Name;
                        data.EngData.UpdatedAt = DateTime.UtcNow.AddHours(3);
                        data.EngData.UpdatedBy = user.UserName;
                        result = await _categoryDescRepo.Update(data.EngData);

                        data.ArData.Name = model.ArabicName;
                        data.ArData.UpdatedAt = DateTime.UtcNow.AddHours(3);
                        data.ArData.UpdatedBy = user.UserName;
                        result = await _categoryDescRepo.Update(data.ArData);
                    }
                }
                else
                {
                    CategoryModel category = new CategoryModel
                    {
                        CompanyId = (int)user.CompanyId,
                        CreateAt = DateTime.UtcNow.AddHours(3),
                        UpdatedAt = DateTime.UtcNow.AddHours(3),
                        CreateBy = user.UserName
                    };
                    var success = await _categoryRepo.Insert(category);
                    if (success)
                    {
                        result = await _categoryDescRepo.Insert(new CategoryDescriptionModel
                        {
                            CategoryId = category.CategoryId,
                            Name = model.Name,
                            LanguageId = 1,
                            CreateAt = DateTime.UtcNow.AddHours(3),
                            UpdatedAt = DateTime.UtcNow.AddHours(3),
                            CreateBy = user.UserName
                        });

                        result = await _categoryDescRepo.Insert(new CategoryDescriptionModel
                        {
                            CategoryId = category.CategoryId,
                            Name = model.ArabicName,
                            LanguageId = 2,
                            CreateAt = DateTime.UtcNow.AddHours(3),
                            UpdatedAt = DateTime.UtcNow.AddHours(3),
                            CreateBy = user.UserName
                        });

                    }
                }
                return Json(new { status = result, message = result ? "Record has been submitted successfully" : "Error occured please try again" });
            }
            return Json(new { status = false, message = "Invalid input please fill the form" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var category = await _context.Categories.Where(x => x.CompanyId == user.CompanyId && x.CategoryId == id).Select(x => new CategoryViewModel
            {
                CategoryId = x.CategoryId,
                Name = x.CategoryDescriptions.FirstOrDefault(x => x.LanguageId == 1).Name,
                ArabicName = x.CategoryDescriptions.FirstOrDefault(x => x.LanguageId == 2).Name
            }).FirstOrDefaultAsync();
            return View("Create", category);
        }

        public async Task<JsonResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var cateories = await _categoryRepo.GetAll();
            var cat = cateories.FirstOrDefault(x => x.CompanyId == user.CompanyId && x.CategoryId == id);
            var result = await _categoryRepo.Delete(cat);
            return Json(new { status = result, message = result ? "Record has been deleted successfully" : "Error occured please try again" });
        }
    }
}