using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppNov14.Handlers.Interfaces;
using AppNov14.Web.Models.Roles;
using AppNov14.Web.Models.Users;
using AppNov14.Web.ViewModels.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppNov14.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : BaseController
    {
        RoleManager<IdentityRole> _roleManager;

        UserManager<Users> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<Users> userManager, IBaseDataHandler baseDataHandler) : base(baseDataHandler)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var viewModel = this.GetViewModel<RoleIndexViewModel>();

            viewModel.Roles = _roleManager.Roles.ToList();
            viewModel.BackUrl = this.Url.Action("Index", "Home");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Редактирование ролей";

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = this.GetViewModel<RoleCreateViewModel>();

            viewModel.BackUrl = this.Url.Action("Index", "Roles");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Добавить роль";

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleCreateForm form)
        {
            if (!string.IsNullOrEmpty(form.RoleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(form.RoleName));
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

            return RedirectToAction(nameof(Create));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult UserList()
        {
            var viewModel = this.GetViewModel<RoleUserListViewModel>();

            viewModel.UserList = _userManager.Users.ToList();
            viewModel.BackUrl = this.Url.Action("Index", "Roles");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Список пользователей";

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string userId)
        {
            var viewModel = this.GetViewModel<RoleEditViewModel>();

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                var model = new ChangeUserRoleModel
                {
                    UserId = user.Id,
                    UserLogin = user.UserName,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };

                viewModel.ChangeRoleModel = model;
                viewModel.BackUrl = this.Url.Action("Index", "Roles");
                viewModel.BackUrlName = "Назад";
                viewModel.PageName = $"Изменение ролей для пользователя {user.UserName}";

                return View(viewModel);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            // получаем пользователя
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _userManager.GetRolesAsync(user);
                // получаем все роли
                var allRoles = _roleManager.Roles.ToList();
                // получаем список ролей, которые были добавлены
                var addedRoles = roles.Except(userRoles);
                // получаем роли, которые были удалены
                var removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(user, addedRoles);

                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                return RedirectToAction(nameof(UserList));
            }

            return NotFound();
        }
    }
}
