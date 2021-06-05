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
using System.Globalization;
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

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> JewelrysTable(double minprice, double maxprice, int page = 1, string jname = "")
        {
            await dbContext.Jewelries.LoadAsync();

            List<JewelryModel> jewelries = await dbContext.Jewelries.ToListAsync();
            if (minprice != 0 && maxprice != 0 && maxprice > minprice)
            {
                jewelries = jewelries.Where(x => double.Parse(x.Price, CultureInfo.InvariantCulture) > minprice && double.Parse(x.Price, CultureInfo.InvariantCulture) < maxprice).ToList();
            }
            else
            {
                minprice = jewelries.Min(x => double.Parse(x.Price, CultureInfo.InvariantCulture));
                maxprice = jewelries.Max(x => double.Parse(x.Price, CultureInfo.InvariantCulture));
            }

            if (jname != null && jname != "")
            {
                jewelries = jewelries.Where(p => p.Name.ToUpper().Contains(jname.ToUpper())).ToList();
            }

            return View(new CardsViewModel(jewelries.OrderByDescending(x => x.ID).Skip(displayedQuantity * ((page - 1) < 0 ? 0 : page - 1)).Take(displayedQuantity).ToList(),
                page, (int)Math.Ceiling((decimal)jewelries.Count() / displayedQuantity), maxprice, minprice, jname));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CreateNewJewelry()
        {
            return View(await ShapingEditViewModel(new JewelryModel(), new int[] { }));
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewJewelry(JewelryModel jewelry, int[] characteristics)
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

            return RedirectToAction("JewelrysTable");
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditJewelryData(int id)
        {
            JewelryModel jewelry = dbContext.Jewelries.Include(x => x.JewelryCharacteristics).FirstOrDefault(x => x.ID == id);

            return View(await ShapingEditViewModel(jewelry, jewelry.JewelryCharacteristics.Select(x => x.ID_CharacteristicValue).ToArray()));
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditJewelryDataSave(JewelryModel jewelry, int[] characteristics)
        {
            if (ModelState.IsValid)
            {
                dbContext.JewelryCharacteristics.RemoveRange(dbContext.JewelryCharacteristics.Where(x => x.ID_Jewelry == jewelry.ID).ToList());

                jewelry = ShapingJewelryModel(jewelry, characteristics);
                dbContext.Jewelries.Add(jewelry);
                dbContext.Entry(jewelry).State = EntityState.Modified;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("JewelrysTable");
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
        public async Task<IActionResult> DeleteJewelry(int id)
        {
            JewelryModel jewelry = await dbContext.Jewelries.Include(x => x.CartContents).FirstOrDefaultAsync(x => x.ID == id);
            dbContext.Jewelries.Remove(jewelry);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("JewelrysTable");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CharacteristicsTable()
        {
            return View(await dbContext.Characteristics.Include(x => x.CharacteristicValues).OrderByDescending(x => x.ID).ToListAsync());
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCharacteristic(string name)
        {
            await dbContext.Characteristics.AddAsync(new CharacteristicsModel { Name = name });
            await dbContext.SaveChangesAsync();

            return RedirectToAction("CharacteristicsTable");
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCharacteristicValue(int characteristic, string name)
        {
            await dbContext.CharacteristicValues.AddAsync(new CharacteristicValueModel { ID_Characteristic = characteristic, Value = name });
            await dbContext.SaveChangesAsync();

            return RedirectToAction("CharacteristicsTable");
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCharacteristic(int characteristic)
        {
            CharacteristicsModel charact = await dbContext.Characteristics.Include(x => x.CharacteristicValues).FirstOrDefaultAsync(x => x.ID == characteristic);
            dbContext.CharacteristicValues.RemoveRange(charact.CharacteristicValues.ToList());
            dbContext.Remove(charact);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("CharacteristicsTable");
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCharacteristicValues(int[] characteristics)
        {
            for (int i = 0; i < characteristics.Length; i++)
            {
                dbContext.CharacteristicValues.RemoveRange(dbContext.CharacteristicValues.Where(x => x.ID == characteristics[i]).ToList());
            }
            await dbContext.SaveChangesAsync();

            return RedirectToAction("CharacteristicsTable");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> JewelryKindsTable()
        {
            await dbContext.JewelryKinds.LoadAsync();

            return View(await dbContext.JewelryKinds.ToListAsync());
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult CreateJewelryKind()
        {
            return View(new JewelryKindsModel());
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateJewelryKind(JewelryKindsModel kind, int[] characteristics)
        {
            if (ModelState.IsValid)
            {
                JewelryKindsModel coincidence = dbContext.JewelryKinds.FirstOrDefault(x => x.EnName == kind.EnName || x.RuName == kind.RuName);
                if (coincidence == null)
                {
                    kind.EnName = kind.EnName.Replace(' ', '-').ToLower();

                    await dbContext.JewelryKinds.AddAsync(kind);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Указанный вами код/артикль уже используется!");

                    return View(kind);
                }
            }

            return RedirectToAction("JewelryKindsTable");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> EditJewelryKind(int id)
        {
            return View(await dbContext.JewelryKinds.FirstOrDefaultAsync(x => x.ID == id));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> EditJewelryKindSave(JewelryKindsModel kind)
        {
            if (ModelState.IsValid)
            {
                kind.EnName = kind.EnName.Replace(' ', '-').ToLower();

                dbContext.JewelryKinds.Add(kind);
                dbContext.Entry(kind).State = EntityState.Modified;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("JewelryKindsTable");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> DeleteJewelryKind(int id)
        {
            JewelryKindsModel kind = await dbContext.JewelryKinds.FirstOrDefaultAsync(x => x.ID == id);
            dbContext.JewelryKinds.Remove(kind);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("JewelryKindsTable");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> DiscountsTable()
        {
            await dbContext.Discounts.LoadAsync();

            return View(await dbContext.Discounts.ToListAsync());
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult CreateDiscount()
        {
            return View(new DiscountModel());
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateDiscount(DiscountModel discount)
        {
            if (ModelState.IsValid)
            {
                DiscountModel coincidence = dbContext.Discounts.FirstOrDefault(x => x.Amount == discount.Amount);
                if (coincidence == null)
                {
                    await dbContext.Discounts.AddAsync(discount);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Указанный размер скидки уже существует!");

                    return View(discount);
                }
            }

            return RedirectToAction("DiscountsTable");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> EditDiscount(int id)
        {
            return View(await dbContext.Discounts.FirstOrDefaultAsync(x => x.ID == id));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> EditDiscountSave(DiscountModel discount)
        {
            if (ModelState.IsValid)
            {
                DiscountModel coincidence = dbContext.Discounts.FirstOrDefault(x => x.Amount == discount.Amount);
                if (coincidence == null)
                {
                    dbContext.Discounts.Add(discount);
                    dbContext.Entry(discount).State = EntityState.Modified;

                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Указанный размер скидки уже существует!");

                    return View("EditDiscount", discount);
                }
            }

            return RedirectToAction("DiscountsTable");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            List<JewelryModel> jewelries = dbContext.Jewelries.Where(x => x.ID_Discount == id).ToList();
            foreach (var j in jewelries)
            {
                j.ID_Discount = 1;
            }
            dbContext.Jewelries.UpdateRange(jewelries);

            DiscountModel discount = await dbContext.Discounts.FirstOrDefaultAsync(x => x.ID == id);
            dbContext.Discounts.Remove(discount);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("DiscountsTable");
        }
    }
}