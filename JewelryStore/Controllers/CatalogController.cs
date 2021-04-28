using JewelryStore.Models;
using JewelryStore.Services;
using JewelryStore.ViewModels;
using Microsoft.AspNetCore.Http;
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

        private int displayedProducts = 24;

        public CatalogController(DataBaseContext context)
        {
            context.Jewelries.Include(x => x.Kind).Include(x => x.Discount).Load();
            context.JewelryKinds.Load();
            context.JewelryCharacteristics.Include(x => x.Jewelry).Include(x => x.CharacteristicValues).Load();
            context.Characteristics.Load();

            dbContext = context;
        }

        [Route("[controller]/{jkind}")]
        public IActionResult List(string jkind = "all", int currentpage = 1)
        {
            ViewData["CurrentPage"] = currentpage;

            List<ModelJewelry> jewelries = dbContext.Jewelries.ToList();
            List<ModelCharacteristicValues> characteristicValues = null;

            if (jkind == null || jkind == "all")
            {
                characteristicValues = dbContext.JewelryCharacteristics.Select(x => x.CharacteristicValues).ToList();
            }
            else
            {
                jewelries = jewelries.Where(x => x.Kind.EnName.ToLower() == jkind).ToList();

                characteristicValues = dbContext.JewelryCharacteristics.Where(x => x.Jewelry.Kind.EnName.ToLower() == jkind).Select(x => x.CharacteristicValues).ToList();
            }

            ViewData["PageCount"] = (int)Math.Ceiling((decimal)jewelries.Count() / displayedProducts);

            jewelries = jewelries.Skip(displayedProducts * ((currentpage - 1) < 0 ? 0 : currentpage - 1)).Take(displayedProducts).ToList();

            List<ModelFilterContent> filter = new List<ModelFilterContent>();
            foreach (var characteristic in dbContext.Characteristics.ToList())
            {
                filter.Add(new ModelFilterContent(characteristic.Name, characteristicValues.Where(x => x.Characteristic.Name == characteristic.Name).Distinct().ToList()));
            }

            return View(new CatalogViewModel(jewelries, dbContext.JewelryKinds.ToList(), filter));
        }
    }
}
