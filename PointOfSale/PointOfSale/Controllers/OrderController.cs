﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Model;
using POS.DataAccessLayer;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models;
using POS.DataAccessLayer.Models.Order;
using POS.DataAccessLayer.Models.Security;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class OrderController : Controller
    {
        private readonly IDropdownsServices _dropdownsServices;
        private int LanguageId;

        AppDbContext _appDbContext;
        UserManager<User> _userManager;
        public OrderController(IDropdownsServices dropdownsServices, AppDbContext appDbContext,
             UserManager<User> userManager)
        {
            LanguageId = 1;
            _dropdownsServices = dropdownsServices;
            _dropdownsServices.LanguageId = LanguageId;
            _appDbContext = appDbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region ==== Sales ==== 
        public async Task<IActionResult> Sales()
        {
            ViewBag.PaymentTypes = await _dropdownsServices.PaymentTypesDropdown();
            return View();
        }

        public async Task<JsonResult> GetProducts(string value)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                _dropdownsServices.CompanyId = (int)user.CompanyId;
                var products = await _dropdownsServices.ProductsDropdown(value);
                return Json(products);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<JsonResult> GetCustomers(string value)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                _dropdownsServices.CompanyId = (int)user.CompanyId;
                var customers = await _dropdownsServices.CustomersDropdown(value);
                return Json(customers);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<IActionResult> CreateSaleOrder(SaleOrder saleOrder, List<SaleOrderDetails> details)
        {
            using (var trans = _appDbContext.Database.BeginTransaction())
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    int invNumber = _appDbContext.SaleOrder.Max(x => (int?)x.InvNumber) ?? 0; invNumber += 1;
                    saleOrder.CompanyId = (int)user.CompanyId;
                    saleOrder.InvNumber = invNumber;
                    saleOrder.CreatedAt = DateTime.Now.AddHours(3);
                    saleOrder.CreatedBy = user.UserName;
                    _appDbContext.SaleOrder.Add(saleOrder);
                    _appDbContext.SaveChanges();

                    var productIds = details.Select(x => x.ProductId).ToArray();

                    var products = _appDbContext.Products.Where(x => productIds.Contains(x.ProductId)).ToList();

                    foreach (var order in details)
                    {
                        order.OrderId = saleOrder.OrderId;
                        order.CreatedAt = DateTime.Now.AddHours(3);
                        order.CreatedBy = user.UserName;
                        _appDbContext.SaleOrderDetails.Add(order);

                        var product = products.FirstOrDefault(p => p.ProductId == order.ProductId);
                        if (product != null)
                        {
                            product.Quantity = product.Quantity - order.Quantity;
                            _appDbContext.Products.Update(product);
                        }
                    }

                    _appDbContext.SaveChanges();

                    var companyDetails = _appDbContext.Companies.Where(x => x.CompanyId == user.CompanyId).Select(c => new
                    {
                        companyName = LanguageId == 1 ? c.Name : c.ArabicName,
                        c.TaxNumber,
                        c.CrNumber,
                        c.ThankyouNote,
                        invNumber = invNumber
                    }).FirstOrDefault();

                    trans.Commit();
                    return Json(new { status = true, message = "Order places successfully", companyDetails = companyDetails });
                }
                catch (Exception)
                {
                    trans.Rollback();
                    return Json(new { status = false, message = "Operation failed" });
                }
            }
        }

        #endregion


        #region ==== Purchase ==== 

        public async Task<IActionResult> Purchase()
        {
            ViewBag.PaymentTypes = await _dropdownsServices.PaymentTypesDropdown();
            return View();
        }

        public async Task<JsonResult> GetSuppliers(string value)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                _dropdownsServices.CompanyId = (int)user.CompanyId;
                var suppliers = await _dropdownsServices.SuppliersDropdown(value);
                return Json(suppliers);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<IActionResult> CreatePurchaseOrder(PurchaseOrder purchaseOrder, List<PurchaseOrderDetails> details)
        {
            using (var trans = _appDbContext.Database.BeginTransaction())
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    int invNumber = _appDbContext.PurchaseOrders.Max(x => (int?)x.InvNumber) ?? 0; invNumber += 1;
                    purchaseOrder.CompanyId = (int)user.CompanyId;
                    purchaseOrder.InvNumber = invNumber;
                    purchaseOrder.CreatedAt = DateTime.Now.AddHours(3);
                    purchaseOrder.CreatedBy = user.UserName;
                    _appDbContext.PurchaseOrders.Add(purchaseOrder);
                    _appDbContext.SaveChanges();

                    var productIds = details.Select(x => x.ProductId).ToArray();

                    var products = _appDbContext.Products.Where(x => productIds.Contains(x.ProductId)).ToList();

                    foreach (var order in details)
                    {
                        order.OrderId = purchaseOrder.OrderId;
                        order.CreatedAt = DateTime.Now.AddHours(3);
                        order.CreatedBy = user.UserName;
                        _appDbContext.PurchaseOrderDetails.Add(order);

                        var product = products.FirstOrDefault(p => p.ProductId == order.ProductId);
                        if (product != null)
                        {
                            product.Quantity = product.Quantity + order.Quantity;
                            _appDbContext.Products.Update(product);
                        }
                    }

                    _appDbContext.SaveChanges();

                    var companyDetails = _appDbContext.Companies.Where(x => x.CompanyId == user.CompanyId).Select(c => new
                    {
                        companyName = LanguageId == 1 ? c.Name : c.ArabicName,
                        c.TaxNumber,
                        c.CrNumber,
                        c.ThankyouNote,
                        invNumber = invNumber

                    }).FirstOrDefault();

                    trans.Commit();
                    return Json(new { status = true, message = "Order has been placed successfully", companyDetails = companyDetails });
                }
                catch (Exception)
                {
                    trans.Rollback();
                    return Json(new { status = false, message = "Operation failed" });
                }
            }
        }

        #endregion     
    }
}