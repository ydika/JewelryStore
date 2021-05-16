﻿using JewelryStore.Models;
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

        private int displayedQuantity = 12;

        public CatalogController(DataBaseContext context)
        {
            dbContext = context;
        }

        [Route("{jkind}")]
        public IActionResult List(string jkind)
        {
            dbContext.JewelryKinds.Load();

            ViewData["Title"] = "Ювелирные изделия купить";
            if (jkind != null && jkind != "all")
            {
                ViewData["Title"] = dbContext.JewelryKinds.Where(x => x.EnName == jkind).Select(x => x.RuName).FirstOrDefault() + " купить";
            }

            return View(dbContext.JewelryKinds.ToList());
        }

        [Route("{jkind}/{code}")]
        public IActionResult Item(string code)
        {
            dbContext.Jewelries.Include(x => x.Discount).Load();
            dbContext.JewelryCharacteristics.Load();
            dbContext.Characteristics.Include(x => x.CharacteristicValues).Load();

            JewelryModel jewelry = dbContext.Jewelries.Where(x => x.Code == code).FirstOrDefault();
            List<ItemCharacteristicsViewModel> itemCharacteristics = new List<ItemCharacteristicsViewModel>();
            List<CharacteristicValuesModel> characteristicValues = null;
            foreach (var characteristic in dbContext.Characteristics.ToList())
            {
                if (characteristic.CharacteristicValues.Count() > 0)
                {
                    characteristicValues = jewelry.JewelryCharacteristics.Select(x => x.CharacteristicValues).Where(x => x.Characteristic.Name == characteristic.Name).ToList();
                    if (characteristicValues.Count() > 0)
                    {
                        itemCharacteristics.Add(new ItemCharacteristicsViewModel(characteristic.Name,
                                        jewelry.JewelryCharacteristics.Select(x => x.CharacteristicValues).Where(x => x.Characteristic.Name == characteristic.Name).ToList()));  
                    }
                }
            }

            return View(new ItemViewModel(jewelry, itemCharacteristics));
        }

        [HttpGet]
        [Route("[action]")]
        public JsonResult GetCatalogFilter()
        {
            dbContext.Characteristics.Include(x => x.CharacteristicValues).Load();

            return Json(dbContext.Characteristics.Where(x => x.CharacteristicValues.Count() > 0).ToList());
        }

        [HttpGet]
        [Route("{jkind}/[action]")]
        public JsonResult GetJewelriesCards(string[] o, string jkind = "all", int page = 1)
        {
            dbContext.Jewelries.Include(x => x.Kind).Include(x => x.Discount).Include(x => x.JewelryCharacteristics).Load();
            dbContext.JewelryCharacteristics.Include(x => x.CharacteristicValues).Load();

            List<JewelryModel> jewelries = dbContext.Jewelries.ToList();

            if (jkind != null && jkind != "all")
            {
                jewelries = jewelries.Where(x => x.Kind.EnName.ToLower() == jkind).ToList();
            }

            for (int i = 0; i < (o != null ? o.Length : 0); i++)
            {
                jewelries = jewelries.Where(x => x.JewelryCharacteristics.Select(x => x.CharacteristicValues.Value).Contains(o[i])).ToList();
            }

            int pageCount = (int)Math.Ceiling((decimal)jewelries.Count() / displayedQuantity);

            jewelries = jewelries.Skip(displayedQuantity * ((page - 1) < 0 ? 0 : page - 1)).Take(displayedQuantity).ToList();

            return Json(new CardsViewModel(jewelries, page, pageCount));
        }
    }
}