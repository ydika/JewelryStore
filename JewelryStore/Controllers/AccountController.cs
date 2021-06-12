using JewelryStore.Models;
using JewelryStore.Services;
using JewelryStore.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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
    public class AccountController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;

        public AccountController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel user = new UserModel { FirstName = model.FirstName, SecondName = model.SecondName, Email = model.Email, UserName = model.Email };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    await SendEmailConfirm(await _userManager.FindByEmailAsync(model.Email), model.Email);

                    return View("ConfirmEmail");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);

                return RedirectToAction("Index", "Home");
            }
            else return View("Error");
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Вы не подтвердили Email. Для подтверждения Email перейдите по ссылке, которую мы отправили на вашу электронную почту.");
                    await SendEmailConfirm(user, model.Email);
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    await _userManager.AccessFailedAsync(user);
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [NonAction]
        public async Task SendEmailConfirm(UserModel user, string email)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
            EmailService emailService = new EmailService();

            await emailService.SendEmailAsync(email, "Confirm your account",
                "<div style=\"background-color:#F7F6F2;text-align:center;color:#836027;padding-bottom:30px;\">" +
                "<div style=\"font-size:40px;margin:0px;\">Glatteis</div>" +
                "<img style=\"border:1px solid #836027;width:600px;\" src=\"https://glatteis.herokuapp.com/images/emailpic.png\" alt=\"Ювелирный изделия. На любой вкус\">" +
                "<div style=\"font-size:30px;margin-bottom:10px;\"> <u>Подтверждение Email</u></div>" +
                $"<div style=\"color:black;font-size:16px;margin-bottom:30px;\">Для подтверждения вашего Email перейдите по <a href={callbackUrl}>ссылке</a>.</div>" +
                "<div style=\"font-size:12px;color:red;\">Внимание! Если вы не регестрировались на нашем сайте, то просто проигнорируйте это сообщение.</div></div>");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("[action]")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("[action]")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                ModelState.AddModelError(string.Empty, "Для сброса пароля перейдите по ссылке в письме, отправленном на Ваш Email.");
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return View(model);
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                EmailService emailService = new EmailService();
                await emailService.SendEmailAsync(model.Email, "Reset Password",
                    "<div style=\"background-color:#F7F6F2;text-align:center;color:#836027;padding-bottom:30px;\">" +
                    "<div style=\"font-size:40px;margin:0px;\">Glatteis</div>" +
                    "<img style=\"border:1px solid #836027;width:600px;\" src=\"https://glatteis.herokuapp.com/images/emailpic.png\" alt=\"Ювелирный изделия. На любой вкус\">" +
                    "<div style=\"font-size:30px;margin-bottom:10px;\"> <u>Сброс пароля</u></div>" +
                    $"<div style=\"color:black;font-size:16px;margin-bottom:30px;\">Для сброса пароля пройдите по <a href={callbackUrl}>ссылке</a>.</div>" +
                    "<div style=\"font-size:12px;color:red;\">Внимание! Если вы не сбрасываете пароль на нашем сайте, то просто проигнорируйте это сообщение.</div></div>");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("[action]")]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("[action]")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
    }
}
