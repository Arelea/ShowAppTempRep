using AppNov14.Handlers.Interfaces;
using AppNov14.Web.ViewModels.Goods;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using AppNov14.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AppNov14.Web.Controllers
{
    [Authorize(Roles = "Gooder")]
    public class GoodsController : BaseController
    {
        private readonly IGoodsHandler GoodsHandler;

        public GoodsController(IGoodsHandler goodsHandler, IBaseDataHandler baseDataHandler) : base(baseDataHandler)
        {
            this.GoodsHandler = goodsHandler;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = this.GetViewModel<GoodsIndexViewModel>();

            viewModel.BackUrl = this.Url.Action("Index", "Home");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Отгрузки";

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult AddCustomer()
        {
            var viewModel = this.GetViewModel<AddCustomerViewModel>();

            viewModel.BackUrl = this.Url.Action("Index", "Goods");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Отгрузки: Добавить покупателя";

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddCustomer(string name)
        {
            if (!name.IsNullOrEmpty())
            {
                var result = this.GoodsHandler.AddCustomerName(name);

                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(AddCustomer));
                }

                throw new Exception(result.Message);
            }

            throw new Exception();
        }

        public IActionResult MoveCompletedBatchList(MoveCompletedBatchListForm form)
        {
            var viewModel = this.GetViewModel<MoveCompletedBatchListViewModel>();
            var handlerResult = this.GoodsHandler.GetBatchList(new List<int>() { BatchStatuses.InManufacturingProcess, BatchStatuses.NoData }, form.BatchStatusId, form.BatchId);

            viewModel.BatchList = base.SelectListifyItemsFromDictionaryWithNull(handlerResult.BatchDictionary, placeholder: "Все партии");
            viewModel.BatchStatusList = base.SelectListifyItemsFromDictionary(handlerResult.BatchStatusDictionary);
            viewModel.Form = form;
            viewModel.Items = handlerResult.Items;

            viewModel.BackUrl = this.Url.Action("Index", "Goods");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Отгрузки: Список партий на внесение";

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult MoveCompletedBatch(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("MoveCompletedBatchList");
            }

            var viewModel = this.GetViewModel<MoveCompletedBatchViewModel>();
            var handlerResult = this.GoodsHandler.GetCompletedBatch(id.Value);
            viewModel.Form = this.CreateForm<MoveCompletedBatchForm>();
            viewModel.Form.Id = handlerResult.Batch.Id;

            viewModel.Item = handlerResult.Batch;
            viewModel.BackUrl = this.Url.Action("MoveCompletedBatchList", "Goods");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = $"Отгрузки: Внести партию «{handlerResult.Batch.Name}» на склад";

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult MoveCompletedBatch(MoveCompletedBatchForm form)
        {
            if (ModelState.IsValid)
            {
                if (form.InitialPackage <= 0 || form.InitialQuantity <= 0)
                {
                    throw new Exception(ErrorMessages.ZeroNotAllowed);
                }

                var result = this.GoodsHandler.MoveCompletedBatch(form.Id, form.InitialPackage, form.InitialQuantity, form.CompletionDate, form.Remark);
                if (result.IsSuccess)
                {
                    return RedirectToAction("BatchFullInfo", "Goods", new { id = form.Id });
                }

                throw new Exception(result.Message);
            }

            return View(ModelState.ErrorCount);
        }

        public IActionResult BatchFullInfo(int id)
        {
            var viewModel = this.GetViewModel<BatchFullInfoViewModel>();
            var handlerResult = this.GoodsHandler.GetBatchFullInfo(id);

            viewModel.Batch = handlerResult.Batch;

            viewModel.BackUrl = this.Url.Action("Index", "Goods");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Отгрузки: Подробная информация о партии";

            return View(viewModel);
        }

        public IActionResult SellBatchList(SellBatchListForm form)
        {
            var viewModel = this.GetViewModel<SellBatchListViewModel>();
            var handlerResult = this.GoodsHandler.GetBatchList(new List<int>() { BatchStatuses.Completed }, BatchStatuses.Completed, form.BatchId);

            viewModel.BatchList = base.SelectListifyItemsFromDictionaryWithNull(handlerResult.BatchDictionary, placeholder: "Все партии");
            viewModel.Form = form;
            viewModel.Items = handlerResult.Items;

            viewModel.BackUrl = this.Url.Action("Index", "Goods");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Отгрузки: Список готовых партий на отгрузку";

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult SellBatch(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("SellBatchList");
            }

            var viewModel = this.GetViewModel<SellBatchViewModel>();
            var handlerResult = this.GoodsHandler.GetBatchForSell(id.Value);
            viewModel.Form = this.CreateForm<SellBatchForm>();
            viewModel.Form.Id = handlerResult.Batch.Id;

            viewModel.Customers = base.SelectListifyItemsFromDictionary(handlerResult.Customers);
            viewModel.Item = handlerResult.Batch;
            viewModel.BackUrl = this.Url.Action("SellBatchList", "Goods");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = $"Отгрузки: Отгрузить партию «{handlerResult.Batch.Name}»";

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SellBatch(SellBatchForm form)
        {
            if (ModelState.IsValid)
            {
                if (form.Package <= 0 || form.Quantity <= 0)
                {
                    throw new Exception(ErrorMessages.ZeroNotAllowed);
                }
                if (!form.CustomerId.HasValue)
                {
                    throw new Exception(ErrorMessages.CustomerNotExist);
                }

                var result = this.GoodsHandler.SellBatch(form.Id, form.Package, form.Quantity, form.CustomerId.Value, form.SoldDate, form.Remark);
                if (result.IsSuccess)
                {
                    return RedirectToAction("BatchFullInfo", "Goods", new { id = form.Id });
                }

                throw new Exception(result.Message);
            }

            return View(ModelState.ErrorCount);
        }

        [HttpGet]
        public IActionResult ReturnBatch(int id)
        {
            
            var viewModel = this.GetViewModel<ReturnBatchViewModel>();
            var handlerResult = this.GoodsHandler.GetBatchForReturn(id);
            viewModel.Form = this.CreateForm<ReturnBatchForm>();
            viewModel.Form.Id = handlerResult.Batch.Id;

            viewModel.Customers = base.SelectListifyItemsFromDictionary(handlerResult.Customers);
            viewModel.Item = handlerResult.Batch;
            viewModel.BackUrl = this.Url.Action("BatchFullInfo", "Goods", new { id = id } );
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = $"Отгрузки: Возврат партии «{handlerResult.Batch.Name}»";

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ReturnBatch(ReturnBatchForm form)
        {
            if (ModelState.IsValid)
            {
                if (form.Package <= 0 || form.Quantity <= 0)
                {
                    throw new Exception(ErrorMessages.ZeroNotAllowed);
                }
                if (!form.CustomerId.HasValue)
                {
                    throw new Exception(ErrorMessages.CustomerNotExist);
                }

                var result = this.GoodsHandler.ReturnBatch(form.Id, form.Package, form.Quantity, form.CustomerId.Value, form.ReturnDate, form.Remark);
                if (result.IsSuccess)
                {
                    return RedirectToAction("BatchFullInfo", "Goods", new { id = form.Id });
                }

                throw new Exception(result.Message);
            }

            return View(ModelState.ErrorCount);
        }

        [HttpGet]
        public IActionResult MergeBatch(int id)
        {
            var viewModel = this.GetViewModel<MergeBatchViewModel>();
            var handlerResult = this.GoodsHandler.GetBatchForMerge(id);
            viewModel.Form = this.CreateForm<MergeBatchForm>();
            viewModel.Form.Id = handlerResult.Batch.Id;

            viewModel.ParentBatches = base.SelectListifyItemsFromDictionary(handlerResult.ParentBatches);
            viewModel.Item = handlerResult.Batch;
            viewModel.BackUrl = this.Url.Action("BatchFullInfo", "Goods", new { id = id });
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = $"Отгрузки: Добавление партии «{handlerResult.Batch.Name}» в другую";

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult MergeBatch(MergeBatchForm form)
        {
            if (ModelState.IsValid)
            {
                if (form.Package <= 0 || form.Quantity <= 0)
                {
                    throw new Exception(ErrorMessages.ZeroNotAllowed);
                }

                var result = this.GoodsHandler.MergeBatch(form.Id, form.Package, form.Quantity, form.ParentBatchId, form.Date, form.Remark);
                if (result.IsSuccess)
                {
                    return RedirectToAction("BatchFullInfo", "Goods", new { id = form.Id });
                }

                throw new Exception(result.Message);
            }

            return View(ModelState.ErrorCount);
        }

        [HttpGet]
        public IActionResult ThrowBatch(int id)
        {
            var viewModel = this.GetViewModel<ThrowBatchViewModel>();
            var handlerResult = this.GoodsHandler.GetBatchForThrow(id);
            viewModel.Form = this.CreateForm<ThrowBatchForm>();
            viewModel.Form.Id = handlerResult.Batch.Id;

            viewModel.Item = handlerResult.Batch;
            viewModel.BackUrl = this.Url.Action("BatchFullInfo", "Goods", new { id = id });
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = $"Отгрузки: Утилизация партии «{handlerResult.Batch.Name}»";

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ThrowBatch(ThrowBatchForm form)
        {
            if (ModelState.IsValid)
            {
                var result = this.GoodsHandler.ThrowBatch(form.Id, form.Remark);
                if (result.IsSuccess)
                {
                    return RedirectToAction("BatchFullInfo", "Goods", new { id = form.Id });
                }

                throw new Exception(result.Message);
            }

            return View(ModelState.ErrorCount);
        }

        public IActionResult ViewBatchWarehouse(ViewBatchWarehouseForm form)
        {
            var viewModel = this.GetViewModel<ViewBatchWarehouseViewModel>();
            var handlerResult = this.GoodsHandler.GetViewBatchWarehouse(form.Query, form.BatchId, form.TypeId, form.LineId, form.StatusId);

            viewModel.BatchNameList = base.SelectListifyItemsFromDictionaryWithNull(handlerResult.BatchNameDictionary, placeholder: "Все");
            viewModel.BatchTypeList = base.SelectListifyItemsFromDictionaryWithNull(handlerResult.BatchTypeDictionary, placeholder: "Все");
            viewModel.BatchLineList = base.SelectListifyItemsFromDictionaryWithNull(handlerResult.BatchLineDictionary, placeholder: "Все");
            viewModel.BatchStatusList = base.SelectListifyItemsFromDictionaryWithNull(handlerResult.BatchStatusDictionary, placeholder: "Все");
            viewModel.Form = form;
            viewModel.Items = handlerResult.Items;

            viewModel.BackUrl = this.Url.Action("Index", "Goods");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Отгрузки: Склад партий";

            return View(viewModel);
        }

        public IActionResult SellingsList(SellingsListForm form)
        {
            var viewModel = this.GetViewModel<SellingsListViewModel>();
            var handlerResult = this.GoodsHandler.GetDataByHistoryList(form.Query, form.BatchId, form.CustomerId, form.SellDateFrom, form.SellDateTo, BatchHistoryActions.SellAction);

            viewModel.BatchNameList = base.SelectListifyItemsFromDictionaryWithNull(handlerResult.BatchNameDictionary, placeholder: "Все");
            viewModel.CustomerList = base.SelectListifyItemsFromDictionaryWithNull(handlerResult.CustomersDictionary, placeholder: "Все");
            viewModel.Form = form;
            viewModel.Items = handlerResult.Items;

            viewModel.BackUrl = this.Url.Action("Index", "Goods");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Отгрузки: Список отгрузок";

            return View(viewModel);
        }

        public IActionResult DownloadBatchWarehouseExcel()
        {
            var items = this.GoodsHandler.GetBatchWarehouseExcel();

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Лист 1");
                worksheet.Cells.LoadFromDataTable(items, true);
                worksheet.Column(8).Style.Numberformat.Format = "dd.mm.yyyy";
                worksheet.Column(9).Style.Numberformat.Format = "dd.mm.yyyy hh:mm:ss";
                worksheet.Cells.AutoFitColumns();
                worksheet.Cells[1, 1, 1, 9].AutoFilter = true;

                using (var heading = worksheet.Cells[1, 1, 1, 9])
                {
                    var colory = heading.Style.Fill;
                    colory.PatternType = ExcelFillStyle.Solid;
                    colory.BackgroundColor.SetColor(Color.LightBlue);
                    heading.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                }

                package.Save();
            }

            stream.Position = 0;
            var excelName = $"Goods.BatchWarehouse.{DateTime.Now}.xlsx";

            return File(stream, "application/vnd.openformats-officedocument.spreadsheetml.sheet", excelName);
        }

        public IActionResult DownloadSellingsExcel()
        {
            var items = this.GoodsHandler.DownloadSellingsExcel();

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Лист 1");
                worksheet.Cells.LoadFromDataTable(items, true);
                worksheet.Column(9).Style.Numberformat.Format = "dd.mm.yyyy";
                worksheet.Cells.AutoFitColumns();
                worksheet.Cells[1, 1, 1, 9].AutoFilter = true;

                using (var heading = worksheet.Cells[1, 1, 1, 9])
                {
                    var colory = heading.Style.Fill;
                    colory.PatternType = ExcelFillStyle.Solid;
                    colory.BackgroundColor.SetColor(Color.LightBlue);
                    heading.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                }

                package.Save();
            }

            stream.Position = 0;
            var excelName = $"Goods.SellingsList.{DateTime.Now}.xlsx";

            return File(stream, "application/vnd.openformats-officedocument.spreadsheetml.sheet", excelName);
        }

        [HttpGet]
        public IActionResult EditHistory(int id, int actionId, int historyId)
        {
            var viewModel = this.GetViewModel<EditHistoryViewModel>();
            var handlerResult = this.GoodsHandler.GetBatchForEditHistory(id, actionId, historyId);
            viewModel.Form = this.CreateForm<EditHistoryForm>();
            viewModel.Form.BatchId = id;
            viewModel.Form.Action = actionId;
            viewModel.Form.HistoryId = handlerResult.Batch.Id;
            viewModel.Form.CustomerId = handlerResult.Batch.CustomerId;
            viewModel.Form.ParentBatchId = handlerResult.Batch.ParentBatchId;
            viewModel.Form.Package = handlerResult.Batch.Package;
            viewModel.Form.Quantity = handlerResult.Batch.Quantity;
            if (actionId == BatchHistoryActions.SellAction || actionId == BatchHistoryActions.ReturnFromCustomerAction)
            {
                viewModel.CustomersList = base.SelectListifyItemsFromDictionary(handlerResult.Customers);
            }
            if (actionId == BatchHistoryActions.MergeToOtherAction)
            {
                viewModel.ParentBatchesList = base.SelectListifyItemsFromDictionary(handlerResult.ParentBatches);
            }

            viewModel.ActionName = handlerResult.Batch.ActionName;

            viewModel.BackUrl = this.Url.Action("BatchFullInfo", "Goods", new { id = id });
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = $"Отгрузки: Редактирование партии «{handlerResult.Batch.Name}»";

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditHistory(EditHistoryForm form)
        {
            if (form.Quantity <= 0 || form.Package <= 0)
            {
                throw new Exception(ErrorMessages.ZeroNotAllowed);
            }
            if (form.Action != BatchHistoryActions.MergeToOtherAction)
            {
                ModelState.Remove("form.ParentBatchId");
            }
            if (form.Action != BatchHistoryActions.SellAction || form.Action != BatchHistoryActions.ReturnFromCustomerAction)
            {
                ModelState.Remove("form.CustomerId");
            }

            if (ModelState.IsValid)
            {
                var result = this.GoodsHandler.EditHistory(form.BatchId, form.HistoryId, form.Action, form.Quantity, form.Package, form.CustomerId, form.ParentBatchId);
                if (result.IsSuccess)
                {
                    return RedirectToAction("BatchFullInfo", "Goods", new { id = form.BatchId });
                }

                throw new Exception(result.Message);
            }

            return View(ModelState.ErrorCount);
        }

        [HttpGet]
        public IActionResult DeleteHistory(int id, int actionId, int historyId)
        {
            var deleteResult = this.GoodsHandler.DeleteHistory(id, actionId, historyId);
            if (deleteResult.IsSuccess)
            {
                return RedirectToAction("BatchFullInfo", "Goods", new { id = id });
            }

            throw new Exception(deleteResult.Message);
        }

        #region Json

        [HttpPost]
        public JsonResult GetBatchByStatusAndTypeAndLine(int? statusId, int? typeId, int? lineId)
        {
            var batchList = base.SelectListifyItemsFromDictionaryWithNull(this.GoodsHandler.GetFilteredBatches(statusId: statusId, typeId: typeId, lineId: lineId), placeholder: "Все");

            return Json(batchList);
        }

        [HttpPost]
        public JsonResult GetBatchByCustomer(int? customerId, int actionHistoryType)
        {
            var batchList = base.SelectListifyItemsFromDictionaryWithNull(this.GoodsHandler.GetBatchesByCustomer(customerId: customerId, actionHistoryType: actionHistoryType), placeholder: "Все");

            return Json(batchList);
        }

        #endregion
    }
}