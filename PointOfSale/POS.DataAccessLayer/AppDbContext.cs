using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using POS.DataAccessLayer.Models;
using POS.DataAccessLayer.Models.Category;
using POS.DataAccessLayer.Models.Company;
using POS.DataAccessLayer.Models.Customer;
using POS.DataAccessLayer.Models.Payment;
using POS.DataAccessLayer.Models.Subscriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace POS.DataAccessLayer
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<CompanyModel> Companies { get; set; }
        public DbSet<SubscriptionModel> Subscriptions { get; set; }
        public DbSet<CompanySubscriptionModel> CompanySubscriptions { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<CategoryDescriptionModel> CategoryDescriptions { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<ProductDescriptionModel> ProductDescriptions { get; set; }
        public DbSet<SupplierModel> Suppliers { get; set; }
        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<PaymentTypeModel> PaymentTypes { get; set; }
        public DbSet<SaleOrder> SaleOrder { get; set; }
        public DbSet<SaleOrderDetails> SaleOrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
