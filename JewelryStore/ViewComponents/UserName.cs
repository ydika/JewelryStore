using JewelryStore.Models;
using JewelryStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.ViewComponents
{
    public class UserName : ViewComponent
    {
        private readonly UserManager<UserModel> _userManager;

        public UserName(UserManager<UserModel> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            UserModel appUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            string displayName = appUser?.FirstName;

            return View("UserName", displayName);
        }
    }
}
