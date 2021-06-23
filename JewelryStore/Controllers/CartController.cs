using JewelryStore.Models;
using JewelryStore.Services;
using JewelryStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
            List<CartContentModel> cartContents = await dbContext.CartContent.Include(x => x.Cart).Include(x => x.Jewelry.Discount).Include(x => x.Jewelry.JewelrySizes)
                .Where(x => x.Cart.ID_User == _userManager.GetUserId(User)).ToListAsync();

            foreach (var cartContent in cartContents)
            {
                if (cartContent.Jewelry.Quantity >= 5)
                {
                    cartContent.Jewelry.Quantity = 5;
                }
                if (cartContent.Size != "")
                {
                    cartContent.Jewelry.Price = cartContent.Jewelry.JewelrySizes.FirstOrDefault(x => x.Size == cartContent.Size).Price;
                }
            }

            return Json(cartContents);
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
            CartContentModel coincidence = await dbContext.CartContent.Include(x => x.Cart).Include(x => x.Jewelry.Discount).Include(x => x.Jewelry.JewelrySizes)
                .FirstOrDefaultAsync(x => x.Cart.ID_User == _userManager.GetUserId(User) && x.ID_Jewelry == jewelryid);
            string jewelryPrice = "";
            if (coincidence != null)
            {
                if (coincidence.Size != "")
                {
                    JewelrySizeModel firstJewelry = coincidence.Jewelry.JewelrySizes.FirstOrDefault(x => x.Size == coincidence.Size);
                    jewelryPrice = firstJewelry.Price;
                    coincidence.Quantity = quantity;
                    coincidence.TotalPrice = Math.Round(coincidence.Quantity * double.Parse(jewelryPrice, CultureInfo.InvariantCulture) * (1 - (double)firstJewelry.Jewelry.Discount.Amount / 100), 2).ToString("0.00");
                }
                else
                {
                    coincidence.Quantity = quantity;
                    coincidence.TotalPrice = Math.Round(coincidence.Quantity * double.Parse(coincidence.Jewelry.Price), 2).ToString("0.00");
                }
                await dbContext.SaveChangesAsync();
            }

            List<CartContentModel> cartContents = await dbContext.CartContent.Include(x => x.Jewelry.Discount).Include(x => x.Jewelry.JewelrySizes)
                .Where(x => x.Cart.ID_User == _userManager.GetUserId(User)).OrderBy(x => x.ID).ToListAsync();
            for (int i = 0; i < cartContents.Count(); i++)
            {
                if (cartContents[i].Jewelry.Quantity >= 5)
                {
                    cartContents[i].Jewelry.Quantity = 5;
                }
                if (cartContents[i].Size != "")
                {
                    cartContents[i].Jewelry.Price = cartContents[i].Jewelry.JewelrySizes.FirstOrDefault(x => x.Size == cartContents[i].Size).Price;
                }
            }

            return Json(cartContents);
        }

        [HttpPost]
        [Authorize]
        [Route("[action]")]
        public async Task<IActionResult> Add(int jewelryid, int quantity, string size)
        {
            string userId = _userManager.GetUserId(User);

            JewelryModel jewelry = await dbContext.Jewelries.Include(x => x.Discount).Include(x => x.JewelrySizes).FirstOrDefaultAsync(x => x.ID == jewelryid);
            CartModel cart = await dbContext.Cart.Include(x => x.CartContent).FirstOrDefaultAsync(x => x.ID_User == userId);

            string jewelryPrice = "";
            if (size != null)
            {
                JewelrySizeModel firstSize = jewelry.JewelrySizes.FirstOrDefault(x => x.Size == size);
                if (firstSize != null)
                {
                    jewelryPrice = (double.Parse(firstSize.Price, CultureInfo.InvariantCulture) * (1 - (double)jewelry.Discount.Amount / 100)).ToString("0.00");
                }
            }
            else
            {
                jewelryPrice = Math.Round(double.Parse(jewelry.Price), 2).ToString("0.00");
            }

            if (cart != null)
            {
                CartContentModel coincidence = cart.CartContent.FirstOrDefault(x => x.ID_Jewelry == jewelry.ID);
                if (coincidence != null)
                {
                    if (coincidence.Quantity++ <= coincidence.Jewelry.Quantity && coincidence.Quantity++ <= 5)
                    {
                        coincidence.Quantity++;
                        coincidence.TotalPrice = Math.Round(coincidence.Quantity * double.Parse(coincidence.Jewelry.Price), 2).ToString("0.00");
                        await dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    if (quantity > 0)
                    {
                        await dbContext.CartContent.AddAsync(new CartContentModel(cart.ID, jewelry.ID, DateTime.Now, quantity, size ?? ""
                            , Math.Round(double.Parse(jewelryPrice) * quantity, 2).ToString("0.00")));
                    }
                    else
                    {
                        await dbContext.CartContent.AddAsync(new CartContentModel(cart.ID, jewelry.ID, DateTime.Now, 1, size ?? "", jewelryPrice));
                    }
                    await dbContext.SaveChangesAsync();
                }
            }
            else
            {
                CartModel newCart = new CartModel(userId, DateTime.Now);
                await dbContext.Cart.AddAsync(newCart);
                await dbContext.SaveChangesAsync();

                if (quantity > 0)
                {
                    await dbContext.CartContent.AddAsync(new CartContentModel(newCart.ID, jewelry.ID, DateTime.Now, quantity, size ?? ""
                        , Math.Round(double.Parse(jewelryPrice) * quantity, 2).ToString("0.00")));
                }
                else
                {
                    await dbContext.CartContent.AddAsync(new CartContentModel(newCart.ID, jewelry.ID, DateTime.Now, 1, size ?? "", jewelryPrice));
                }
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

            List<CartContentModel> cartContents = await dbContext.CartContent.Include(x => x.Jewelry.Discount).Include(x => x.Jewelry.JewelrySizes)
                .Where(x => x.Cart.ID_User == _userManager.GetUserId(User)).OrderBy(x => x.ID).ToListAsync();
            for (int i = 0; i < cartContents.Count(); i++)
            {
                if (cartContents[i].Size != "")
                {
                    cartContents[i].Jewelry.Price = cartContents[i].Jewelry.JewelrySizes.FirstOrDefault(x => x.Size == cartContents[i].Size).Price.Replace(',', '.');
                }
            }

            return Json(cartContents);
        }

        [HttpPost]
        [Authorize]
        [Route("[action]")]
        public async Task<IActionResult> Purchase()
        {
            string purchaseCode = "";
            string userId = _userManager.GetUserId(User);
            List<JewelryModel> jewelries = await dbContext.Jewelries.Include(x => x.JewelrySizes).Include(x => x.Discount).ToListAsync();
            CartModel cart = await dbContext.Cart.Include(x => x.CartContent).FirstOrDefaultAsync(x => x.ID_User == userId);

            JewelryModel jewelry = null;
            bool noJewelry = false;
            foreach (var cartContent in cart.CartContent)
            {
                jewelry = jewelries.FirstOrDefault(x => x.ID == cartContent.ID_Jewelry);
                if (cartContent.Quantity > jewelry.Quantity)
                {
                    noJewelry = true;
                    if (jewelry.Quantity == 0)
                    {
                        dbContext.CartContent.Remove(cartContent);
                    }
                    else
                    {
                        cartContent.Quantity = jewelry.Quantity;
                        dbContext.Cart.Add(cart);
                        dbContext.Entry(cart).State = EntityState.Modified;
                    }
                }
            }
            await dbContext.SaveChangesAsync();
            if (noJewelry)
            {
                return View("PurchaseNoJewelry");
            }

            if (cart != null)
            {
                await dbContext.Orders.AddAsync(new OrdersModel { ID_User = userId, DateOfPlacement = DateTime.Now });
                await dbContext.SaveChangesAsync();
                OrdersModel userOrder = await dbContext.Orders.OrderBy(x => x.ID).LastOrDefaultAsync(x => x.ID_User == userId);
                List<OrderContentModel> orderContents = new List<OrderContentModel>();
                jewelry = null;
                for (int i = 0; i < cart.CartContent.Count(); i++)
                {
                    jewelry = jewelries.FirstOrDefault(x => x.ID == cart.CartContent[i].ID_Jewelry);
                    jewelry.Quantity -= cart.CartContent[i].Quantity;
                    if (jewelry.Quantity < 0)
                    {
                        break;
                    }
                    dbContext.Jewelries.Add(jewelry);
                    dbContext.Entry(jewelry).State = EntityState.Modified;
                    orderContents.Add(new OrderContentModel
                    {
                        ID_Jewelry = cart.CartContent[i].ID_Jewelry,
                        ID_Order = userOrder.ID,
                        Quantity = cart.CartContent[i].Quantity,
                        Size = cart.CartContent[i].Size,
                        TotalPrice = cart.CartContent[i].TotalPrice.Replace(',', '.'),
                        DateOfPlacement = cart.CartContent[i].DateOfPlacement
                    });
                    purchaseCode += cart.CartContent[i].ID_Jewelry.ToString();
                }
                List<CartContentModel> cartContents = cart.CartContent.ToList();
                JewelrySizeModel jewelrySize = null;
                foreach (var cartContent in cartContents)
                {
                    if (cartContent.Size != "")
                    {
                        jewelrySize = cartContent.Jewelry.JewelrySizes.FirstOrDefault(x => x.Size == cartContent.Size);
                        cartContent.Jewelry.Price = jewelrySize.Price;
                    }
                }
                cart.CartContent = cartContents;
                userOrder.Code = GetPurchaseCode(purchaseCode + DateTime.Now).Substring(0, 10);
                purchaseCode = userOrder.Code;

                dbContext.Orders.Add(userOrder);
                dbContext.Entry(userOrder).State = EntityState.Modified;

                await dbContext.OrderContents.AddRangeAsync(orderContents);
                dbContext.Cart.Remove(cart);
                await dbContext.SaveChangesAsync();

                EmailService emailService = new EmailService();
                string emailMessage = "<div style\"margin: 16px 0px;padding-bottom: 8px;\" style=\"border-bottom: 1px solid #ddd;\">" +
                    "<div style=\"font-size: 22px;text-align: center;padding-bottom: 24px;\">Спасибо за покупку!</div>" +
                    $"<div>Номер Вашего заказа: <b>{purchaseCode}</b></div>" +
                    $"<div>Итоговая цена: <b>{cart.CartContent.Sum(x => double.Parse(x.TotalPrice)).ToString("0.00")}</b></div></div>" +
                    "<div style=\"color: #555555;padding-bottom: 20px;margin-bottom: 20px;position: relative;\">" +
                    "<h3>Ваш заказ</h3></div><table style=\"width: 100%; border-spacing: 0; font-size: 14px;\"><thead><tr>" +
                    "<th style=\"padding-left: 0px;padding-right: 0px;padding: 8px;\">Фотография</th>" +
                    "<th style=\"padding: 8px;\">Название</th>" +
                    "<th style=\"padding: 8px;\">Цена</th>" +
                    "<th style=\"padding: 8px;\">Количество</th>" +
                    "<th style=\"padding: 8px;\">Итого</th></tr></thead><tbody style=\"text-align: center;\">";
                foreach (var orderContent in (await dbContext.Orders.Include(x => x.OrderContents).OrderBy(x => x.ID).LastOrDefaultAsync(x => x.ID_User == userId)).OrderContents.ToList())
                {
                    emailMessage += "<tr style=\"border-top: 1px solid #ddd;\">" +
                        $"<td><img style=\"width: 100%;max-height: 200px;max-width: 200px;\" src=\"https://glatteis.herokuapp.com{orderContent.Jewelry.ImageSrc}\" alt=\"{orderContent.Jewelry.Name}\" /></td>" +
                        $"<td><a style=\"transition: .25s ease-in;color: black;\" onmouseover=\"this.style.color: #d8a247;\" href=\"https://glatteis.herokuapp.com{orderContent.Jewelry.Url}\">{orderContent.Jewelry.Name}</a></td>" +
                        $"<td>BYN {orderContent.Jewelry.Price}</td>" +
                        $"<td>{orderContent.Quantity}</td>" +
                        $"<td>BYN {orderContent.TotalPrice}</td>" +
                        "</tr>";
                }
                emailMessage += "</tbody></table>";

                await emailService.SendEmailAsync((await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId)).Email, "Your purchase", emailMessage);
            }
            else
            {
                return RedirectToAction("List", "Catalog");
            }

            return View(new PurchaseViewModel(purchaseCode, cart));
        }

        [NonAction]
        public string GetPurchaseCode(string str)
        {
            byte[] tmpSource = Encoding.ASCII.GetBytes(str);
            byte[] tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            StringBuilder sOutput = new StringBuilder(tmpHash.Length);
            for (int i = 0; i < tmpHash.Length; i++)
            {
                sOutput.Append(tmpHash[i].ToString("X2"));
            }
            return sOutput.ToString();
        }
    }
}