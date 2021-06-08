﻿using JewelryStore.Models;
using JewelryStore.Services;
using JewelryStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
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
        public async Task<JsonResult> GetCart()
        {
            return Json(await dbContext.CartContent.Include(x => x.Cart).Include(x => x.Jewelry.Discount).Include(x => x.Jewelry.JewelrySizes).Where(x => x.Cart.ID_User == _userManager.GetUserId(User)).ToListAsync());
        }

        [HttpGet]
        [Authorize]
        [Route("[action]")]
        [ResponseCache(NoStore = true)]
        public IActionResult GetCartItemCount()
        {
            return ViewComponent("CartItemCount");
        }

        [HttpGet]
        [Authorize]
        [Route("[action]")]
        public async Task<JsonResult> ChangeQuantity(int jewelryid, int quantity)
        {
            CartContentModel coincidence = await dbContext.CartContent.Include(x => x.Cart).Include(x => x.Jewelry.Discount)
                .FirstOrDefaultAsync(x => x.Cart.ID_User == _userManager.GetUserId(User) && x.ID_Jewelry == jewelryid);
            if (coincidence != null)
            {
                coincidence.Quantity = quantity;
                coincidence.TotalPrice = Math.Round(coincidence.Quantity * double.Parse(coincidence.Jewelry.Price), 2).ToString("0.00");
                await dbContext.SaveChangesAsync();
            }

            return Json(await dbContext.CartContent.Include(x => x.Jewelry.Discount).Where(x => x.Cart.ID_User == _userManager.GetUserId(User)).OrderBy(x => x.ID).ToListAsync());
        }

        [HttpPost]
        [Authorize]
        [Route("[action]")]
        public async Task<IActionResult> Add(int jewelryid)
        {
            string userId = _userManager.GetUserId(User);

            JewelryModel jewelry = await dbContext.Jewelries.Include(x => x.Discount).FirstOrDefaultAsync(x => x.ID == jewelryid);
            CartModel cart = await dbContext.Cart.Include(x => x.CartContent).FirstOrDefaultAsync(x => x.ID_User == userId);
            string jewelryPrice = Math.Round(double.Parse(jewelry.Price), 2).ToString("0.00");

            if (cart != null)
            {
                CartContentModel coincidence = cart.CartContent.FirstOrDefault(x => x.ID_Jewelry == jewelry.ID);
                if (coincidence != null)
                {
                    coincidence.Quantity++;
                    coincidence.TotalPrice = Math.Round(coincidence.Quantity * double.Parse(coincidence.Jewelry.Price), 2).ToString("0.00");
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    await dbContext.CartContent.AddAsync(new CartContentModel(cart.ID, jewelry.ID, DateTime.Now, 1, jewelryPrice));
                    await dbContext.SaveChangesAsync();
                }
            }
            else
            {
                CartModel newCart = new CartModel(userId, DateTime.Now);
                await dbContext.Cart.AddAsync(newCart);
                await dbContext.SaveChangesAsync();

                dbContext.CartContent.Add(new CartContentModel(newCart.ID, jewelry.ID, DateTime.Now, 1, jewelryPrice));
                await dbContext.SaveChangesAsync();
            }

            return ViewComponent("CartItemCount");
        }

        [HttpPost]
        [Authorize]
        [Route("[action]")]
        public async Task<JsonResult> Remove(int jewelryid)
        {
            await dbContext.CartContent.Include(x => x.Cart).Include(x => x.Jewelry).LoadAsync();

            CartModel cart = await dbContext.Cart.FirstOrDefaultAsync(x => x.ID_User == _userManager.GetUserId(User));

            if (cart != null)
            {
                dbContext.CartContent.Remove(cart.CartContent.FirstOrDefault(x => x.ID_Jewelry == jewelryid));
                await dbContext.SaveChangesAsync();
            }

            return Json(await dbContext.CartContent.Include(x => x.Jewelry.Discount).Where(x => x.Cart.ID_User == _userManager.GetUserId(User)).OrderBy(x => x.ID).ToListAsync());
        }
    }
}
