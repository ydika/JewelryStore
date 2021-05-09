using JewelryStore.Models;
using JewelryStore.Models.DataBase;
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

        public DbSet<BasketModel> Baskets { get; set; }
        public DbSet<BasketContentsModel> BasketContents { get; set; }
        public DbSet<CharacteristicsModel> Characteristics { get; set; }
        public DbSet<CharacteristicValuesModel> CharacteristicValues { get; set; }
        public DbSet<DiscountsModel> Discounts { get; set; }
        public DbSet<JewelryModel> Jewelries { get; set; }
        public DbSet<JewelryKindsModel> JewelryKinds { get; set; }
        public DbSet<UsersModel> Users { get; set; }

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
