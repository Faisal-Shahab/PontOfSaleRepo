using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PointOfSale.Model;
using POS.DataAccessLayer;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models;
using POS.DataAccessLayer.Models.Security;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        IRepository<Category> _categoryRepo;
        AppDbContext _context;
        private int languageId;
        UserManager<User> _userManager;
        public CategoryController(IRepository<Category> categoryRepo, AppDbContext context,
           UserManager<User> userManager)
        {
            _categoryRepo = categoryRepo;
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
                    x.Name,
                    x.NameAr
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

            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (model.CategoryId > 0)
                    {

                        var data = _context.Categories.Where(x => x.CompanyId == user.CompanyId
                                                          && x.CategoryId == model.CategoryId).FirstOrDefault();

                        data.UpdatedBy = user.UserName;
                        data.Name = model.Name;
                        data.NameAr = model.ArabicName;
                        result = await _categoryRepo.Update(data);
                    }
                    else
                    {
                        Category category = new Category
                        {
                            CompanyId = (int)user.CompanyId,
                            CreateBy = user.UserName,
                            Name = model.Name,
                            NameAr = model.ArabicName
                        };
                        result = await _categoryRepo.Insert(category);
                    }
                    return Json(new { success = result });
                }
                return Json(new { success = false });
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var category = await _context.Categories
                .Where(x => x.CompanyId == user.CompanyId && x.CategoryId == id).Select(x => new CategoryViewModel
                {
                    CategoryId = x.CategoryId,
                    Name = x.Name,
                    ArabicName = x.NameAr
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