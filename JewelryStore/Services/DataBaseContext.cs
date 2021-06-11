using JewelryStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Services
{
    public class DataBaseContext : IdentityDbContext<UserModel>
    {
        private readonly IConfiguration configuration;

        public DbSet<CartContentModel> CartContent { get; set; }
        public DbSet<CartModel> Cart { get; set; }
        public DbSet<CharacteristicsModel> Characteristics { get; set; }
        public DbSet<CharacteristicValueModel> CharacteristicValues { get; set; }
        public DbSet<DiscountModel> Discounts { get; set; }
        public DbSet<JewelryModel> Jewelries { get; set; }
        public DbSet<JewelryCharacteristicsModel> JewelryCharacteristics { get; set; }
        public DbSet<JewelryKindsModel> JewelryKinds { get; set; }
        public DbSet<JewelrySizeModel> JewelrySizes { get; set; }
        public DbSet<OrderContentModel> OrderContents { get; set; }
        public DbSet<OrdersModel> Orders { get; set; }
        public DbSet<SubspeciesModel> Subspecies { get; set; }

        public DataBaseContext(IConfiguration configuration)
        {
            this.configuration = configuration;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
