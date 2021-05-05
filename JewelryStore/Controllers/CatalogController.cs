using JewelryStore.Models;
using JewelryStore.Services;
using JewelryStore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Controllers
{
    [Route("[controller]")]
    public class CatalogController : Controller
    {
        private DataBaseContext dbContext;

        private List<ModelJewelry> jewelries = null;
        private List<ModelCharacteristicValues> characteristicValues = null;
        private List<ModelDynamicFilter> filter = new List<ModelDynamicFilter>();

        private static string jewelryKind = "all";

        private int displayedProducts = 3;

        public CatalogController(DataBaseContext context)
        {
            context.Jewelries.Include(x => x.Kind).Include(x => x.Discount).Include(x => x.CharacteristicValues).Load();
            context.JewelryKinds.Load();
            context.CharacteristicValues.Include(x => x.Jewelry).Load();
            context.Characteristics.Load();

            dbContext = context;
        }

        public IActionResult Main() 
        {
            return View();
        }

        [Route("{jkind}")]
        public IActionResult List(string[] o, string jkind = "all", int page = 1)
        {
            jewelryKind = jkind;
            jewelries = dbContext.Jewelries.ToList();
            characteristicValues = null;

            if (jkind == null || jkind == "all")
            {
                characteristicValues = dbContext.CharacteristicValues.ToList();
            }
            else
            {
                jewelries = jewelries.Where(x => x.Kind.EnName.ToLower() == jkind).ToList();

                characteristicValues = dbContext.CharacteristicValues.Where(x => x.Jewelry.Kind.EnName.ToLower() == jkind).ToList();
            }

            for (int i = 0; i < o.Length; i++)
            {
                jewelries = jewelries.Where(x => x.CharacteristicValues.Select(x => x.Value).Contains(o[i])).ToList();
            }

            ViewData["CurrentPage"] = page;
            ViewData["PageCount"] = (int)Math.Ceiling((decimal)jewelries.Count() / displayedProducts);

            jewelries = jewelries.Skip(displayedProducts * ((page - 1) < 0 ? 0 : page - 1)).Take(displayedProducts).ToList();

            foreach (var characteristic in dbContext.Characteristics.ToList())
            {
                filter.Add(new ModelDynamicFilter(characteristic.Name, characteristicValues.Where(x => x.Characteristic.Name == characteristic.Name).Select(x => x.Value).Distinct().ToList()));
            }

            return View(new CatalogViewModel(jewelries, dbContext.JewelryKinds.ToList(), filter));
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Cards(string[] o, int page = 1)
        {
            jewelries = dbContext.Jewelries.ToList();
            
            if (jewelryKind != null && jewelryKind != "all")
            {
                jewelries = jewelries.Where(x => x.Kind.EnName.ToLower() == jewelryKind).ToList();
            }

            for (int i = 0; i < o.Length; i++)
            {
                jewelries = jewelries.Where(x => x.CharacteristicValues.Select(x => x.Value).Contains(o[i])).ToList();
            }

            int jCount = jewelries.Count();
            ViewData["CurrentPage"] = page;
            ViewData["PageCount"] = (int)Math.Ceiling((decimal)jewelries.Count() / displayedProducts);

            jewelries = jewelries.Skip(displayedProducts * ((page - 1) < 0 ? 0 : page - 1)).Take(displayedProducts).ToList();

            ModelCards result = new ModelCards(jewelries, jCount);

            return Json(result);
        }
    }
}