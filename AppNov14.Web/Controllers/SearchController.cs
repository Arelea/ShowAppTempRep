using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web.WebPages.Html;
using AppNov14.Web.ViewModels.Search;
using AppNov14.Helpers;
using AppNov14.Handlers.Interfaces;
using AppNov14.Models.Search;
using Castle.Core.Internal;

namespace AppNov14.Web.Controllers
{
    [Authorize]
    public class SearchController : BaseController
    {
        private readonly ISearchHandler SearchHandler;

        public SearchController(ISearchHandler searchHandler, IBaseDataHandler baseDataHandler) : base(baseDataHandler)
        {
            this.SearchHandler = searchHandler;
        }   

        public IActionResult SearchLaboratory(CommonSearchForm form)
        {
            var viewModel = this.GetViewModel<CommonSearchViewModel>();
            if (form.DateStart == null && form.DateFinish == null && form.Type == null
                && form.SubType == null && form.Provider == null && form.Manufacturer == null
                && form.DocumentNumber == null && form.Index == null && form.Document == null && form.Line == null && form.Id == null)
            {
                form.ShowMode = RawActionTypes.All.ToString();
            }

            var isReplenishment = Convert.ToInt32(form.ShowMode) == RawActionTypes.Replenishment;
            var model = new SearchModel()
            {
                Id = form.Id,
                Type = form.Type,
                SubType = form.SubType,
                Provider = form.Provider,
                Manufacturer = form.Manufacturer,
                DocumentNumber = isReplenishment ? null : form.DocumentNumber,
                Document = isReplenishment ? null : form.Document,
                Indexation = form.Index,
                ShowMode = form.ShowMode.IsNullOrEmpty() ? default(int?) : Convert.ToInt32(form.ShowMode),
            };

            var items = this.SearchHandler.SearchLaboratory(model);

            viewModel.Items = items;
            viewModel.Form = form;
            viewModel.TypesList = base.SelectListifyItemsWithNull(this.SearchHandler.GetDistinctTypeList(ActionModes.Laboratory));
            viewModel.SubTypesList = base.SelectListifyItemsWithNull(this.SearchHandler.GetDistinctSubTypeList(ActionModes.Laboratory));
            viewModel.ProviderList = base.SelectListifyItemsWithNull(this.SearchHandler.GetDistinctProviderList(ActionModes.Laboratory));
            viewModel.ManufacturerList = base.SelectListifyItemsWithNull(this.SearchHandler.GetDistinctManufacturerList(ActionModes.Laboratory));
            viewModel.ConsignmentTypesList = isReplenishment ? new List<SelectListItem>() : ConsignmentNamesLab.ConsignmentNamesListLabWithUnselect;
            viewModel.ConsignmentNumbersList = isReplenishment
                    ? new List<SelectListItem>()
                    : base.SelectListifyItemsWithNull(this.SearchHandler.GetDistinctLabConsignmentNumberList());
            viewModel.ShowModeList = OperationTypes.OperationTypeList;

            viewModel.BackUrl = this.Url.Action("Index", "Laboratory");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Поиск: Лаборатория";
            viewModel.IsLabMode = true;

            return View(viewModel);
        }

        #region Json

        [HttpPost]
        public JsonResult GetSubTypeByTypeJson(string type, int mode)
        {
            var subTypesList = base.SelectListifyItemsWithNull(this.SearchHandler.GetDistinctSubTypeList(mode, type));

            return Json(subTypesList);
        }

        [HttpPost]
        public JsonResult GetFullSubTypeJson(int mode)
        {
            var subTypesList = base.SelectListifyItemsWithNull(this.SearchHandler.GetDistinctSubTypeList(mode));

            return Json(subTypesList);
        }    

        [HttpPost]
        public JsonResult GetConsignmentNumbersList(int mode)
        {
            var consignmentNumbersList = base.SelectListifyItemsWithNull(this.SearchHandler.GetDistinctConsignmentNumberList(mode));

            return Json(consignmentNumbersList);
        }

        [HttpPost]
        public JsonResult GetLabConsignmentNumbersList()
        {
            var consignmentNumbersList = base.SelectListifyItemsWithNull(this.SearchHandler.GetDistinctLabConsignmentNumberList());

            return Json(consignmentNumbersList);
        }

        #endregion  
    }
}