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
                var data = await query.OrderByDescending(x => x.Id).Skip(filter.Start).Take(filter.PageLength)
                             .Select(x => new
                             {
                                 OrderId = x.Id,
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
            var order = await _appDbContext.SaleOrder.FirstOrDefaultAsync(x => x.CompanyId == user.CompanyId && x.Id == id);
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
                        order.SaleOrderId = saleOrder.Id;
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
                        orderId = saleOrder.Id,
                        date = DateTime.Now.AddHours(3).ToString("yyyy-MM-dd hh:mm"),
                        qrCode = GenerateQC(invNumber.ToString())
                    };

                    trans.Commit();
                    return Json(new { status = true, message = "Order placed successfully", orderData = orderData });
                }
                catch (Exception e)
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
                        order.PurchaseOrderId = purchaseOrder.Id;
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
                        orderId = purchaseOrder.Id,
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
                var data = await query.OrderByDescending(x => x.Id).Skip(filter.Start).Take(filter.PageLength)
                             .Select(x => new
                             {
                                 OrderId = x.Id,
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

        #region ==== Return Order =====

        public IActionResult ReturnOrders()
        {
            return View();
        }

        public async Task<JsonResult> GetReturnOrders(SearchFilter filter)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var query = _appDbContext.ReturnOrders.Where(x => x.CompanyId == user.CompanyId);
                int totalRows = query.Count();
                var data = await query.OrderByDescending(x => x.Id).Skip(filter.Start).Take(filter.PageLength)
                             .Select(x => new
                             {
                                 x.Id,
                                 x.SaleOrderId,
                                 CustomerName = x.Customer.Name,
                                 x.Total,
                                 OrderDate = x.CreatedAt.ToString("yyyy-MM-dd"),
                                 UpdatedDate = x.UpdatedAt.ToString("yyyy-MM-dd")
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

        public async Task<ActionResult> Return()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.Company = await _appDbContext.Companies.FirstOrDefaultAsync(c => c.CompanyId == user.CompanyId);
            ViewBag.PaymentTypes = await _dropdownsServices.PaymentTypesDropdown();
            return View();
        }

        public async Task<JsonResult> GetOrderDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _appDbContext.SaleOrder.Where(x => x.Id == id && x.CompanyId == user.CompanyId)
                  .Select(o => new
                  {
                      OrderId = o.Id,
                      o.InvNumber,
                      o.Total,
                      o.CompanyId,
                      CustomerName = o.Customer != null ? o.Customer.Name : "",
                      Date = o.CreatedAt.ToString("yyyy-MM-dd"),
                      OrderDetails = o.SaleOrderDetails.Select(od => new
                      {
                          od.ProductId,
                          ProductName = od.Product.Name,
                          od.Quantity,
                          od.SalePrice,
                          od.SubTotal
                      })
                  }).FirstOrDefaultAsync();

            return Json(order);
        }


        public async Task<IActionResult> CreateReturnOrder(ReturnOrder returnOrder, List<ReturnOrderDetail> details)
        {
            using (var trans = _appDbContext.Database.BeginTransaction())
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    int invNumber = _appDbContext.ReturnOrders.Max(x => (int?)x.InvNumber) ?? 0; invNumber += 1;
                    returnOrder.CompanyId = (int)user.CompanyId;
                    returnOrder.InvNumber = invNumber;
                    returnOrder.CreatedBy = user.UserName;
                    _appDbContext.ReturnOrders.Add(returnOrder);
                    _appDbContext.SaveChanges();

                    var productIds = details.Select(x => x.ProductId).ToArray();

                    var products = _appDbContext.Products.Where(x => productIds.Contains(x.ProductId)).ToList();

                    foreach (var order in details)
                    {
                        var product = products.FirstOrDefault(p => p.ProductId == order.ProductId);
                        order.ReturnOrderId = returnOrder.Id;
                        order.CreatedBy = user.UserName;
                        _appDbContext.ReturnOrderDetails.Add(order);


                        if (product != null)
                        {
                            product.Quantity = product.Quantity + order.Quantity;
                            _appDbContext.Products.Update(product);
                        }
                    }

                    _appDbContext.SaveChanges();

                    var orderData = new
                    {
                        invNumber = invNumber,
                        orderId = returnOrder.Id,
                        date = DateTime.Now.AddHours(3).ToString("yyyy-MM-dd hh:mm"),
                        qrCode = GenerateQC(invNumber.ToString())
                    };

                    trans.Commit();
                    return Json(new { status = true, message = "Order placed successfully", orderData = orderData });
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    return Json(new { status = false, message = "Operation failed" });
                }
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