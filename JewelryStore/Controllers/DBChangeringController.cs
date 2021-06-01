using JewelryStore.Models;
using JewelryStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public DBChangeringController(DataBaseContext context, UserManager<UserModel> userManager)
        {
            dbContext = context;
            _userManager = userManager;
        }

        [Route("[action]")]
        public IActionResult DataBaseEditor()
        {
            dbContext.Jewelries.Load();

            return View(dbContext.Jewelries.ToList());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetJewelrysTable()
        {
            await dbContext.Jewelries.LoadAsync();

            return PartialView("_JewelrysTable", await dbContext.Jewelries.ToListAsync());
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
