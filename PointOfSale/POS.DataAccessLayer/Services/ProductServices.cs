using Microsoft.EntityFrameworkCore;
using PointOfSale.Model;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models;
using POS.DataAccessLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS.DataAccessLayer.Services
{

    public class ProductServices : IProductServices
    {
        private readonly AppDbContext _appDbContext;

        public int CompanyId { get; set; }

        public ProductServices(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IEnumerable<ProductsListViewModel>> GetProducts(SearchFilter filter)
        {
            var products = _appDbContext.Products.Where(x => x.CompanyId == CompanyId);
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var isNumber = double.TryParse(filter.SearchTerm, out double numericValue);
                if (isNumber)
                {
                    products = products.Where(x => x.ProductId == Convert.ToInt32(filter.SearchTerm));
                }
            }
            var total = products.Count();
            return products.OrderByDescending(x => x.ProductId).Skip(filter.Start).Take(filter.PageLength)
                            .Select(x => new ProductsListViewModel
                            {
                                ProductId = x.ProductId,
                                Name = filter.LanguageId == 1 ? x.Name : x.NameAr,
                                CategoryName = filter.LanguageId == 1 ? x.Category.Name : x.NameAr,
                                Quantity = x.Quantity,
                                CostPrice = x.CostPrice,
                                SalePrice = x.SalePrice,
                                Discount = x.Discount,
                                Total = total
                            });
        }

        public async Task<bool> Insert(ProductViewModel model, string user)
        {
            Product product = new Product
            {
                Name = model.Name,
                NameAr = model.ArabicName,
                CategoryId = model.CategoryId,
                CompanyId = CompanyId,
                Barcode = model.Barcode,
                CostPrice = model.CostPrice,
                SalePrice = model.SalePrice,
                Quantity = model.Quantity,
                Discount = model.DiscoutPercentage,
                IsTaxApplied = model.IsTaxApplied,
                CreateAt = DateTime.UtcNow.AddHours(3),
                UpdatedAt = DateTime.UtcNow.AddHours(3),
                CreateBy = user
            };

            _appDbContext.Products.Add(product);

            return await _appDbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(ProductViewModel model, string user)
        {
            var data = _appDbContext.Products.Where(x => x.CompanyId == CompanyId
                                                && x.ProductId == model.ProductId).FirstOrDefault();
            data.Name = model.Name;
            data.NameAr = model.ArabicName;
            data.CompanyId = CompanyId;
            data.CategoryId = model.CategoryId;
            data.Barcode = model.Barcode;
            data.CostPrice = model.CostPrice;
            data.SalePrice = model.SalePrice;
            data.Quantity = model.Quantity;
            data.Discount = model.DiscoutPercentage;
            data.IsTaxApplied = model.IsTaxApplied;
            data.UpdatedAt = DateTime.UtcNow.AddHours(3);
            data.UpdatedBy = user;
            _appDbContext.Products.Update(data);

            return (await _appDbContext.SaveChangesAsync()) > 0;
        }

        public async Task<ProductViewModel> GetProductById(int id)
        {
            return await _appDbContext.Products.Where(x => x.CompanyId == CompanyId && x.ProductId == id).Select(x => new ProductViewModel
            {
                ProductId = x.ProductId,
                CategoryId = (int)x.CategoryId,
                CompanyId = x.CompanyId,
                CostPrice = x.CostPrice,
                SalePrice = x.SalePrice,
                Quantity = x.Quantity,
                IsTaxApplied = x.IsTaxApplied,
                DiscoutPercentage = (int)x.Discount,
                Barcode = x.Barcode,
                Name = x.Name,
                ArabicName = x.NameAr
            }).FirstOrDefaultAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var product = await _appDbContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == CompanyId && x.ProductId == id);
            _appDbContext.Products.Remove(product);
            return (await _appDbContext.SaveChangesAsync()) > 0;
        }
    }
}
