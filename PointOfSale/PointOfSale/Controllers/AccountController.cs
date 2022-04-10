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
using PointOfSale.Utility;
using POS.DataAccessLayer;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models.Security;
using POS.DataAccessLayer.ViewModels;

namespace PointOfSale.Controllers
{
    [Authorize(Roles="Admin")] 
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
            _dropdownsServices.LanguageId = 1;
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
                var user = await _userManager.GetUserAsync(User);
                var query = _appContext.UserView.FromSqlRaw(@$"SELECT u.Id, u.Name, u.UserName, u.Email, u.PhoneNumber, r.Name AS RoleName, ur.RoleId,u.IsActive,u.CompanyId,u.DateCreated From AspNetUsers AS u JOIN AspNetUserRoles AS ur ON ur.UserId = u.Id JOIN AspNetRoles AS r ON r.Id = ur.RoleId")
                                                .Where(x => x.CompanyId == user.CompanyId);
                var users = await query.OrderByDescending(x => x.DateCreated).Skip(filter.Start).Take(filter.PageLength).ToListAsync();

                return Json(new
                {
                    sEcho = filter.Draw,
                    iTotalRecords = query.Count(),
                    iTotalDisplayRecords = query.Count(),
                    aaData = users
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Registration()
        {

            ViewBag.Roles = await _dropdownsServices.RolesDropdown();
            return View();
        }

        //superadmin,admin,company, co
        [HttpPost]
        public async Task<IActionResult> Registration(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    var userModel = new User
                    {
                        Email = model.Email,
                        Name = model.Name,
                        UserName = model.UserName,
                        DateCreated = DateTime.UtcNow.AddHours(3),
                        PhoneNumber = model.PhoneNumber,
                        IsActive = true,
                        CreatedBy = user.UserName,
                        CompanyId = user.CompanyId
                    };

                    var result = await _userManager.CreateAsync(userModel, "temp@2022");

                    if (result.Succeeded)
                    {
                        var currentUser = await _userManager.FindByNameAsync(userModel.UserName);
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
                Role = roleDetails.NormalizedName,
            };

            ViewBag.Roles = await _dropdownsServices.RolesDropdown();
            return View("Registration", model);

        }

        [HttpPost]
        public async Task<IActionResult> UpdateRegistration(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    user.Email = model.Email;
                    user.Name = model.Name;
                    user.UserName = model.UserName;
                    user.DateCreated = DateTime.UtcNow.AddHours(3);
                    user.PhoneNumber = model.PhoneNumber;
                    user.UpdatedBy = user.UserName;

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        var currentUser = await _userManager.FindByNameAsync(user.UserName);
                        await _userManager.AddToRoleAsync(currentUser, model.Role);
                        return Json(new { status = result.Succeeded, message = "Record updated successfully" });
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


        [HttpPost]
        public async Task<JsonResult> ActiveInActiveUser(string userName, bool status)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Json(new { status = false, message = "operation faild, please try again" });
            }
            user.IsActive = status;
            var isSaved = await _userManager.UpdateAsync(user);
            return Json(new { status = isSaved.Succeeded, message = isSaved.Succeeded ? "Status updated successfully" : "operation faild, please try again" });
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

        [AllowAnonymous]
        public IActionResult Login()
        {
            var message = TempData["SuccessMsg"];
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
                if (user == null || !user.IsActive)
                {
                    ModelState.AddModelError("", "invalid username || password");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.Rememberme, lockoutOnFailure: false);

                if (result.Succeeded)
                {

                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin")) { return RedirectToAction("index"); }
                    else if (roles.Contains("User"))
                    {
                        return RedirectToAction("Sales", "Order");
                    }

                    ModelState.AddModelError("", "No Role Assign to you in the system");
                    return View(model);
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
                await _signInManager.SignOutAsync();
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }
        #endregion

        #region ===== Reset Password =====

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgetPassword()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            string message = string.Empty;
            bool isError = false;

            if (string.IsNullOrWhiteSpace(email))
            {
                message = "Please fill the required field"; isError = true;
                return Json(new { status = isError, message = message });
            }
            message = "Password reset link has been sent to your email";
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                if (email.ToLower() == user.Email.ToLower())
                {
                    string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var userIp = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    string resetLink = "<a href='" + Url.Action("PasswordReset", "Account", new { email = email, code = token }, "https") + "'>Reset Password</a>";
                    EmailViewModel emailModel = new EmailViewModel()
                    {

                        To = email,
                        Subject = "Reset Password",
                        MessageBody = @"Dear " + user.UserName + @",<br/>                               
                                    Password reset link has been provided to you, please click on the link to <b>" + resetLink + @"</b>.<br/>                                                                          
                                    Password reset request from : " + userIp + @"<br/><br/>                                    
                                    Best Regards
                                    <br/>
                                    Software Engineering Team."
                    };

                    EmailService.SendEmail(emailModel.Subject, emailModel.To, emailModel.MessageBody);
                }
            }
            return Json(new { status = isError, message = message });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PasswordReset(string code, string email)
        {
            PasswordResetViewModel model = new PasswordResetViewModel();
            model.Token = code;
            model.Email = email;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PasswordReset(PasswordResetViewModel model)
        {
            string message = "";
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null || user.IsActive)
                {
                    var isUpdated = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (isUpdated.Succeeded)
                    {
                        message = "Password reset successfully";
                    }
                    else
                    {
                        ModelState.AddModelError("", "Operation failed, " + isUpdated.Errors.FirstOrDefault().Description);
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email does not match");
                    return View();
                }
                TempData["SuccessMsg"] = message;
                return RedirectToAction("login");
            }
            return View();
        }
        #endregion
    }
}