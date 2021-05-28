using JewelryStore.Models;
using JewelryStore.Services;
using JewelryStore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly DataBaseContext dbContext;
        private readonly UserManager<UserModel> _userManager;

        private int displayedQuantity = 12;

        public CatalogController(DataBaseContext context, UserManager<UserModel> userManager)
        {
            dbContext = context;
            _userManager = userManager;
        }

        [Route("{jkind}")]
        public async Task<IActionResult> List(string jkind = "all")
        {
            await dbContext.JewelryKinds.LoadAsync();

            ViewData["Title"] = "Ювелирные изделия купить";
            if (jkind != "all")
            {
                ViewData["Title"] = dbContext.JewelryKinds.Where(x => x.EnName == jkind).Select(x => x.RuName).FirstOrDefault() + " купить";
            }

            return View(await dbContext.JewelryKinds.ToListAsync());
        }

        [Route("{jkind}/{code}")]
        public IActionResult Item(string code)
        {
            dbContext.Jewelries.Include(x => x.Discount).Load();
            dbContext.JewelryCharacteristics.Load();
            dbContext.Characteristics.Include(x => x.CharacteristicValues).Load();

            JewelryModel jewelry = dbContext.Jewelries.Where(x => x.Code == code).FirstOrDefault();
            List<ItemCharacteristics> itemCharacteristics = new List<ItemCharacteristics>();
            List<CharacteristicValueModel> characteristicValues = null;
            foreach (var characteristic in dbContext.Characteristics.ToList())
            {
                if (characteristic.CharacteristicValues.Count() > 0)
                {
                    characteristicValues = jewelry.JewelryCharacteristics.Select(x => x.CharacteristicValues).Where(x => x.Characteristic.Name == characteristic.Name).ToList();
                    if (characteristicValues.Count() > 0)
                    {
                        itemCharacteristics.Add(new ItemCharacteristics(characteristic.Name,
                                        jewelry.JewelryCharacteristics.Select(x => x.CharacteristicValues).Where(x => x.Characteristic.Name == characteristic.Name).ToList()));
                    }
                }
            }

            return View(new ItemViewModel(jewelry, itemCharacteristics));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> GetItemNames(string searchName)
        {
            if (searchName == null || searchName == "")
            {
                return Json("");
            }

            await dbContext.Jewelries.Include(x => x.Discount).LoadAsync();

            return Json(FilterByName(searchName).Select(x => x.Name).Distinct());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> GetCatalogFilter()
        {
            await dbContext.Characteristics.Include(x => x.CharacteristicValues).LoadAsync();

            return Json(await dbContext.Characteristics.Where(x => x.CharacteristicValues.Count() > 0).ToListAsync());
        }

        [HttpGet]
        [Route("{jkind}/[action]")]
        public async Task<JsonResult> GetJewelriesCards(string searchName, string[] o, string jkind = "all", int page = 1)
        {
            List<JewelryModel> jewelries = null;

            if (searchName != null && searchName != "")
            {
                await dbContext.Jewelries.Include(x => x.Discount).LoadAsync();

                jewelries = FilterByName(searchName);

                return Json(new CardsViewModel(jewelries.Skip(displayedQuantity * ((page - 1) < 0 ? 0 : page - 1)).Take(displayedQuantity).ToList(),
                    page, (int)Math.Ceiling((decimal)jewelries.Count() / displayedQuantity)));
            }

            await dbContext.Jewelries.Include(x => x.Kind).Include(x => x.Discount).Include(x => x.JewelryCharacteristics).LoadAsync();
            await dbContext.JewelryCharacteristics.Include(x => x.CharacteristicValues).LoadAsync();

            jewelries = await dbContext.Jewelries.ToListAsync();

            if (jkind != "all")
            {
                jewelries = jewelries.Where(x => x.Kind.EnName.ToLower() == jkind).ToList();
            }

            for (int i = 0; i < o.Length; i++)
            {
                jewelries = jewelries.Where(x => x.JewelryCharacteristics.Select(x => x.CharacteristicValues.Value).Contains(o[i])).ToList();
            }

            int pageCount = (int)Math.Ceiling((decimal)jewelries.Count() / displayedQuantity);

            jewelries = jewelries.Skip(displayedQuantity * ((page - 1) < 0 ? 0 : page - 1)).Take(displayedQuantity).ToList();

            return Json(new CardsViewModel(jewelries, page, pageCount));
        }

        [NonAction]
        private List<JewelryModel> FilterByName(string searchName)
        {
            char[] delimiterChars = new char[] { ' ', ',', '.', ':', '\t' };
            var words = searchName.Split(delimiterChars);

            List<JewelryModel> jewelries = dbContext.Jewelries.ToList();

            foreach (string keyword in words)
            {
                jewelries = jewelries.Where(p => p.Name.ToUpper().Contains(keyword.ToUpper())).ToList();
            }

            return jewelries;
        }
    }
}