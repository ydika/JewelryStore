﻿using JewelryStore.Models;
using JewelryStore.Services;
using JewelryStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        private readonly IWebHostEnvironment _appEnvironment;

        private int displayedQuantity = 20;

        public DBChangeringController(DataBaseContext context, UserManager<UserModel> userManager, IWebHostEnvironment appEnvironment)
        {
            dbContext = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> JewelrysTable(double minprice, double maxprice, int page = 1, string jname = "")
        {
            List<JewelryModel> jewelries = await dbContext.Jewelries.Include(x => x.Discount).ToListAsync();
            if (minprice != 0 && maxprice != 0 && maxprice >= minprice)
            {
                jewelries = jewelries.Where(x => double.Parse(x.Price) >= minprice && double.Parse(x.Price) <= maxprice).ToList();
            }
            else
            {
                minprice = jewelries.Min(x => double.Parse(x.Price));
                maxprice = jewelries.Max(x => double.Parse(x.Price));
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
        public async Task<IActionResult> CreateNewJewelry(JewelryModel jewelry, int[] characteristics, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                JewelryModel coincidence = dbContext.Jewelries.Include(x => x.Discount).FirstOrDefault(x => x.Code == jewelry.Code);
                if (coincidence == null)
                {
                    jewelry = ShapingJewelryModel(jewelry, characteristics);
                    List<CharacteristicValueModel> characteristicValues = new List<CharacteristicValueModel>();
                    for (int i = 0; i < characteristics.Length; i++)
                    {
                        characteristicValues.Add(dbContext.CharacteristicValues.Include(x => x.Characteristic).FirstOrDefault(x => x.ID == characteristics[i]));
                    }
                    characteristicValues = characteristicValues.Where(x => x.Characteristic.Name == "Размер").ToList();
                    List<JewelrySizeModel> jewelrySizes = new List<JewelrySizeModel>();
                    foreach (var item in characteristicValues)
                    {
                        jewelrySizes.Add(new JewelrySizeModel(jewelry.ID, item.Value, ""));
                    }
                    jewelry.JewelrySizes = jewelrySizes;

                    string path = "";
                    if (image != null)
                    {
                        string dir = $"/images/jewelrys/{dbContext.Subspecies.Include(x => x.Kind).FirstOrDefault(x => x.ID == jewelry.ID_Subspecies).Kind.EnName}";
                        path = $"{dir}/{image.FileName}";
                        if (!Directory.Exists(_appEnvironment.WebRootPath + dir)) Directory.CreateDirectory(_appEnvironment.WebRootPath + dir);
                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }
                        FilesModel file = new FilesModel { Name = image.FileName, Path = path };

                        jewelry.ImageSrc = path;

                        await dbContext.Files.AddAsync(file);
                        await dbContext.SaveChangesAsync();
                    }

                    await dbContext.Jewelries.AddAsync(jewelry);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Указанный вами код/артикль уже используется!");

                    return View(await ShapingEditViewModel(jewelry, characteristics));
                }
            }

            ModelState.AddModelError(string.Empty, "Запись успешно добавлена!");
            return View(await ShapingEditViewModel(new JewelryModel(), characteristics));
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditJewelryData(int id)
        {
            JewelryModel jewelry = dbContext.Jewelries.Include(x => x.JewelryCharacteristics).Include(x => x.Subspecies).Include(x => x.JewelrySizes).FirstOrDefault(x => x.ID == id);

            return View(await ShapingEditViewModel(jewelry, jewelry.JewelryCharacteristics.Select(x => x.ID_CharacteristicValue).ToArray()));
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditJewelryDataSave(JewelryModel jewelry, int[] characteristics, string[] sizePrice, string[] sizeValue)
        {
            if (ModelState.IsValid)
            {
                List<JewelrySizeModel> jewelrySizes = new List<JewelrySizeModel>();
                for (int i = 0; i < sizePrice.Length; i++)
                {
                    jewelrySizes.Add(new JewelrySizeModel(jewelry.ID, sizeValue[i], sizePrice[i]));
                }

                dbContext.JewelrySizes.RemoveRange(dbContext.JewelrySizes.Where(x => x.ID_Jewelry == jewelry.ID));
                dbContext.JewelrySizes.AddRange(jewelrySizes);

                if (dbContext.Jewelries.Where(x => x.Code == jewelry.Code).Count() > 1)
                {
                    ModelState.AddModelError(string.Empty, "Указанный вами код/артикль уже используется!");
                    return View("EditJewelryData", await ShapingEditViewModel(jewelry, characteristics));
                }

                dbContext.JewelryCharacteristics.RemoveRange(dbContext.JewelryCharacteristics.Where(x => x.ID_Jewelry == jewelry.ID).ToList());

                int q = jewelry.Quantity;
                jewelry = ShapingJewelryModel(jewelry, characteristics);
                jewelry.Quantity = q;
                dbContext.Jewelries.Add(jewelry);
                dbContext.Entry(jewelry).State = EntityState.Modified;

                for (int i = 0; i < sizePrice.Length; i++)
                {
                    if (sizePrice[i] == "" || sizePrice[i] == null)
                    {
                        ModelState.AddModelError(string.Empty, "Укажите цены для каждого размера!");
                        return View("EditJewelryData", await ShapingEditViewModel(jewelry, characteristics));
                    }
                }

                await dbContext.SaveChangesAsync();
                ModelState.AddModelError(string.Empty, "Запись успешно обновлена!");
            }

            return View("EditJewelryData", await ShapingEditViewModel(jewelry, characteristics));
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
            SubspeciesModel subspecies = dbContext.Subspecies.Include(x => x.Kind).FirstOrDefault(x => x.ID == jewelry.ID_Subspecies);
            jewelry.Url = $"/catalog/{subspecies.Kind.EnName}/{subspecies.EnName}/{jewelry.Code}";

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

            List<SubspeciesModel> subspecies = await dbContext.Subspecies.ToListAsync();
            SelectedKind selectedKind = new SelectedKind(jewelry.ID_Subspecies.ToString(),
                subspecies.Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.RuName }).ToList());
            SelectedDiscount selectedDiscount = new SelectedDiscount(jewelry.ID_Discount.ToString(),
                await dbContext.Discounts.Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Amount.ToString() }).ToListAsync());

            return new EditItemViewModel(jewelry, selectedItemCharacteristics, selectedKind, selectedDiscount);
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteJewelry(int id)
        {
            JewelryModel jewelry = await dbContext.Jewelries.Include(x => x.CartContents).Include(x => x.Discount).FirstOrDefaultAsync(x => x.ID == id);
            if (dbContext.OrderContents.FirstOrDefault(x => x.ID_Jewelry == jewelry.ID) == null)
            {
                dbContext.Jewelries.Remove(jewelry);
            }
            else
            {
                jewelry.Quantity = 0;
                dbContext.Jewelries.Add(jewelry);
                dbContext.Entry(jewelry).State = EntityState.Modified;
            }
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
            return View(await dbContext.JewelryKinds.Include(x => x.Subspecies).FirstOrDefaultAsync(x => x.ID == id));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> EditJewelryKindSave(JewelryKindsModel kind, string[] sRuName, string[] sEnName, int[] sDelete)
        {
            if (ModelState.IsValid)
            {
                if (sDelete.Length > 0)
                {
                    for (int i = 0; i < sDelete.Length; i++)
                    {
                        dbContext.Subspecies.RemoveRange(dbContext.Subspecies.Where(x => x.ID == sDelete[i]).ToList());
                    }
                }

                List<SubspeciesModel> subspecies = new List<SubspeciesModel>();
                for (int i = 0; i < sRuName.Length; i++)
                {
                    if (sRuName[i] != null && sEnName[i] != null)
                    {
                        subspecies.Add(new SubspeciesModel { ID_Kind = kind.ID, RuName = sRuName[i], EnName = sEnName[i] });
                    }
                }

                kind.EnName = kind.EnName.Replace(' ', '-').ToLower();
                dbContext.Subspecies.AddRange(subspecies);
                dbContext.JewelryKinds.Add(kind);
                dbContext.Entry(kind).State = EntityState.Modified;

                await dbContext.SaveChangesAsync();
            }

            ModelState.AddModelError(string.Empty, "Данные успешно обновлены");

            return View("EditJewelryKind", await dbContext.JewelryKinds.Include(x => x.Subspecies).FirstOrDefaultAsync(x => x.ID == kind.ID));
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

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> OrdersTable()
        {
            await dbContext.Jewelries.LoadAsync();

            List<OrdersModel> orders = await dbContext.Orders.Include(x => x.OrderContents).OrderByDescending(x => x.ID).Take(20).ToListAsync();
            foreach (var order in orders)
            {
                foreach (var content in order.OrderContents)
                {
                    content.TotalPrice = double.Parse(content.TotalPrice, CultureInfo.InvariantCulture).ToString("0.00");
                }
            }

            return View(orders);
        }
    }
}