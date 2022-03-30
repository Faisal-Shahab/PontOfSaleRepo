using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PointOfSale.Model;
using POS.DataAccessLayer;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models.Category;
using POS.DataAccessLayer.Models.Subscriptions;

namespace PointOfSale.Controllers
{
    public class CategoryController : Controller
    {
        IRepository<CategoryModel> _categoryRepo;
        IRepository<CategoryDescriptionModel> _categoryDescRepo;
        AppDbContext _context;
        private readonly string user;
        private int languageId;
        private readonly int companyId;
        IDropdownsServices _dropdownsServices;
        public CategoryController(IRepository<CategoryModel> categoryRepo,
            IRepository<CategoryDescriptionModel> categoryDescRepo, AppDbContext context
            , IDropdownsServices dropdownsServices)
        {
            _categoryRepo = categoryRepo;
            _categoryDescRepo = categoryDescRepo;
            _context = context;
            user = "test";
            languageId = 1;
            _dropdownsServices = dropdownsServices;
            _dropdownsServices.LanguageId = 1;
            companyId = 1;
        }

        public IActionResult Index() => View();

        public JsonResult GetCategories()
        {
            return Json(new
            {
                data = _context.Categories.Where(x=>x.CompanyId == companyId).Select(x => new
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
                if (model.CategoryId > 0)
                {
                    var data = _context.Categories.Where(x => x.CompanyId == companyId && x.CategoryId == model.CategoryId).Select(x => new
                    {
                        Category = x,
                        EngData = x.CategoryDescriptions.FirstOrDefault(x => x.LanguageId == 1),
                        ArData = x.CategoryDescriptions.FirstOrDefault(x => x.LanguageId == 2)
                    }).FirstOrDefault();

                   // data.Category.CompanyId = model.CompanyId;
                    data.Category.UpdatedAt = DateTime.UtcNow.AddHours(3);
                    data.Category.UpdatedBy = user;
                    result = await _categoryRepo.Update(data.Category);
                    if (result)
                    {
                        data.EngData.Name = model.Name;
                        data.EngData.UpdatedAt = DateTime.UtcNow.AddHours(3);
                        data.EngData.UpdatedBy = user;
                        result = await _categoryDescRepo.Update(data.EngData);

                        data.ArData.Name = model.ArabicName;
                        data.ArData.UpdatedAt = DateTime.UtcNow.AddHours(3);
                        data.ArData.UpdatedBy = user;
                        result = await _categoryDescRepo.Update(data.ArData);
                    }
                }
                else
                {
                    CategoryModel category = new CategoryModel
                    {
                        CompanyId = companyId,
                        CreateAt = DateTime.UtcNow.AddHours(3),
                        UpdatedAt = DateTime.UtcNow.AddHours(3),
                        CreateBy = user
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
                            CreateBy = user
                        });

                        result = await _categoryDescRepo.Insert(new CategoryDescriptionModel
                        {
                            CategoryId = category.CategoryId,
                            Name = model.ArabicName,
                            LanguageId = 2,
                            CreateAt = DateTime.UtcNow.AddHours(3),
                            UpdatedAt = DateTime.UtcNow.AddHours(3),
                            CreateBy = user
                        });

                    }
                }
                return Json(new { status = result, message = result ? "Record has been submitted successfully" : "Error occured please try again" });
            }
            return Json(new { status = false, message = "Invalid input please fill the form" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Companies = await _dropdownsServices.CompaniesDropdown();
            var category = await _context.Categories.Where(x => x.CompanyId == companyId && x.CategoryId == id).Select(x => new CategoryViewModel
            {
                CategoryId = x.CategoryId,                
                Name = x.CategoryDescriptions.FirstOrDefault(x => x.LanguageId == 1).Name,
                ArabicName = x.CategoryDescriptions.FirstOrDefault(x => x.LanguageId == 2).Name
            }).FirstOrDefaultAsync();
            return View("Create", category);
        }

        public async Task<JsonResult> Delete(int id)
        {
            var result = await _categoryRepo.Delete(new CategoryModel { CategoryId = id });
            return Json(new { status = result, message = result ? "Record has been deleted successfully" : "Error occured please try again" });
        }
    }
}