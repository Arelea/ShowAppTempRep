using AppNov14.Web.Models.Users;
using AppNov14.Web.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace AppNov14.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        public AccountController(UserManager<Users> userManager, SignInManager<Users> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Register(AccountRegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Users user = new Users { UserName = model.LoginApp, FirstName = model.FName, FamilyName = model.LName };
        //        // добавляем пользователя
        //        var result = await _userManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            // установка куки
        //            await _signInManager.SignInAsync(user, false);
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError(string.Empty, error.Description);
        //            }
        //        }
        //    }
        //    return View(model);

        //}

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new AccountLoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.LoginApp, model.Password, false, true);
                if (result.Succeeded)
                {
                   
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (result.IsLockedOut)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("", "Вы превысили количество попыток для входа в систему! Пожалуйста, повторите попытку входа через 5 минут.");
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
