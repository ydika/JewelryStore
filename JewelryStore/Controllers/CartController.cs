using JewelryStore.Models;
using JewelryStore.Services;
using JewelryStore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [Route("[action]")]
        public IActionResult Cart()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public void Add(int jewelryid)
        {
            JewelryModel jewelry = dbContext.Jewelries.FirstOrDefault(x => x.ID == jewelryid);

            var coincidence = dbContext.Cart.FirstOrDefault(x => x.ID_User == _userManager.GetUserId(User) && x.ID_Jewelry == jewelryid);
            if (coincidence != null)
            {
                coincidence.Quantity++;
            }
            else
            {
                dbContext.Cart.Add(new CartModel(_userManager.GetUserId(User), jewelry.ID, DateTime.Now, 1));
            }
            dbContext.SaveChanges();
        }

        [HttpPost]
        [Route("[action]")]
        public void Remove(int jewelryid)
        {
            JewelryModel jewelry = dbContext.Jewelries.FirstOrDefault(x => x.ID == jewelryid);

            if (jewelry != null) dbContext.Remove(jewelry);
        }
    }
}
