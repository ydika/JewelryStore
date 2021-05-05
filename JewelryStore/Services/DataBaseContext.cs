using JewelryStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Services
{
    public class DataBaseContext : DbContext
    {
        private readonly IConfiguration configuration;

        public DbSet<ModelBasket> Baskets { get; set; }
        public DbSet<ModelBasketContents> BasketContents { get; set; }
        public DbSet<ModelCharacteristics> Characteristics { get; set; }
        public DbSet<ModelCharacteristicValues> CharacteristicValues { get; set; }
        public DbSet<ModelDiscounts> Discounts { get; set; }
        public DbSet<ModelJewelry> Jewelries { get; set; }
        public DbSet<ModelJewelryKinds> JewelryKinds { get; set; }
        public DbSet<ModelUsers> Users { get; set; }

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
