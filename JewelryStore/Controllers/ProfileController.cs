using JewelryStore.Models;
using JewelryStore.Services;
using JewelryStore.ViewModels;
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
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly DataBaseContext dbContext;
        private readonly UserManager<UserModel> _userManager;

        public ProfileController(DataBaseContext context, UserManager<UserModel> userManager)
        {
            dbContext = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Profile()
        {
            UserModel user = await _userManager.GetUserAsync(User);
            ChangeAccountDataViewModel change = new ChangeAccountDataViewModel
            {
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                Email = user.Email
            };

            return View(change);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetProfile()
        {
            UserModel user = await _userManager.GetUserAsync(User);
            ChangeAccountDataViewModel change = new ChangeAccountDataViewModel
            {
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                Email = user.Email
            };

            return PartialView("_Main", change);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ChangeAccountData(ChangeAccountDataViewModel model = null)
        {
            if (ModelState.IsValid)
            {
                UserModel user = await _userManager.GetUserAsync(User);
                if (user.FirstName != model.FirstName || user.SecondName != model.SecondName)
                {
                    user.FirstName = model.FirstName;
                    user.SecondName = model.SecondName;
                    await _userManager.UpdateAsync(user); 
                }
                if (model.Password != null)
                {
                    IdentityResult result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Кажется что-то пошло не так");
            }

            return RedirectToAction("Profile");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetOrders()
        {
            await dbContext.Jewelries.LoadAsync();
            List<OrdersModel> userOrders = await dbContext.Orders.Include(x => x.OrderContents).Where(x => x.ID_User == _userManager.GetUserId(User)).OrderByDescending(x => x.ID).ToListAsync();

            return PartialView("_Order", userOrders);
        }
    }
}