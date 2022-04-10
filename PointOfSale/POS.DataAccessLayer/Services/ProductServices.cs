using Microsoft.EntityFrameworkCore;
using PointOfSale.Model;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models.Category;
using POS.DataAccessLayer.Models.Company;
using POS.DataAccessLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                                Product = x,
                                ProductName = x.ProductDescriptions.FirstOrDefault(x => x.LanguageId == filter.LanguageId).Name,
                                CategoryName = x.Category.CategoryDescriptions.FirstOrDefault(x => x.LanguageId == filter.LanguageId).Name,
                                CompanyName = (filter.LanguageId == 1) ? x.Company.Name : x.Company.ArabicName,
                                Total = total
                            });
        }

        public async Task<bool> Insert(ProductViewModel model, string user)
        {
            ProductModel product = new ProductModel
            {
                CategoryId = model.CategoryId,
                CompanyId = CompanyId,
                Barcode = model.Barcode,
                CostPrice = model.CostPrice,
                SalePrice = model.SalePrice,
                Size = model.Size,
                Quantity = model.Quantity,
                DiscoutPercentage = model.DiscoutPercentage,
                IsTaxApplied = model.IsTaxApplied,
                Color = model.Color,
                CreateAt = DateTime.UtcNow.AddHours(3),
                UpdatedAt = DateTime.UtcNow.AddHours(3),
                CreateBy = user
            };
            _appDbContext.Products.Add(product);

            _appDbContext.SaveChanges();

            _appDbContext.ProductDescriptions.Add(new ProductDescriptionModel
            {
                ProductId = product.ProductId,
                Name = model.Name,
                LanguageId = 1,
                CreateAt = DateTime.UtcNow.AddHours(3),
                UpdatedAt = DateTime.UtcNow.AddHours(3),
                CreateBy = user
            });

            _appDbContext.ProductDescriptions.Add(new ProductDescriptionModel
            {
                ProductId = product.ProductId,
                Name = model.ArabicName,
                LanguageId = 2,
                UpdatedAt = DateTime.UtcNow.AddHours(3),
                UpdatedBy = user
            });
            return _appDbContext.SaveChanges() > 0;
        }

        public async Task<bool> Update(ProductViewModel model, string user)
        {
            var data = _appDbContext.Products.Where(x => x.CompanyId == CompanyId && x.ProductId == model.ProductId).Select(x => new
            {
                Product = x,
                EngData = x.ProductDescriptions.FirstOrDefault(x => x.LanguageId == 1),
                ArData = x.ProductDescriptions.FirstOrDefault(x => x.LanguageId == 2)
            }).FirstOrDefault();

            data.Product.CompanyId = CompanyId;
            data.Product.CategoryId = model.CategoryId;
            data.Product.Barcode = model.Barcode;
            data.Product.CostPrice = model.CostPrice;
            data.Product.SalePrice = model.SalePrice;
            data.Product.Size = model.Size;
            data.Product.Quantity = model.Quantity;
            data.Product.DiscoutPercentage = model.DiscoutPercentage;
            data.Product.IsTaxApplied = model.IsTaxApplied;
            data.Product.Color = model.Color;
            data.Product.UpdatedAt = DateTime.UtcNow.AddHours(3);
            data.Product.UpdatedBy = user;
            _appDbContext.Products.Update(data.Product);

            data.EngData.Name = model.Name;
            data.EngData.UpdatedAt = DateTime.UtcNow.AddHours(3);
            data.EngData.UpdatedBy = user;
            _appDbContext.ProductDescriptions.Update(data.EngData);

            data.ArData.Name = model.ArabicName;
            data.ArData.UpdatedAt = DateTime.UtcNow.AddHours(3);
            data.ArData.UpdatedBy = user;
            _appDbContext.ProductDescriptions.Update(data.ArData);
            return (await _appDbContext.SaveChangesAsync()) > 0;
        }

        public async Task<ProductViewModel> GetProductById(int id)
        {
            return await _appDbContext.Products.Where(x => x.CompanyId == CompanyId && x.ProductId == id).Select(x => new ProductViewModel
            {
                ProductId = x.ProductId,
                CategoryId = x.CategoryId,
                CompanyId = x.CompanyId,
                CostPrice = x.CostPrice,
                SalePrice = x.SalePrice,
                Size = x.Size,
                Quantity = x.Quantity,
                Color = x.Color,
                IsTaxApplied = x.IsTaxApplied,
                DiscoutPercentage = x.DiscoutPercentage,
                Barcode = x.Barcode,
                Name = x.ProductDescriptions.FirstOrDefault(x => x.LanguageId == 1).Name,
                ArabicName = x.ProductDescriptions.FirstOrDefault(x => x.LanguageId == 2).Name
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
