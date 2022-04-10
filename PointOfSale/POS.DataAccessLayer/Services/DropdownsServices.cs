using Microsoft.EntityFrameworkCore;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models.Category;
using POS.DataAccessLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.DataAccessLayer.Services
{
    public class DropdownsServices : IDropdownsServices
    {
        AppDbContext _appDbContext;
        public int CompanyId { get; set; }
        public int LanguageId { get; set; }
        public DropdownsServices(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<SelectListViewModel>> CategoriesDropdown()
        {
            var selectList = await _appDbContext.Categories.Where(x => x.CompanyId == CompanyId)
                .Select(x => new SelectListViewModel
                {
                    Text = x.CategoryDescriptions.FirstOrDefault(c => c.LanguageId == LanguageId).Name,
                    Value = x.CategoryId.ToString()
                }).ToListAsync();
            return selectList;
        }

        public async Task<List<ProductListViewModel>> ProductsDropdown(string value)
        {
            var products = _appDbContext.Products.Where(x => x.CompanyId == CompanyId);

            products = products.Where(x => x.Barcode == value || x.ProductDescriptions.FirstOrDefault(x => x.LanguageId == 1).Name.ToLower().Contains(value.ToLower()));

            var result = await products.Select(x => new ProductListViewModel
            {
                Id = x.ProductId,
                Name = x.ProductDescriptions.FirstOrDefault(p => p.LanguageId == LanguageId).Name,
                Price = x.SalePrice,
                Discount = x.DiscoutPercentage
            }).ToListAsync();
            return result;
        }

        public async Task<List<SelectListViewModel>> CompaniesDropdown()
        {
            var selectList = await _appDbContext.Companies.Select(x => new SelectListViewModel
            {
                Text = LanguageId == 1 ? x.Name : x.ArabicName,
                Value = x.CompanyId.ToString()
            }).ToListAsync();
            return selectList;
        }

        public async Task<List<SelectListViewModel>> PaymentTypesDropdown()
        {
            var selectList = await _appDbContext.PaymentTypes.Select(x => new SelectListViewModel
            {
                Text = x.Name,
                Value = x.PaymentId.ToString()
            }).ToListAsync();
            return selectList;
        }

        public async Task<List<SelectListViewModel>> CustomersDropdown(string value)
        {
            var selectList = await _appDbContext.Customers
                          .Where(x => x.CompanyId == CompanyId && x.ContactNo.Contains(value)).Take(10)
                          .Select(x => new SelectListViewModel
                          {
                              Text = x.Name,
                              Value = x.CustomerId.ToString()
                          }).ToListAsync();
            return selectList;
        }


        public async Task<List<SelectListViewModel>> SuppliersDropdown(string value)
        {
            var selectList = await _appDbContext.Suppliers
                          .Where(x => x.CompanyId == CompanyId && x.ContactNo.Contains(value)).Take(10)
                          .Select(x => new SelectListViewModel
                          {
                              Text = x.Name,
                              Value = x.SupplierId.ToString()
                          }).ToListAsync();
            return selectList;
        }

        public async Task<List<SelectListViewModel>> SubscriptionsDropdown(string value)
        {
            var selectList = await _appDbContext.Subscriptions.Select(x => new SelectListViewModel
            {
                Text = (LanguageId == 1 ? x.Name : x.ArabicName),
                Value = x.SubscriptionId.ToString()
            }).ToListAsync();

            return selectList;
        }

        public async Task<List<SelectListViewModel>> RolesDropdown()
        {
            var selectList = await _appDbContext.Roles.Select(x => new SelectListViewModel
            {
                Text = x.Name,
                Value = x.NormalizedName               
            }).ToListAsync();
            return selectList;
        }
    }
}
