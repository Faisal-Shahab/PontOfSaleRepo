using Microsoft.AspNetCore.Mvc;
using PointOfSale.Model;
using POS.DataAccessLayer;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.Controllers
{

    public class OrderController : Controller
    {
        private readonly IDropdownsServices _dropdownsServices;
        private int LanguageId;
        private int CompanyId;
        AppDbContext _appDbContext;
        string user = "test";
        public OrderController(IDropdownsServices dropdownsServices, AppDbContext appDbContext)
        {
            LanguageId = 1;
            CompanyId = 1;
            _dropdownsServices = dropdownsServices;
            _dropdownsServices.LanguageId = LanguageId;
            _dropdownsServices.CompanyId = CompanyId;
            _appDbContext = appDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Sales()
        {

            ViewBag.PaymentTypes = await _dropdownsServices.PaymentTypesDropdown();
            return View();
        }

        public async Task<JsonResult> GetProducts(string value)
        {
            try
            {
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
                var customers = await _dropdownsServices.CustomersDropdown(value);
                return Json(customers);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IActionResult CreateSaleOrder(SaleOrder saleOrder, List<SaleOrderDetails> details)
        {
            using (var trans = _appDbContext.Database.BeginTransaction())
            {
                try
                {
                    int invNumber = _appDbContext.SaleOrder.Max(x => (int?)x.InvNumber) ?? 0; invNumber += 1;
                    saleOrder.CompanyId = CompanyId;
                    saleOrder.InvNumber = invNumber;
                    saleOrder.CreatedAt = DateTime.Now.AddHours(3);
                    saleOrder.CreatedBy = user;
                    _appDbContext.SaleOrder.Add(saleOrder);
                    _appDbContext.SaveChanges();

                    var productIds = details.Select(x => x.ProductId).ToArray();

                    var products = _appDbContext.Products.Where(x => productIds.Contains(x.ProductId)).ToList();

                    foreach (var order in details)
                    {
                        order.OrderId = saleOrder.OrderId;
                        order.CreatedAt = DateTime.Now.AddHours(3);
                        order.CreatedBy = user;
                        _appDbContext.SaleOrderDetails.Add(order);

                        var product = products.FirstOrDefault(p => p.ProductId == order.ProductId);
                        if (product != null)
                        {
                            product.Quantity = product.Quantity - order.Quantity;
                            _appDbContext.Products.Update(product);
                        }
                    }

                    _appDbContext.SaveChanges();

                    var companyDetails = _appDbContext.Companies.Where(x => x.CompanyId == CompanyId).Select(c => new
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
                catch (Exception e)
                {
                    trans.Rollback();
                    return Json(new { status = false, message = "Operation failed" });
                }
            }
        }

        //public IActionResult PrintInvoice(SaleOrderViewModel sale)
        //{
        
        //    return View(new SaleOrderViewModel());
        //}

    }
}