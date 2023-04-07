using AppNov14.Web.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AppNov14.Web.ViewModels.Users;
using AppNov14.Handlers.Interfaces;

namespace AppNov14.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : BaseController
    {
        UserManager<Users> _userManager;

        public UsersController(UserManager<Users> userManager, IBaseDataHandler baseDataHandler) : base(baseDataHandler)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = this.GetViewModel<UserIndexViewModel>();

            viewModel.Users = _userManager.Users.ToList();
            viewModel.BackUrl = this.Url.Action("Index", "Home");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Редактирование пользователей";

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = this.GetViewModel<UserCreateViewModel>();

            viewModel.BackUrl = this.Url.Action("Index", "Users");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Добавление пользователя";

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateForm form)
        {
            if (ModelState.IsValid)
            {
                var user = new Users
                {
                    UserName = form.LoginApp,
                    FirstName = form.FirstName,
                    FamilyName = form.LastName,
                };

                var result = await _userManager.CreateAsync(user, form.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var viewModel = this.GetViewModel<UserEditViewModel>();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var form = this.CreateForm<UserEditForm>();
            form.Id = user.Id;
            form.LoginApp = user.UserName;
            form.FirstName = user.FirstName;
            form.LastName = user.FamilyName;

            viewModel.Form = form;
            viewModel.BackUrl = this.Url.Action("Index", "Users");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = $"Редактирование пользователя {user.UserName}";

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditForm form)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(form.Id);
                if (user != null)
                {

                    user.UserName = form.LoginApp;
                    user.FirstName = form.FirstName;
                    user.FamilyName = form.LastName;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(string id)
        {
            var viewModel = this.GetViewModel<UserChangePasswordViewModel>();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var form = this.CreateForm<UserChangePasswordForm>();
            form.Id = user.Id;

            viewModel.Form = form;
            viewModel.BackUrl = this.Url.Action("Index", "Users");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = $"Изменение пароля для пользователя {user.UserName}";

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserChangePasswordForm form)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(form.Id);
                if (user != null)
                {
                    var _passwordValidator =
                        HttpContext.RequestServices.GetService(typeof(IPasswordValidator<Users>)) as IPasswordValidator<Users>;
                    var _passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<Users>)) as IPasswordHasher<Users>;

                    IdentityResult result =
                        await _passwordValidator.ValidateAsync(_userManager, user, form.NewPassword);
                    if (result.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, form.NewPassword);
                        await _userManager.UpdateAsync(user);

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}