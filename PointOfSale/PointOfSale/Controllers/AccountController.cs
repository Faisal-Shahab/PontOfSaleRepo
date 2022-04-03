using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PointOfSale.Model;
using PointOfSale.Model.Account;
using POS.DataAccessLayer;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models.Security;
using POS.DataAccessLayer.ViewModels;

namespace PointOfSale.Controllers
{
    public class AccountController : Controller
    {
        AppDbContext _appContext;
        UserManager<User> _userManager;
        RoleManager<IdentityRole> _roleManager;
        IDropdownsServices _dropdownsServices;
        SignInManager<User> _signInManager;
        public AccountController(AppDbContext appContext,
                            UserManager<User> userManager,
                            RoleManager<IdentityRole> roleManager,
                            IDropdownsServices dropdownsServices,
                            SignInManager<User> signInManager)
        {
            _appContext = appContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _dropdownsServices = dropdownsServices;
            _signInManager = signInManager;
        }


        #region =========== User Reg =========

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAccounts(SearchFilter filter)
        {
            try
            {
                var roles = await _appContext.Roles.ToListAsync();
                var query = _userManager.Users.Where(x => x.CompanyId == 1).OrderByDescending(x => x.DateCreated);

                var users = await query.Skip(filter.Start).Take(filter.PageLength).ToListAsync();

                var usersList = users.Select(x => new
                {
                    x.UserName,
                    Role = roles.FirstOrDefault(r => r.Id == x.Roles.FirstOrDefault().RoleId).Name,
                    x.PhoneNumber,
                    x.Email,
                    x.IsActive
                });

                return Json(new
                {
                    sEcho = filter.Draw,
                    iTotalRecords = usersList.Count(),
                    iTotalDisplayRecords = usersList.Count(),
                    aaData = usersList
                });
            }
            catch (Exception e)
            {

                throw;
            }

        }

        [HttpGet]
        public async Task<IActionResult> Registration()
        {
            ViewBag.Companies = await _dropdownsServices.CompaniesDropdown();
            ViewBag.Roles = await _dropdownsServices.RolesDropdown();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var user = new User
                    {
                        Email = model.Email,
                        Name = model.Name,
                        UserName = model.UserName,
                        DateCreated = DateTime.UtcNow.AddHours(3),
                        PhoneNumber = model.PhoneNumber
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var currentUser = await _userManager.FindByNameAsync(user.UserName);
                        await _userManager.AddToRoleAsync(currentUser, model.Role);
                        return Json(new { status = result.Succeeded, message = "User registered successfully" });
                    }
                    return Json(new { status = result.Succeeded, message = "Operation falied" });
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = "Operation falied" });
                }
            }

            return Json(new { status = false, message = "Please fill the required fields" });
        }

        // user role should be admin for this action
        public async Task<IActionResult> EditRegistration(string userName)
        {
            var user = new User();
            if (!string.IsNullOrWhiteSpace(userName))
            {
                user = await _userManager.FindByNameAsync(userName);
            }
            else
            {
                user = await _userManager.GetUserAsync(User);
            }
            var roleName = await _userManager.GetRolesAsync(user);
            var roleDetails = await _roleManager.FindByNameAsync(roleName[0]);
            var model = new RegisterViewModel
            {
                Name = user.Name,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                //Role = roleDetails.NormalizedName
            };

            ViewBag.Companies = await _dropdownsServices.CompaniesDropdown();
            ViewBag.Roles = await _dropdownsServices.RolesDropdown();
            return View(model);

        }

        #endregion

        #region ============= Role CRUD ==================
        public IActionResult Roles()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Json(new { data = roles });
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CreateRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(model.RoleId))
                {
                    var role = await _roleManager.FindByIdAsync(model.RoleId);
                    if (ModelState.IsValid)
                    {
                        role.Name = model.RoleName;
                        var isUpdate = await _roleManager.UpdateAsync(role);
                        return Json(new { status = isUpdate.Succeeded, message = isUpdate.Succeeded ? "Role has been updated successfully" : isUpdate.Errors.FirstOrDefault().Description });
                    }
                }
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                var result = await _roleManager.CreateAsync(identityRole);
                if (result.Succeeded) { return Json(new { status = result, message = result.Succeeded ? "Record has been submitted successfully" : result.Errors.FirstOrDefault().Description }); }
            }
            return Json(new { status = false, message = "Error : Please validate the inputs & try again !" });
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) { role = new IdentityRole(); ModelState.Clear(); }
            var model = new RoleViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name
            };
            return View("index", model);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteRole(string data)
        {
            bool isDeleted;
            var role = await _roleManager.FindByIdAsync(data);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
                isDeleted = true;
            }
            else
            {
                isDeleted = false;
            }
            return Json(isDeleted);
        }

        #endregion

        #region === Login ====

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    //await LogUser(model.Username, false);

                    ModelState.AddModelError("", "invalid username || password");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.Rememberme, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // await LogUser(model.Username, true);

                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin")) { return RedirectToAction("index", "Account"); }
                //    else if (roles.Contains("Company")) { return RedirectToAction("index", "pilgrams"); }
                    else
                    {
                        return RedirectToAction("Sales", "Order");
                        //try
                        //{
                        //    var roleIds = await _userService.GetRoleIds(user.Id);

                        //    var menuList = await _userService.GetMenuItems(roleIds);
                        //    if (menuList.Count > 0)
                        //    {
                        //        var defaultMenu = menuList.FirstOrDefault(m => m.IsSidebarMenu);
                        //        if (defaultMenu != null)
                        //        {
                        //            var menu = defaultMenu.Link.TrimStart('/').Split('/');
                        //            return RedirectToAction(menu[1], menu[0]);
                        //        }
                        //    }
                        //}
                        //catch (Exception ex)
                        //{

                        //}

                        //ModelState.AddModelError("", "No Role Assign to you in the system");
                        //return View(model);
                    }

                }
                ModelState.AddModelError("", "invalid username || password");
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "invalid username || password");
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> LogOff()
        {
            if (User.Identity.IsAuthenticated)
            {

                var user = await _userManager.GetUserAsync(User);              

                await _signInManager.SignOutAsync();
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }
        #endregion
    }
}