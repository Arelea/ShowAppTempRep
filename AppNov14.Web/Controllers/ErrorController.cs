using AppNov14.Handlers.Interfaces;
using AppNov14.Web.ViewModels.Error;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AppNov14.Web.Controllers
{

    public class ErrorController : BaseController
    {
        public ErrorController(IBaseDataHandler baseDataHandler) : base(baseDataHandler)
        {
        }

        [Authorize]
        [Route("Error")]
        public IActionResult Error()
        {
            var viewModel = this.GetViewModel<ErrorViewModel>();
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            viewModel.ExceptionPath = exceptionDetails.Path;
            viewModel.ExceptionMessage = exceptionDetails.Error.Message;

            viewModel.BackUrl = this.Url.Action("Index", "Home");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Ошибка";

            return View(viewModel);
        }

    }
}