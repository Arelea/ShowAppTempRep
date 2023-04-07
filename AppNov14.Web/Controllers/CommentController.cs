using AppNov14.Handlers.Interfaces;
using AppNov14.Helpers;
using AppNov14.Models.Comment;
using AppNov14.Web.ViewModels.Comment;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AppNov14.Web.Controllers
{
    public class CommentController : BaseController
    {
        private readonly ICommentHandler CommentHandler;

        public CommentController(ICommentHandler commentHandler, IBaseDataHandler baseDataHandler) : base(baseDataHandler)
        {
            this.CommentHandler = commentHandler;
        }

        public IActionResult List(string status)
        {
            var viewModel = this.GetViewModel<CommentListViewModel>();
            if (status == null)
            {
                status = CommentStatus.Opened.ToString();
            }
         
            var handlerResult = this.CommentHandler.GetList(Convert.ToInt32(status));
            viewModel.Status = status;
            viewModel.Statuses = CommentStatus.StatusesList;          
            viewModel.Items = handlerResult;
            viewModel.BackUrl = this.Url.Action("Index", "Home");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Список комментариев";

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = this.GetViewModel<CommentCreateViewModel>();

            viewModel.CommentTypes = CommentType.TypesList;
            viewModel.BackUrl = this.Url.Action("List", "Comment");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Создать комментарий";

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(CommentCreateForm form)
        {
            if (ModelState.IsValid)
            {
                var model = new CommentModel()
                {
                    Type = form.Type,
                    Name = form.Name,
                    Text = form.Text,
                    InsertDate = DateTime.Now,
                    Status = CommentStatus.Opened,
                    Employee = this.User.Identity.Name,
                };

                var result = this.CommentHandler.Create(model);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Create));
                }

                throw new Exception(result.Message);
            }

            return View(ModelState.ErrorCount);
        }


        [HttpGet]
        public IActionResult View(int id)
        {
            var viewModel = this.GetViewModel<CommentViewViewModel>();
            var handlerResult = this.CommentHandler.GetComment(id);

            if (handlerResult.Status == CommentStatus.Opened)
            {
                viewModel.Form = base.CreateForm<CommentViewForm>();
            }

            viewModel.Item = handlerResult;
            viewModel.BackUrl = this.Url.Action("List", "Comment");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Комментарий";

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult View(CommentViewForm form)
        {
            return RedirectToAction("Close", new { id = form.Id, answer = form.Answer });
        }

        public IActionResult Close(int id, string answer)
        {
            var result = this.CommentHandler.Close(id, answer);

            return RedirectToAction(nameof(View), new { Id = id } );    
        }
    }
}
