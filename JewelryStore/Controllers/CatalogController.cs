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

        private static string jewelryKind = "all";

        private int displayedQuantity = 3;

        public CatalogController(DataBaseContext context)
        {
            dbContext = context;
        }

        [Route("{jkind}")]
        public IActionResult List(string jkind = "all")
        {
            jewelryKind = jkind;
            return View();
        }

        [HttpGet]
        [Route("[action]")]
        public JsonResult GetCatalogFilter()
        {
            dbContext.JewelryKinds.Load();
            dbContext.Characteristics.Include(x => x.CharacteristicValues).Load();

            return Json(new FilterModel(dbContext.JewelryKinds.ToList(), 
                dbContext.Characteristics.Where(x => x.CharacteristicValues.Count() > 0).ToList()));
        }

        [HttpGet]
        [Route("[action]")]
        public JsonResult GetJewelriesCards(string[] o, int page = 1)
        {
            dbContext.Jewelries.Include(x => x.Kind).Include(x => x.Discount).Load();
            dbContext.JewelryCharacteristics.Include(x => x.Jewelry).Include(x => x.CharacteristicValues).Load();

            List<JewelryModel> jewelries = dbContext.Jewelries.ToList();
            
            if (jewelryKind != null && jewelryKind != "all")
            {
                jewelries = jewelries.Where(x => x.Kind.EnName.ToLower() == jewelryKind).ToList();
            }

            for (int i = 0; i < (o != null ? o.Length : 0); i++)
            {
                jewelries = jewelries.Where(x => x.JewelryCharacteristics.Select(x => x.CharacteristicValues.Value).Contains(o[i])).ToList();
            }

            int pageCount = (int)Math.Ceiling((decimal)jewelries.Count() / displayedQuantity);

            jewelries = jewelries.Skip(displayedQuantity * ((page - 1) < 0 ? 0 : page - 1)).Take(displayedQuantity).ToList();

            return Json(new CardsModel(jewelries, page, pageCount));
        }
    }
}