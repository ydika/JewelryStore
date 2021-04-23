using JewelryStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Controllers
{
    public class CatalogController : Controller
    {
        private DataBaseContext dbContext;
        private int productCount = 10;

        public CatalogController(DataBaseContext context)
        {
            context.Jewelries.Include(x => x.Kind).Include(x => x.Discount).Load();

            dbContext = context;
        }

        public IActionResult Products(int page)
        {
            ViewData["PageNum"] = page;

            return View(dbContext.Jewelries.Skip(productCount * page).Take(productCount).ToList());
        }
    }
}
