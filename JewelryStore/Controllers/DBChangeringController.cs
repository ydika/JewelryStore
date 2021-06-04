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
            return View(await ShapingEditViewModel(new JewelryModel(), new int[] { }));
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewData(JewelryModel jewelry, int[] characteristics)
        {
            if (ModelState.IsValid)
            {
                JewelryModel coincidence = dbContext.Jewelries.FirstOrDefault(x => x.Code == jewelry.Code);
                if (coincidence == null)
                {
                    jewelry = ShapingJewelryModel(jewelry, characteristics);

                    await dbContext.Jewelries.AddAsync(jewelry);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Указанный вами код/артикль уже используется!");

                    return View(await ShapingEditViewModel(jewelry, characteristics));
                }
            }

            return RedirectToAction("DataBaseEditor");
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditData(int id)
        {
            JewelryModel jewelry = dbContext.Jewelries.Include(x => x.JewelryCharacteristics).FirstOrDefault(x => x.ID == id);

            return View(await ShapingEditViewModel(jewelry, jewelry.JewelryCharacteristics.Select(x => x.ID_CharacteristicValue).ToArray()));
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDataSave(JewelryModel jewelry, int[] characteristics)
        {
            if (ModelState.IsValid)
            {
                dbContext.JewelryCharacteristics.RemoveRange(dbContext.JewelryCharacteristics.Where(x => x.ID_Jewelry == jewelry.ID).ToList());

                jewelry = ShapingJewelryModel(jewelry, characteristics);
                dbContext.Jewelries.Add(jewelry);
                dbContext.Entry(jewelry).State = EntityState.Modified;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("DataBaseEditor");
        }

        [NonAction]
        private JewelryModel ShapingJewelryModel(JewelryModel jewelry, int[] characteristics)
        {
            List<JewelryCharacteristicsModel> jewelryCharacteristics = new List<JewelryCharacteristicsModel>();

            for (int i = 0; i < characteristics.Length; i++)
            {
                jewelryCharacteristics.Add(new JewelryCharacteristicsModel { ID_CharacteristicValue = characteristics[i] });
            }

            jewelry.Code = jewelry.Code.ToLower();
            jewelry.JewelryCharacteristics = jewelryCharacteristics;
            jewelry.Url = $"/catalog/{dbContext.JewelryKinds.FirstOrDefault(x => x.ID == jewelry.ID_Kind).EnName}/{jewelry.Code}";

            return jewelry;
        }

        [NonAction]
        private async Task<EditItemViewModel> ShapingEditViewModel(JewelryModel jewelry, int[] characteristics)
        {
            List<SelectedItemCharacteristics> selectedItemCharacteristics = new List<SelectedItemCharacteristics>();
            List<CharacteristicValueModel> dbCharacteristicValues = await dbContext.CharacteristicValues.Include(x => x.Characteristic).ToListAsync();
            List<CharacteristicValueModel> characteristicValues = null;
            List<CharacteristicValueModel> receivedCharacteristics = null;
            List<string> characteristicsNames = dbCharacteristicValues.Select(x => x.Characteristic.Name).Distinct().ToList();

            foreach (var characteristic in characteristicsNames)
            {
                characteristicValues = dbCharacteristicValues.Where(x => x.Characteristic.Name == characteristic).ToList();
                if (characteristicValues.Count() > 0)
                {
                    if (jewelry.JewelryCharacteristics == null)
                    {
                        if (characteristics.Length > 0)
                        {
                            receivedCharacteristics = new List<CharacteristicValueModel>();
                            for (int i = 0; i < characteristics.Length; i++)
                            {
                                receivedCharacteristics.Add(dbCharacteristicValues.FirstOrDefault(x => x.ID == characteristics[i]));
                            }
                            selectedItemCharacteristics.Add(new SelectedItemCharacteristics(characteristic, receivedCharacteristics.Select(x => x.ID.ToString()).ToList(),
                                characteristicValues.Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Value }).ToList()));
                        }
                        else
                        {
                            selectedItemCharacteristics.Add(new SelectedItemCharacteristics(characteristic, new List<string>(),
                                characteristicValues.Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Value }).ToList()));
                        }
                    }
                    else
                    {
                        selectedItemCharacteristics.Add(new SelectedItemCharacteristics(characteristic,
                            jewelry.JewelryCharacteristics.Where(x => x.CharacteristicValues.Characteristic.Name == characteristic).Select(x => x.CharacteristicValues.ID.ToString()).ToList(),
                            characteristicValues.Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Value }).ToList()));
                    }
                }
            }

            SelectedKind selectedKind = new SelectedKind(jewelry.ID_Kind.ToString(),
                await dbContext.JewelryKinds.Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.RuName }).ToListAsync());
            SelectedDiscount selectedDiscount = new SelectedDiscount(jewelry.ID_Discount.ToString(),
                await dbContext.Discounts.Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Amount.ToString() }).ToListAsync());

            return new EditItemViewModel(jewelry, selectedItemCharacteristics, selectedKind, selectedDiscount);
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
