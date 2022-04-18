using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PointOfSale.Model;
using POS.DataAccessLayer;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models;
using POS.DataAccessLayer.Models.Order;
using POS.DataAccessLayer.Models.Security;
using POS.DataAccessLayer.ViewModels;
using QRCoder;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

        public async Task<JsonResult> GetSaleOrders(SearchFilter filter)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var query = _appDbContext.SaleOrder.Where(x => x.CompanyId == user.CompanyId);
                int totalRows = query.Count();
                var data = await query.OrderByDescending(x => x.OrderId).Skip(filter.Start).Take(filter.PageLength)
                             .Select(x => new
                             {
                                 x.OrderId,
                                 CustomerName = x.Customer.Name,
                                 x.Total,
                                 OrderDate = x.CreatedAt.ToString("yyyy-MM-dd"),
                                 UpdatedDate = x.UpdatedAt.HasValue ? x.UpdatedAt.Value.ToString("yyyy-MM-dd") : ""
                             }).ToListAsync();

                return Json(new
                {
                    sEcho = filter.Draw,
                    iTotalRecords = totalRows,
                    iTotalDisplayRecords = totalRows,
                    aaData = data
                });
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<JsonResult> DeleteSaleOrders(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _appDbContext.SaleOrder.FirstOrDefaultAsync(x => x.CompanyId == user.CompanyId && x.OrderId == id);
            _appDbContext.SaleOrder.Remove(order);
            return Json(new { status = await _appDbContext.SaveChangesAsync() > 0 });
        }

        #region ==== Sales ==== 
        public async Task<IActionResult> Sales()
        {
            var user = await _userManager.GetUserAsync(User);

            ViewBag.Company = await _appDbContext.Companies.FirstOrDefaultAsync(c => c.CompanyId == user.CompanyId);
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
                        var product = products.FirstOrDefault(p => p.ProductId == order.ProductId);
                        order.OrderId = saleOrder.OrderId;
                        order.CreatedAt = DateTime.Now.AddHours(3);
                        order.CreatedBy = user.UserName;
                        order.CostPrice = product.CostPrice;
                        _appDbContext.SaleOrderDetails.Add(order);


                        if (product != null)
                        {
                            product.Quantity = product.Quantity - order.Quantity;
                            _appDbContext.Products.Update(product);
                        }
                    }

                    _appDbContext.SaveChanges();

                    var orderData = new
                    {
                        invNumber = invNumber,
                        orderId = saleOrder.OrderId,
                        date = DateTime.Now.AddHours(3).ToString("yyyy-MM-dd hh:mm"),
                        qrCode = GenerateQC(invNumber.ToString())
                    };

                    trans.Commit();
                    return Json(new { status = true, message = "Order places successfully", orderData = orderData });
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    return Json(new { status = false, message = "Operation failed" });
                }
            }
        }

        public async Task<JsonResult> GetOrderDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _appDbContext.SaleOrder.Where(x => x.CompanyId == user.CompanyId && x.OrderId == id)
                           .Select(x => new
                           {
                               x.OrderId,
                               x.InvNumber,
                               x.Total,
                               Date = x.CreatedAt.ToString("yyyy-MM-dd"),
                               CustomerName = x.Customer.Name,
                               CustomerEmail = x.Customer.Email,
                               CompanyLogo = x.Company.Logo,
                               CompanyName = x.Company.Name,
                               CompanyCrNumber = x.Company.CrNumber,
                               CompanyTaxNumber = x.Company.TaxNumber,
                               Details = x.SaleOrderDetails,
                               //details = x.SaleOrderDetails.Select(d => new
                               //{
                               //    productName = d.Product.Name,
                               //    d.SalePrice,
                               //    d.Quantity,
                               //    d.SubTotal
                               //})
                           }).ToListAsync();
            return Json(order);
        }
        #endregion

        #region ==== Purchase ==== 

        public async Task<IActionResult> Purchase()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.Company = await _appDbContext.Companies.FirstOrDefaultAsync(c => c.CompanyId == user.CompanyId);
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

                    var purchaseDetails = new
                    {
                        orderId = purchaseOrder.OrderId,
                        invNumber = invNumber,
                        date = DateTime.Now.AddHours(3).ToString("yyyy-MM-dd hh:mm"),
                        qrCode = GenerateQC(invNumber.ToString())
                    };

                    trans.Commit();
                    return Json(new { status = true, message = "Order has been placed successfully", purchaseDetails = purchaseDetails });
                }
                catch (Exception)
                {
                    trans.Rollback();
                    return Json(new { status = false, message = "Operation failed" });
                }
            }
        }

        public IActionResult PurchaseOrders()
        {
            return View();
        }

        public async Task<JsonResult> GetPurchaseOrders(SearchFilter filter)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var query = _appDbContext.PurchaseOrders.Where(x => x.CompanyId == user.CompanyId);
                int totalRows = query.Count();
                var data = await query.OrderByDescending(x => x.OrderId).Skip(filter.Start).Take(filter.PageLength)
                             .Select(x => new
                             {
                                 x.OrderId,
                                 supplierName = x.Supplier.Name,
                                 x.Total,
                                 OrderDate = x.CreatedAt.ToString("yyyy-MM-dd"),
                                 UpdatedDate = x.UpdatedAt.HasValue ? x.UpdatedAt.Value.ToString("yyyy-MM-dd") : ""
                             }).ToListAsync();

                return Json(new
                {
                    sEcho = filter.Draw,
                    iTotalRecords = totalRows,
                    iTotalDisplayRecords = totalRows,
                    aaData = data
                });
            }
            catch (Exception e)
            {

                throw;
            }
        }

        #endregion

        private string GenerateQC(string value)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData QrCodeInfo = qrGenerator.CreateQrCode(value, QRCodeGenerator.ECCLevel.Q);
                QRCode QrCode = new QRCode(QrCodeInfo);
                using (Bitmap bitMap = QrCode.GetGraphic(20))
                {
                    bitMap.Save(ms, ImageFormat.Png);
                    value = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }

            return value;
        }
    }
}