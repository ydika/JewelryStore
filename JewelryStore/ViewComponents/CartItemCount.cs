using JewelryStore.Models;
using JewelryStore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.ViewComponents
{
    public class CartItemCount : ViewComponent
    {
        private DataBaseContext dbContext;
        private readonly UserManager<UserModel> _userManager;

        public CartItemCount(DataBaseContext context, UserManager<UserModel> userManager)
        {
            dbContext = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View("CartItemCount", 0);
            }

            return View("CartItemCount", await dbContext.CartContent.Where(x => x.Cart.ID_User == _userManager.GetUserId((System.Security.Claims.ClaimsPrincipal)User)).CountAsync());
        }
    }
}
