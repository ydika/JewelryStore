using JewelryStore.Models;
using JewelryStore.Services;
using JewelryStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataBaseContext dbContext;

        public HomeController(ILogger<HomeController> logger, DataBaseContext context)
        {
            _logger = logger;
            dbContext = context;
        }

        public async Task<IActionResult> Index()
        {
            List<JewelryModel> jewelries = await dbContext.Jewelries.Include(x => x.Discount).Include(x => x.JewelrySizes).ToListAsync();
            List<JewelryModel> jewelriesWithDiscount = jewelries.Where(x => x.Discount.Amount > 0).Take(12).ToList();

            List<JewelrySliderViewModel> jewelrySliders = new List<JewelrySliderViewModel>();
            for (int i = 0; i < jewelriesWithDiscount.Count() / 4; i++)
            {
                jewelrySliders.Add(new JewelrySliderViewModel(jewelriesWithDiscount.Skip(i * 4).Take(4).ToList(), jewelries.OrderByDescending(x => x.ID).Skip(i * 4).Take(4).ToList()));
            }
            return View(jewelrySliders);
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
