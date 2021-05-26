using JewelryStore.Models;
using JewelryStore.Services;
using JewelryStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JewelryStore.Controllers
{
    [Route("[controller]")]
    public class CartController : Controller
    {
        private DataBaseContext dbContext;
        private readonly UserManager<UserModel> _userManager;

        public CartController(DataBaseContext context, UserManager<UserModel> userManager)
        {
            dbContext = context;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Cart()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        [Route("[action]")]
        [ResponseCache(NoStore = true)]
        public JsonResult GetCart()
        {
            dbContext.Cart.Include(x => x.Jewelry).Load();
            return Json(dbContext.Cart.Where(x => x.ID_User == _userManager.GetUserId(User)).OrderBy(x => x.ID).ToList());
        }

        [HttpGet]
        [Authorize]
        [Route("[action]")]
        public async Task<JsonResult> ChangeQuantity(int jewelryid, int quantity)
        {
            await dbContext.Cart.Include(x => x.Jewelry).LoadAsync();

            CartModel coincidence = dbContext.Cart.FirstOrDefault(x => x.ID_User == _userManager.GetUserId(User) && x.ID_Jewelry == jewelryid);
            if (coincidence != null)
            {
                coincidence.Quantity = quantity;
                coincidence.TotalPrice = coincidence.Quantity * coincidence.Jewelry.Price;
                await dbContext.SaveChangesAsync();
            }
            
            return Json(dbContext.Cart.Where(x => x.ID_User == _userManager.GetUserId(User)).OrderBy(x => x.ID).ToList());
        }

        [HttpPost]
        [Authorize]
        [Route("[action]")]
        public async Task<JsonResult> Add(int jewelryid)
        {
            await dbContext.Cart.Include(x => x.Jewelry).LoadAsync();

            JewelryModel jewelry = dbContext.Jewelries.FirstOrDefault(x => x.ID == jewelryid);

            CartModel coincidence = dbContext.Cart.FirstOrDefault(x => x.ID_User == _userManager.GetUserId(User) && x.ID_Jewelry == jewelryid);
            if (coincidence != null)
            {
                coincidence.Quantity++;
                coincidence.TotalPrice = coincidence.Quantity * coincidence.Jewelry.Price;
                await dbContext.SaveChangesAsync();
            }
            else
            {
                dbContext.Cart.Add(new CartModel(_userManager.GetUserId(User), jewelry.ID, DateTime.Now, 1, jewelry.Price));
                await dbContext.SaveChangesAsync();
            }

            return Json(dbContext.Cart.Where(x => x.ID_User == _userManager.GetUserId(User)).OrderBy(x => x.ID).ToList());
        }

        [HttpPost]
        [Authorize]
        [Route("[action]")]
        public async Task<JsonResult> Remove(int jewelryid)
        {
            await dbContext.Cart.Include(x => x.Jewelry).LoadAsync();

            CartModel coincidence = dbContext.Cart.FirstOrDefault(x => x.ID_User == _userManager.GetUserId(User) && x.ID_Jewelry == jewelryid);

            if (coincidence != null)
            { 
                dbContext.Cart.Remove(coincidence);
                await dbContext.SaveChangesAsync();
            }

            return Json(dbContext.Cart.Where(x => x.ID_User == _userManager.GetUserId(User)).OrderBy(x => x.ID).ToList());
        }
    }
}
