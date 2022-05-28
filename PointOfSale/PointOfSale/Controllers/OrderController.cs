using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PointOfSale.Model;
using PointOfSale.Utility;
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
        System.Globalization.CultureInfo info;
        AppDbContext _appDbContext;
        UserManager<User> _userManager;
        public OrderController(IDropdownsServices dropdownsServices, AppDbContext appDbContext,
             UserManager<User> userManager)
        {
            info = System.Globalization.CultureInfo.CurrentCulture;
            LanguageId = info.TwoLetterISOLanguageName == "ar" ? 2 : 1;
            _dropdownsServices = dropdownsServices;
            _dropdownsServices.LanguageId = LanguageId;
            _appDbContext = appDbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            ViewBag.Company = await _appDbContext.Companies.FirstOrDefaultAsync(c => c.CompanyId == user.CompanyId);
            return View();
        }

        public async Task<JsonResult> GetSaleOrders(SearchFilter filter)
        {
            var fromDate = Convert.ToDateTime($"{filter.FromDate} 00:00:00");
            var toDate = Convert.ToDateTime($"{filter.ToDate} 23:59:59");
            try
            {
                var user = await _userManager.GetUserAsync(User);

                long.TryParse(filter.SearchTerm, out long orderId);

                var query = _appDbContext.SaleOrder.Where(x => x.CompanyId == user.CompanyId && x.CreatedAt >= fromDate && x.CreatedAt <= toDate);

                if (orderId > 0)
                {
                    query = query.Where(x => x.Id == orderId);
                }

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

        public async Task<JsonResult> GetSaleOrderDetails(int id)
        {
            var data = await _appDbContext.SaleOrderDetails.Where(x => x.SaleOrderId == id)
                          .Select(x => new
                          {
                              ProductName = x.Product.Name,
                              x.CostPrice,
                              x.SalePrice,
                              x.Discount,
                              x.Quantity,
                              x.SubTotal,
                          }).ToListAsync();
            return Json(new { data = data });
        }

        public async Task<IActionResult> ExportSaleOrders(SearchFilter filter)
        {
            var fromDate = Convert.ToDateTime($"{filter.FromDate} 00:00:00");
            var toDate = Convert.ToDateTime($"{filter.ToDate} 23:59:59");

            try
            {
                var user = await _userManager.GetUserAsync(User);

                long.TryParse(filter.SearchTerm, out long orderId);

                var query = _appDbContext.SaleOrder.Where(x => x.CompanyId == user.CompanyId
                                         && x.CreatedAt >= fromDate && x.CreatedAt <= toDate);

                if (orderId > 0)
                {
                    query = query.Where(x => x.Id == orderId);
                }

                var fileContents = ExportExcelReport.GetExcelFile(query.Select(p =>
                    new
                    {
                        OrderId = p.Id,
                        CustomerName = p.Customer.Name,
                        p.Total,
                        OrderDate = p.CreatedAt.ToString("yyyy-MM-dd")
                    }).ToList());
                return File(
                    fileContents: fileContents,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: $"Sale_Orders_{filter.FromDate}_To_{filter.ToDate}.xlsx"
                );
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        public async Task<JsonResult> PrintSaleOrderDetails(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var order = _appDbContext.SaleOrder.Where(x => x.CompanyId == user.CompanyId && x.Id == id)
                                               .Select(x => new
                                               {
                                                   x.InvNumber,
                                                   OrderId = x.Id,
                                                   Date = x.CreatedAt.ToString("yyyy-MMM-dd"),
                                                   details = x.SaleOrderDetails.Select(od => new
                                                   {
                                                       ProductName = od.Product.Name,
                                                       od.SalePrice,
                                                       od.Quantity,
                                                       Subtotal = od.SubTotal
                                                   }).ToList()
                                               }).ToList().Select(x => new
                                               {
                                                   qrCode = GenerateQC(x.InvNumber.ToString()),
                                                   x.InvNumber,
                                                   OrderId = x.OrderId,
                                                   Date = x.Date,
                                                   x.details
                                               }).FirstOrDefault();

                return Json(order);
            }
            catch (Exception e)
            {
                throw e;
            }
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

        public async Task<IActionResult> PurchaseOrders()
        {
            var user = await _userManager.GetUserAsync(User);

            ViewBag.Company = await _appDbContext.Companies.FirstOrDefaultAsync(c => c.CompanyId == user.CompanyId);
            return View();
        }

        public async Task<JsonResult> GetPurchaseOrders(SearchFilter filter)
        {
            var fromDate = Convert.ToDateTime($"{filter.FromDate} 00:00:00");
            var toDate = Convert.ToDateTime($"{filter.ToDate} 23:59:59");
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var query = _appDbContext.PurchaseOrders.Where(x => x.CompanyId == user.CompanyId && x.CreatedAt >= fromDate && x.CreatedAt <= toDate);
                int totalRows = query.Count();
                var data = await query.OrderByDescending(x => x.Id).Skip(filter.Start).Take(filter.PageLength)
                             .Select(x => new
                             {
                                 OrderId = x.Id,
                                 supplierName = x.Supplier.Name,
                                 Balance = x.Supplier.Balance,
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

        public async Task<IActionResult> ExportPurchaseOrders(SearchFilter filter)
        {
            var fromDate = Convert.ToDateTime($"{filter.FromDate} 00:00:00");
            var toDate = Convert.ToDateTime($"{filter.ToDate} 23:59:59");

            try
            {
                var user = await _userManager.GetUserAsync(User);

                long.TryParse(filter.SearchTerm, out long orderId);

                var query = _appDbContext.PurchaseOrders.Where(x => x.CompanyId == user.CompanyId
                                         && x.CreatedAt >= fromDate && x.CreatedAt <= toDate);

                if (orderId > 0)
                {
                    query = query.Where(x => x.Id == orderId);
                }

                var fileContents = ExportExcelReport.GetExcelFile(query.Select(p =>
                    new
                    {
                        OrderId = p.Id,
                        SupplierName = p.Supplier.Name,
                        p.Total,
                        OrderDate = p.CreatedAt.ToString("yyyy-MM-dd")
                    }).ToList());
                return File(
                    fileContents: fileContents,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: $"Purchase_Orders_{filter.FromDate}_To_{filter.ToDate}.xlsx"
                );
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        public async Task<JsonResult> GetPurchaseOrderDetail(int id)
        {
            try
            {
                var data = await _appDbContext.PurchaseOrderDetails.Where(x => x.OrderDetailId == id)
                        .Select(x => new
                        {
                            ProductName = x.Product.Name,
                            x.SalePrice,
                            x.Quantity,
                            x.SubTotal,
                        }).ToListAsync();
                return Json(new { data = data });
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<JsonResult> GetPurchaseOrderInvoiceDetails(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var order = _appDbContext.PurchaseOrders.Where(x => x.CompanyId == user.CompanyId && x.Id == id)
                                               .Select(x => new
                                               {
                                                   x.InvNumber,
                                                   OrderId = x.Id,
                                                   Date = x.CreatedAt.ToString("yyyy-MMM-dd"),
                                                   details = x.PurchaseOrderDetails.Select(od => new
                                                   {
                                                       ProductName = od.Product.Name,
                                                       od.SalePrice,
                                                       od.Quantity,
                                                       Subtotal = od.SubTotal
                                                   }).ToList()
                                               }).ToList().Select(x => new
                                               {
                                                   qrCode = GenerateQC(x.InvNumber.ToString()),
                                                   x.InvNumber,
                                                   OrderId = x.OrderId,
                                                   Date = x.Date,
                                                   x.details
                                               }).FirstOrDefault();

                return Json(order);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region ==== Return Order =====

        public async Task<IActionResult> ReturnOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.Company = await _appDbContext.Companies
                                                 .FirstOrDefaultAsync(c => c.CompanyId == user.CompanyId);
            return View();
        }

        public async Task<JsonResult> GetReturnOrders(SearchFilter filter)
        {
            var fromDate = Convert.ToDateTime($"{filter.FromDate} 00:00:00");
            var toDate = Convert.ToDateTime($"{filter.ToDate} 23:59:59");

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var query = _appDbContext.ReturnOrders.Where(x => x.CompanyId == user.CompanyId && x.CreatedAt >= fromDate && x.CreatedAt <= toDate);
                int totalRows = query.Count();
                var data = await query.OrderByDescending(x => x.Id).Skip(filter.Start).Take(filter.PageLength)
                             .Select(x => new
                             {
                                 x.Id,
                                 x.SaleOrderId,
                                 CustomerName = x.Customer.Name,
                                 x.Total,
                                 OrderDate = (x.CreatedAt != null) ? x.CreatedAt.ToString("yyyy-MM-dd") : "",
                                 UpdatedDate = (x.UpdatedAt != null) ? x.UpdatedAt.ToString("yyyy-MM-dd") : ""
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

        public async Task<IActionResult> ExportReturnOrders(SearchFilter filter)
        {
            var fromDate = Convert.ToDateTime($"{filter.FromDate} 00:00:00");
            var toDate = Convert.ToDateTime($"{filter.ToDate} 23:59:59");

            try
            {
                var user = await _userManager.GetUserAsync(User);

                long.TryParse(filter.SearchTerm, out long orderId);

                var query = _appDbContext.ReturnOrders.Where(x => x.CompanyId == user.CompanyId
                                         && x.CreatedAt >= fromDate && x.CreatedAt <= toDate);

                if (orderId > 0)
                {
                    query = query.Where(x => x.Id == orderId);
                }

                var fileContents = ExportExcelReport.GetExcelFile(query.Select(p =>
                    new
                    {
                        ReturnId = p.Id,
                        SaleOrderId = p.SaleOrderId,
                        SupplierName = p.Customer.Name,
                        Total = p.Total,
                        OrderDate = p.CreatedAt.ToString("yyyy-MM-dd")
                    }).ToList());
                return File(
                    fileContents: fileContents,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: $"Return_Orders_{filter.FromDate}_To_{filter.ToDate}.xlsx"
                );
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        public async Task<JsonResult> GetReturnOrderDetail(int id)
        {
            try
            {
                var data = await _appDbContext.ReturnOrderDetails.Where(x => x.ReturnOrderId == id)
                        .Select(x => new
                        {
                            ProductName = x.Product.Name,
                            x.SalePrice,
                            x.Quantity,
                            x.SubTotal,
                        }).ToListAsync();
                return Json(new { data = data });
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<JsonResult> GetReturnOrderInvoiceDetails(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var order = _appDbContext.ReturnOrders.Where(x => x.CompanyId == user.CompanyId && x.Id == id)
                                               .Select(x => new
                                               {
                                                   x.InvNumber,
                                                   OrderId = x.Id,
                                                   Date = x.CreatedAt.ToString("yyyy-MMM-dd"),
                                                   details = x.ReturnOrderDetails.Select(od => new
                                                   {
                                                       ProductName = od.Product.Name,                                                       
                                                       od.SalePrice,
                                                       od.Quantity,
                                                       Subtotal = od.SubTotal
                                                   }).ToList()
                                               }).ToList().Select(x => new
                                               {
                                                   qrCode = GenerateQC(x.InvNumber.ToString()),
                                                   x.InvNumber,
                                                   OrderId = x.OrderId,
                                                   Date = x.Date,
                                                   x.details
                                               }).FirstOrDefault();

                return Json(order);
            }
            catch (Exception e)
            {
                throw e;
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