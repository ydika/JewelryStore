using JewelryStore.Models;
using JewelryStore.Models.Catalog;
using JewelryStore.Models.DataBase;
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

        private List<JewelryModel> jewelries = null;
        private List<CharacteristicValuesModel> characteristicValues = null;
        private List<DynamicFilterModel> filter = new List<DynamicFilterModel>();

        private static string jewelryKind = "all";

        private int displayedQuantity = 3;

        public CatalogController(DataBaseContext context)
        {
            context.Jewelries.Include(x => x.Kind).Include(x => x.Discount).Include(x => x.CharacteristicValues).Load();
            context.JewelryKinds.Load();
            context.CharacteristicValues.Include(x => x.Jewelry).Load();
            context.Characteristics.Load();

            dbContext = context;
        }

        [Route("{jkind}")]
        public IActionResult List(string jkind = "all")
        {
            jewelryKind = jkind;
            return View();
        }

        //[Route("{jkind}")]
        //public IActionResult List(string[] o, string jkind = "all", int page = 1)
        //{
        //    jewelryKind = jkind;
        //    jewelries = dbContext.Jewelries.ToList();
        //    characteristicValues = null;

        //    if (jkind == null || jkind == "all")
        //    {
        //        characteristicValues = dbContext.CharacteristicValues.ToList();
        //    }
        //    else
        //    {
        //        jewelries = jewelries.Where(x => x.Kind.EnName.ToLower() == jkind).ToList();

        //        characteristicValues = dbContext.CharacteristicValues.Where(x => x.Jewelry.Kind.EnName.ToLower() == jkind).ToList();
        //    }

        //    for (int i = 0; i < o.Length; i++)
        //    {
        //        jewelries = jewelries.Where(x => x.CharacteristicValues.Select(x => x.Value).Contains(o[i])).ToList();
        //    }

        //    jewelries = jewelries.Skip(displayedQuantity * ((page - 1) < 0 ? 0 : page - 1)).Take(displayedQuantity).ToList();

        //    foreach (var characteristic in dbContext.Characteristics.ToList())
        //    {
        //        filter.Add(new DynamicFilterModel(characteristic.Name, characteristicValues.Where(x => x.Characteristic.Name == characteristic.Name).Select(x => x.Value).Distinct().ToList()));
        //    }

        //    return View(new CatalogViewModel(jewelries, dbContext.JewelryKinds.ToList(), filter));
        //}

        [HttpGet]
        [Route("[action]")]
        public JsonResult GetCatalogFilter()
        {
            foreach (var characteristic in dbContext.Characteristics.ToList())
            {
                filter.Add(new DynamicFilterModel(characteristic.Name, characteristicValues.Where(x => x.Characteristic.Name == characteristic.Name).Select(x => x.Value).Distinct().ToList()));
            }

            return Json(filter);
        }

        [HttpGet]
        [Route("[action]")]
        public JsonResult GetJewelriesCards(string[] o, int page = 1)
        {
            jewelries = dbContext.Jewelries.ToList();
            
            if (jewelryKind != null && jewelryKind != "all")
            {
                jewelries = jewelries.Where(x => x.Kind.EnName.ToLower() == jewelryKind).ToList();
            }

            for (int i = 0; i < (o != null ? o.Length : 0); i++)
            {
                jewelries = jewelries.Where(x => x.CharacteristicValues.Select(x => x.Value).Contains(o[i])).ToList();
            }

            int pageCount = (int)Math.Ceiling((decimal)jewelries.Count() / displayedQuantity);

            jewelries = jewelries.Skip(displayedQuantity * ((page - 1) < 0 ? 0 : page - 1)).Take(displayedQuantity).ToList();

            return Json(new CardsModel(jewelries, page, pageCount));
        }
    }
}