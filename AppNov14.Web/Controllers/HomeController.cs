using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using AppNov14.Web.ViewModels.Home;
using AppNov14.Handlers.Interfaces;

namespace AppNov14.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IBaseDataHandler baseDataHandler) : base(baseDataHandler)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var viewModel = this.GetViewModel<IndexViewModel>();

            viewModel.PageName = "База данных VestPlast";
            viewModel.IsBackButtonNeeded = false;

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            var viewModel = this.GetViewModel<PrivacyViewModel>();

            viewModel.BackUrl = this.Url.Action("Index", "Home");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "О базе";

            return View(viewModel);
        }

        [AllowAnonymous]
        public IActionResult Gate()
        {
            return View();
        }
    }
}