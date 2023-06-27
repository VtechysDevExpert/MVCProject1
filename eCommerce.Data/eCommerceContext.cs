﻿using eCommerce.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace eCommerce.Data
{
    public class eCommerceContext : IdentityDbContext<eCommerceUser>
    {
        public eCommerceContext() : base("name=eCommerceConnectionString_OK")
        {
            Database.SetInitializer<eCommerceContext>(new eCommerceDBInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // This needs to go before the other rules!

            modelBuilder.Entity<Promo>()
                .HasIndex(p => new { p.Code })
                .IsUnique(true);

            modelBuilder.Entity<eCommerceUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
        }

        public DbSet<Language> Languages { get; set; }
        public DbSet<LanguageResource> LanguageResources { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryRecord> CategoryRecords { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductRecord> ProductRecords { get; set; }
        public DbSet<Promo> Promos { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<ProductPicture> ProductPictures { get; set; }
        public DbSet<ProductSpecification> ProductSpecifications { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<NewsletterSubscription> NewsletterSubscriptions { get; set; }
        public DbSet<ProductDetails> ProductDtls { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<ReturnRequest> ReturnRequest { get; set; }
        public DbSet<CancelRequest> CancelRequest { get; set; }

        public DbSet<UserAddress> Users_Address { get; set; }
        public DbSet<OrderPaymentDetails> OrderPayment_Dtls { get;set;}
        public  DbSet<CustomerReviews> CustomerReviews { get; set; }
        public static eCommerceContext Create()
        {
            return new eCommerceContext();
        }
    }
}
