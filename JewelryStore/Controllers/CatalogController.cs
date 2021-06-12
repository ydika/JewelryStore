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
using System.Globalization;
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
        [Route("{jkind}/{subspecies}")]
        public async Task<IActionResult> List()
        {
            await dbContext.Subspecies.Include(x => x.Jewelries).LoadAsync();
            List<JewelryModel> jewelries = await dbContext.Jewelries.Include(x => x.Discount).Where(x => x.Quantity > 0).ToListAsync();

            return View(new FiltersViewModel(
                await dbContext.JewelryKinds.Include(x => x.Subspecies).ToListAsync(),
                await dbContext.Characteristics.Include(x => x.CharacteristicValues).Where(x => x.CharacteristicValues.Count() > 0).ToListAsync(),
                Math.Ceiling(jewelries.Max(x => double.Parse(x.Price))).ToString(),
                Math.Ceiling(jewelries.Min(x => double.Parse(x.Price))).ToString()
            ));
        }

        [Route("{jkind}/{subspecies}/{code}")]
        public IActionResult Item(string code)
        {
            JewelryModel jewelry = dbContext.Jewelries.Include(x => x.Discount).Include(x => x.JewelryCharacteristics).Include(x => x.Subspecies.Kind).Include(x => x.JewelrySizes).FirstOrDefault(x => x.Code == code);
            if (jewelry.JewelryCharacteristics == null) return View(new ItemViewModel(jewelry, null));

            List<ItemCharacteristics> itemCharacteristics = new List<ItemCharacteristics>();
            List<CharacteristicValueModel> characteristicValues = null;
            foreach (var characteristic in dbContext.Characteristics.Include(x => x.CharacteristicValues).ToList())
            {
                if (characteristic.CharacteristicValues.Count() > 0)
                {
                    characteristicValues = jewelry.JewelryCharacteristics.Select(x => x.CharacteristicValues).Where(x => x.Characteristic.Name == characteristic.Name).ToList();
                    if (characteristicValues.Count() > 0)
                    {
                        itemCharacteristics.Add(new ItemCharacteristics(characteristic.Name, characteristicValues.OrderBy(x => x.ID).ToList()));
                    }
                }
            }

            List<ItemCharacteristics> jewelrySizes = itemCharacteristics.Where(x => x.Name == "Размер").ToList();
            if (jewelrySizes.Count() != 0)
            {
                var a = jewelry.JewelrySizes.FirstOrDefault(x => x.Size == jewelrySizes.Select(x => x.Values).FirstOrDefault().Select(x => x.Value).FirstOrDefault());
                if (a != null)
                {
                    jewelry.Price = a.Price.ToString();
                }
            }

            if (jewelry.Quantity >= 5)
            {
                jewelry.Quantity = 5;
            }

            return View(new ItemViewModel(jewelry, itemCharacteristics));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> GetItemPrice(int jewelryid, int quantity, string size = "16")
        {
            JewelryModel jewelry = await dbContext.Jewelries.Include(x => x.Discount).Include(x => x.JewelrySizes).FirstOrDefaultAsync(x => x.ID == jewelryid);

            List<JewelrySizeModel> jewelrySizes = jewelry.JewelrySizes.ToList();
            if (jewelrySizes.Count() != 0)
            {
                JewelrySizeModel first = jewelrySizes.FirstOrDefault(x => x.Size == size);
                if (first != null)
                {
                    jewelry.Price = first.Price;
                }
            }

            return Json(Math.Round(double.Parse(jewelry.Price) * quantity, 2).ToString("0.00"));
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

            return Json(FilterByName(searchName).Select(x => x.Name).Distinct().Take(5));
        }

        [HttpGet]
        [Route("{jkind}/[action]")]
        [Route("{jkind}/{subspecies}/[action]")]
        public async Task<JsonResult> GetJewelriesCards(int maxPrice, int minPrice, string subspecies, string searchName, string[] o, string jkind = "list", int page = 1)
        {
            List<JewelryModel> jewelries = null;

            await dbContext.Jewelries.Include(x => x.Subspecies.Kind).Include(x => x.Discount).Include(x => x.JewelryCharacteristics).Include(x => x.JewelrySizes).LoadAsync();
            await dbContext.JewelryCharacteristics.Include(x => x.CharacteristicValues).LoadAsync();

            if (searchName != null && searchName != "")
            {
                jewelries = FilterByName(searchName);
            }
            else
            {
                jewelries = await dbContext.Jewelries.Where(x => x.Quantity > 0).OrderByDescending(x => x.ID).ToListAsync();
            }

            if (jkind != "list")
            {
                jewelries = jewelries.Where(x => x.Subspecies.Kind.EnName.ToLower() == jkind).ToList();
                if (subspecies != null && subspecies != "" && subspecies != "all")
                {
                    jewelries = jewelries.Where(x => x.Subspecies.EnName.ToLower() == subspecies).ToList();
                }
            }

            if (minPrice >= 0 && maxPrice >= 0 && maxPrice >= minPrice)
            {
                jewelries = jewelries.Where(x => double.Parse(x.Price) >= minPrice && double.Parse(x.Price) <= maxPrice).ToList();
            }

            if (o.Length > 0)
            {
                List<JewelryModel> buff = new List<JewelryModel>();
                for (int i = 0; i < o.Length; i++)
                {
                    foreach (var item in jewelries.Where(x => x.JewelryCharacteristics.Select(x => x.CharacteristicValues.Value).Contains(o[i])).ToList())
                    {
                        buff.Add(item);
                    }
                }
                jewelries = buff.Distinct().ToList();
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

            List<JewelryModel> jewelries = dbContext.Jewelries.Where(x => x.Quantity > 0).OrderByDescending(x => x.ID).ToList();

            foreach (string keyword in words)
            {
                jewelries = jewelries.Where(p => p.Name.ToUpper().Contains(keyword.ToUpper())).ToList();
            }

            return jewelries;
        }
    }
}