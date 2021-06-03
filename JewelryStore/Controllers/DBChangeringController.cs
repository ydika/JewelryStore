using JewelryStore.Models;
using JewelryStore.Services;
using JewelryStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Controllers
{
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class DBChangeringController : Controller
    {
        private readonly DataBaseContext dbContext;
        private readonly UserManager<UserModel> _userManager;

        private int displayedQuantity = 20;

        public DBChangeringController(DataBaseContext context, UserManager<UserModel> userManager)
        {
            dbContext = context;
            _userManager = userManager;
        }

        [Route("[action]")]
        public IActionResult DataBaseEditor()
        {
            dbContext.Jewelries.Load();

            List<JewelryModel> jewelries = dbContext.Jewelries.ToList();

            return View(new CardsViewModel(jewelries.OrderByDescending(x => x.ID).Take(displayedQuantity).ToList(), 1,
                (int)Math.Ceiling((decimal)jewelries.Count() / displayedQuantity)));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CreateNewData()
        {
            ViewBag.Kinds = new SelectList(await dbContext.JewelryKinds.ToListAsync(), "ID", "RuName");
            ViewBag.Discounts = new SelectList(await dbContext.Discounts.ToListAsync(), "ID", "Amount");

            await dbContext.Characteristics.Include(x => x.CharacteristicValues).LoadAsync();

            List<ItemCharacteristics> itemCharacteristics = new List<ItemCharacteristics>();
            List<CharacteristicValueModel> characteristicValues = null;
            foreach (var characteristic in dbContext.Characteristics.ToList())
            {

                if (characteristic.CharacteristicValues.Count() > 0)
                {
                    characteristicValues = dbContext.CharacteristicValues.Where(x => x.Characteristic.Name == characteristic.Name).ToList();
                    if (characteristicValues.Count() > 0)
                    {
                        itemCharacteristics.Add(new ItemCharacteristics(characteristic.Name, characteristicValues));
                    }
                }
            }

            return View(new ItemViewModel(new JewelryModel(), itemCharacteristics));
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewData(JewelryModel model, int[] characteristics)
        {
            if (ModelState.IsValid)
            {
                JewelryModel jewelry = dbContext.Jewelries.FirstOrDefault(x => x.Code == model.Code);
                if (jewelry == null)
                {
                    List<JewelryCharacteristicsModel> jewelryCharacteristics = new List<JewelryCharacteristicsModel>();
                    for (int i = 0; i < characteristics.Length; i++)
                    {
                        jewelryCharacteristics.Add(new JewelryCharacteristicsModel { ID_CharacteristicValue = characteristics[i] });
                    }
                    model.Code = model.Code.ToLower();
                    model.JewelryCharacteristics = jewelryCharacteristics;
                    model.Url = $"/catalog/{dbContext.JewelryKinds.FirstOrDefault(x => x.ID == model.ID_Kind).EnName}/{model.Code}";

                    await dbContext.Jewelries.AddAsync(model);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Указанный вами код/артикль уже используется!");

                    ViewBag.Kinds = new SelectList(await dbContext.JewelryKinds.ToListAsync(), "ID", "RuName");
                    ViewBag.Discounts = new SelectList(await dbContext.Discounts.ToListAsync(), "ID", "Amount");

                    return View(model);
                }
            }

            return RedirectToAction("DataBaseEditor");
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditData(int id)
        {
            await dbContext.Jewelries.Include(x => x.JewelryCharacteristics).LoadAsync();
            await dbContext.Characteristics.Include(x => x.CharacteristicValues).LoadAsync();

            JewelryModel jewelry = dbContext.Jewelries.FirstOrDefault(x => x.ID == id);

            List<SelectedItemCharacteristics> selectedItemCharacteristics = new List<SelectedItemCharacteristics>();
            List<CharacteristicValueModel> characteristicValues = await dbContext.CharacteristicValues.ToListAsync();
            List<string> characteristics = dbContext.Characteristics.Select(x => x.Name).ToList();
            foreach (var characteristic in characteristics)
            {
                if (characteristic.Count() > 0)
                {
                    if (characteristicValues.Where(x => x.Characteristic.Name == characteristic).ToList().Count() > 0)
                    {
                        selectedItemCharacteristics.Add(new SelectedItemCharacteristics(characteristic,
                            jewelry.JewelryCharacteristics.Where(x => x.CharacteristicValues.Characteristic.Name == characteristic).Select(x => x.CharacteristicValues.ID.ToString()).ToList(),
                            characteristicValues.Where(x => x.Characteristic.Name == characteristic).Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Value }).ToList()));
                    }
                }
            }

            SelectedKind selectedKind = new SelectedKind(jewelry.ID_Kind.ToString(), await dbContext.JewelryKinds.Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.RuName }).ToListAsync());
            SelectedDiscount selectedDiscount = new SelectedDiscount(jewelry.ID_Discount.ToString(), await dbContext.Discounts.Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Amount.ToString() }).ToListAsync());

            return View(new EditItemViewModel(jewelry, selectedItemCharacteristics, selectedKind, selectedDiscount));
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDataSave(JewelryModel model, int[] characteristics)
        {
            if (ModelState.IsValid)
            {
                List<JewelryCharacteristicsModel> jewelryCharacteristics = new List<JewelryCharacteristicsModel>();
                for (int i = 0; i < characteristics.Length; i++)
                {
                    jewelryCharacteristics.Add(new JewelryCharacteristicsModel { ID_CharacteristicValue = characteristics[i] });
                }

                JewelryModel jewelry = dbContext.Jewelries.FirstOrDefault(x => x.ID == model.ID);
                jewelry.ID_Kind = model.ID_Kind;
                jewelry.ID_Discount = model.ID_Discount;
                jewelry.Name = model.Name;
                jewelry.InsertedGemChar = model.InsertedGemChar;
                jewelry.Price = model.Price;
                jewelry.Quantity = model.Quantity;
                jewelry.Code = model.Code.ToLower();
                jewelry.JewelryCharacteristics = jewelryCharacteristics;
                jewelry.Url = $"/catalog/{dbContext.JewelryKinds.FirstOrDefault(x => x.ID == model.ID_Kind).EnName}/{model.Code}";

                dbContext.JewelryCharacteristics.RemoveRange(dbContext.JewelryCharacteristics.Where(x => x.ID_Jewelry == model.ID).ToList());

                dbContext.Jewelries.Update(jewelry);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("DataBaseEditor");
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteData(int id)
        {
            JewelryModel jewelry = dbContext.Jewelries.Include(x => x.CartContents).FirstOrDefault(x => x.ID == id);
            dbContext.Jewelries.Remove(jewelry);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("DataBaseEditor");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetJewelrysTable(int page = 1)
        {
            await dbContext.Jewelries.LoadAsync();

            List<JewelryModel> jewelries = await dbContext.Jewelries.ToListAsync();

            return PartialView("_JewelrysTable", new CardsViewModel(
                jewelries.OrderByDescending(x => x.ID).Skip(displayedQuantity * ((page - 1) < 0 ? 0 : page - 1)).Take(displayedQuantity).ToList(),
                page, (int)Math.Ceiling((decimal)jewelries.Count() / displayedQuantity)));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetCharacteristicsTable()
        {
            await dbContext.Characteristics.Include(x => x.CharacteristicValues).LoadAsync();

            return PartialView("_CharacteristicsTable", await dbContext.Characteristics.ToListAsync());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetJewelryKindsTable()
        {
            await dbContext.JewelryKinds.LoadAsync();

            return PartialView("_JewelryKinds", await dbContext.JewelryKinds.ToListAsync());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetDiscountsTable()
        {
            await dbContext.Discounts.LoadAsync();

            return PartialView("_DiscountsTable", await dbContext.Discounts.ToListAsync());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUsersTable()
        {
            await dbContext.Users.LoadAsync();

            return PartialView("_UsersTable", await dbContext.Users.ToListAsync());
        }
    }
}
