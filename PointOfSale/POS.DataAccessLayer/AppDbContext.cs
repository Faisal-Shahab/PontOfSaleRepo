﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using POS.DataAccessLayer.Models;
using POS.DataAccessLayer.Models.Company;
using POS.DataAccessLayer.Models.Customer;
using POS.DataAccessLayer.Models.Order;
using POS.DataAccessLayer.Models.Payment;
using POS.DataAccessLayer.Models.Subscriptions;
using POS.DataAccessLayer.ViewModels;

namespace POS.DataAccessLayer
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<CompanyModel> Companies { get; set; }
        public DbSet<SubscriptionModel> Subscriptions { get; set; }
        public DbSet<CompanySubscriptionModel> CompanySubscriptions { get; set; }
        public DbSet<Category> Categories { get; set; }        
        public DbSet<Product> Products { get; set; }        
        public DbSet<SupplierModel> Suppliers { get; set; }
        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<PaymentTypeModel> PaymentTypes { get; set; }
        public DbSet<SaleOrder> SaleOrder { get; set; }
        public DbSet<SaleOrderDetails> SaleOrderDetails { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderDetails> PurchaseOrderDetails { get; set; }

        [System.Obsolete]
        public DbQuery<UserViewModel> UserView { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
