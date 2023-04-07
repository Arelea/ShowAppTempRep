using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using AppNov14.Web.ViewModels.Laboratory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using AppNov14.Helpers;
using AppNov14.Handlers.Interfaces;
using AppNov14.Models.ManufacturingTable;
using AppNov14.Models.Warehouse;

namespace AppNov14.Web.Controllers
{
    [Authorize]
    public class LaboratoryController : BaseController
    {
        private readonly ILaboratoryHandler LaboratoryHandler;

        public LaboratoryController(ILaboratoryHandler laboratoryHandler, IBaseDataHandler baseDataHandler) : base(baseDataHandler)
        {
            this.LaboratoryHandler = laboratoryHandler;
        }

        public IActionResult Index()
        {
            var viewModel = this.GetViewModel<LaboratoryIndexViewModel>();

            viewModel.BackUrl = this.Url.Action("Index", "Home");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Лаборатория";
            viewModel.IsLabMode = true;

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult CreateIncomingOrderLab()
        {
            var viewModel = this.GetViewModel<CreateIncomingOrderLabViewModel>();

            viewModel.TypeOfMaterialList = base.SelectListifyItems(this.LaboratoryHandler.GetDistinctTypeList(ActionModes.Laboratory));
            viewModel.BackUrl = this.Url.Action("Index", "Laboratory");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Лаборатория: Поступление нового сырья";
            viewModel.IsLabMode = true;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateIncomingOrderLab(CreateIncomingOrderLabForm form)
        {
            if (ModelState.IsValid)
            {
                if (form.Quantity <= 0)
                {
                    throw new Exception(ErrorMessages.ZeroNotAllowed);
                }
                var model = new ManufacturingTableWriteModel()
                {
                    Type = form.Type,
                    SubType = form.SubType,
                    Provider = form.Provider,
                    Manufacturer = form.Manufacturer,
                    Quantity = form.Quantity,
                    Employee = User.Identity.Name,
                    Indexation = form.Indexation,
                    DocDate = form.DocDate,
                    IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    OperationType = RawActionTypes.Replenishment,
                    AutoDate = DateTime.Now,
                    Remarks = form.Remarks,
                };

                var result = this.LaboratoryHandler.ReplenishWarehouse(model);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(CreateIncomingOrderLab));
                }

                throw new Exception(result.Message);
            }

            return View(ModelState.ErrorCount);
        }

        [HttpGet]
        public IActionResult CreateOutcomingOrderLab()
        {
            var viewModel = this.GetViewModel<CreateOutcomingOrderLabViewModel>();

            viewModel.ConsignmentNamesList = ConsignmentNamesLab.ConsignmentNamesListLab;
            viewModel.TypeOfMaterialList = base.SelectListifyItems(this.LaboratoryHandler.GetDistinctTypeList(ActionModes.Laboratory));
            viewModel.BackUrl = this.Url.Action("Index", "Laboratory");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Лаборатория: Списание сырья";
            viewModel.IsLabMode = true;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateOutcomingOrderLab(CreateOutcomingOrderLabForm form)
        {
            if (ModelState.IsValid)
            {
                if (form.Quantity <= 0)
                {
                    throw new Exception(ErrorMessages.ZeroNotAllowed);
                }
                var model = new ManufacturingTableWriteModel()
                {
                    Type = form.Type,
                    SubType = form.SubType,
                    Provider = form.Provider,
                    Manufacturer = form.Manufacturer,
                    Quantity = form.Quantity,
                    DocumentNumber = form.DocumentNumber,
                    Document = form.Document,
                    Employee = User.Identity.Name,
                    Indexation = form.Indexation,
                    DocDate = form.DocDate,
                    IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    OperationType = RawActionTypes.WriteOff,
                    AutoDate = DateTime.Now,
                    Remarks = form.Remarks,
                };

                var result = this.LaboratoryHandler.WriteOffWarehouse(model);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(CreateOutcomingOrderLab));
                }

                throw new Exception(result.Message);
            }

            return View(ModelState.ErrorCount);
        }

        [HttpGet]
        public IActionResult LastAddingsLab()
        {
            var viewModel = this.GetViewModel<LastAddingsLabViewModel>();
            var items = this.LaboratoryHandler.GetLastAdding();

            viewModel.Items = items;
            viewModel.BackUrl = this.Url.Action("Index", "Laboratory");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Лаборатория: Последние записи";
            viewModel.IsLabMode = true;

            return View(viewModel);
        }

        [Authorize(Roles = "Moderator, Admin")]
        public IActionResult EditListLab(EditDeleteAllDataLabForm form)
        {
            var viewModel = this.GetViewModel<EditDeleteAllDataLabViewModel>();
            if (form.DateStart == DateTime.MinValue || form.DateFinish == DateTime.MinValue)
            {
                form.DateStart = DateTime.Now.AddMonths(Other.EditFromMonths);
                form.DateFinish = DateTime.Now.AddDays(Other.EditToDays);
            }
            var items = this.LaboratoryHandler.GetItemsByStartAndEndDate(form.DateStart, form.DateFinish);

            viewModel.Form = form;
            viewModel.Items = items;
            viewModel.BackUrl = this.Url.Action("Index", "Laboratory");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Лаборатория: Записи главной таблицы для редактирования";
            viewModel.IsLabMode = true;

            return View(viewModel);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpGet]
        public IActionResult EditLab(int? id)
        {
            var viewModel = this.GetViewModel<EditLabViewModel>();
            var record = this.LaboratoryHandler.GetById(id.Value);
            if (record == null)
            {
                throw new Exception(ErrorMessages.RecordNotFound);
            }

            var form = this.CreateForm<EditLabForm>();
            form.Fill(record);

            viewModel.Form = form;
            viewModel.TypeOfMaterialList = base.SelectListifyItems(this.LaboratoryHandler.GetDistinctTypeList(ActionModes.Laboratory));
            viewModel.NameOfTypeMaterialList = base.SelectListifyItems(this.LaboratoryHandler.GetDistinctSubTypeList(ActionModes.Laboratory, record.Type));
            viewModel.ProviderList = base.SelectListifyItems(this.LaboratoryHandler.GetDistinctProviderList(ActionModes.Laboratory, record.Type, record.SubType));
            viewModel.ManufacturerList = base.SelectListifyItems(this.LaboratoryHandler.GetDistinctManufacturerList(ActionModes.Laboratory, record.Type, record.SubType, record.Provider));
            if (record.OperationType == RawActionTypes.WriteOff)
            {
                viewModel.IndexList = base.SelectListifyItemsFromDictionary(this.LaboratoryHandler.GetDistinctFilledIndexationLabList(record.Type, record.SubType, record.Provider, record.Manufacturer));
                viewModel.ConsignmentTypesList = ConsignmentNamesLab.ConsignmentNamesListLab;
            }
            viewModel.BackUrl = this.Url.Action("EditListLab", "Laboratory");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Лаборатория: Редактор записей таблицы";
            viewModel.IsLabMode = true;

            return View(viewModel);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpPost]
        public IActionResult EditLab(EditLabForm form)
        {

            if (ModelState.IsValid)
            {
                var model = new ManufacturingTableWriteModel()
                {
                    Id = form.Id,
                    Type = form.Type,
                    SubType = form.SubType,
                    Provider = form.Provider,
                    Manufacturer = form.Manufacturer,
                    Quantity = form.Quantity,
                    DocumentNumber = form.DocumentNumber,
                    Document = form.Document,
                    Employee = Other.UpdatedBy + User.Identity.Name,
                    Indexation = form.Indexation,
                    DocDate = form.DocDate,
                    OperationType = form.OperationType,
                    Remarks = form.Remarks,
                };

                var result = this.LaboratoryHandler.Edit(model);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(EditListLab));
                }

                throw new Exception(result.Message);
            }

            return View(ModelState.ErrorCount);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpGet]
        public IActionResult DeleteLab(int? id)
        {
            var viewModel = this.GetViewModel<DeleteTableRowLabViewModel>();
            var record = this.LaboratoryHandler.GetById(id.Value);
            if (record == null)
            {
                throw new Exception(ErrorMessages.RecordNotFound);
            }

            var form = this.CreateForm<EditLabForm>();
            form.Fill(record);

            viewModel.Form = form;
            viewModel.BackUrl = this.Url.Action("EditListLab", "Laboratory");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Лаборатория: Удаление записи";
            viewModel.IsLabMode = true;

            return View(viewModel);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpPost]
        public IActionResult DeleteLab(int id)
        {
            var result = this.LaboratoryHandler.Delete(id);
            if (result.IsSuccess)
            {
                return RedirectToAction(nameof(EditListLab));
            }

            throw new Exception(result.Message);
        }

        public IActionResult ImportExcelFileLab(ImportExcelFileLabForm form)
        {
            var viewModel = this.GetViewModel<ImportExcelFileLabViewModel>();
            if (form.DateStart == DateTime.MinValue || form.DateFinish == DateTime.MinValue)
            {
                form.DateStart = DateTime.Now.AddMonths(Other.EditFromMonths);
                form.DateFinish = DateTime.Now.AddDays(Other.EditToDays);
            }
            var handler = this.LaboratoryHandler.GetRecordCountByDate(form.DateStart, form.DateFinish);

            viewModel.ResultsCount = handler;
            viewModel.Form = form;
            viewModel.BackUrl = this.Url.Action("Index", "Laboratory");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Лаборатория: Скачать эксель выписку";
            viewModel.IsLabMode = true;

            return View(viewModel);
        }

        public IActionResult DownloadExcelLab(string dateStart, string dateFinish)
        {
            var items = this.LaboratoryHandler.GetDataTableForExcel(Convert.ToDateTime(dateStart), Convert.ToDateTime(dateFinish));
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Лист 1");
                worksheet.Cells.LoadFromDataTable(items, true);
                worksheet.Column(11).Style.Numberformat.Format = "dd:mm:yyyy hh:mm:ss";
                worksheet.Column(14).Style.Numberformat.Format = "dd:mm:yyyy hh:mm:ss";
                worksheet.Cells.AutoFitColumns();
                worksheet.Cells[1, 1, 1, 16].AutoFilter = true;

                using (var heading = worksheet.Cells[1, 1, 1, 16])
                {
                    var colory = heading.Style.Fill;
                    colory.PatternType = ExcelFillStyle.Solid;
                    colory.BackgroundColor.SetColor(Color.LightBlue);
                    heading.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                }

                package.Save();
            }

            stream.Position = 0;
            var excelname = $"VestPlast.DataLab.{DateTime.Now}.xlsx";

            return File(stream, "application/vnd.openformats-officedocument.spreadsheetml.sheet", excelname);
        }

        public IActionResult ConsignmentNumberInfoLab(string number)
        {
            var viewModel = this.GetViewModel<ConsignmentNumberInfoLabViewModel>();
            var items = new List<ManufacturingTableWriteModel>();
            if (number != null)
            {
                items = this.LaboratoryHandler.GetItemsByConsignmentNumber(number);
            }

            viewModel.Items = items;
            viewModel.Number = number;
            viewModel.ConsignmentList = base.SelectListifyItems(this.LaboratoryHandler.GetDistinctLabConsignmentNumberList());

            viewModel.BackUrl = this.Url.Action("Index", "Laboratory");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Лаборатория: Состав партии";
            viewModel.IsLabMode = true;

            return View(viewModel);
        }

        public IActionResult ViewIndexLab(ViewIndexLabForm form)
        {
            var viewModel = this.GetViewModel<ViewIndexLabViewModel>();
            var items = new List<IndexDataModel>();

            if (form.Manufacturer != null)
            {
                items = this.LaboratoryHandler.GetIndexationByWarehouse(ActionModes.Laboratory, form.Type, form.SubType, form.Provider, form.Manufacturer);

                viewModel.NameOfTypeMaterialList = base.SelectListifyItems(this.LaboratoryHandler.GetDistinctSubTypeList(ActionModes.Laboratory, form.Type));
                viewModel.ProviderList = base.SelectListifyItems(this.LaboratoryHandler.GetDistinctProviderList(ActionModes.Laboratory, form.Type, form.SubType));
                viewModel.ManufacturerList = base.SelectListifyItems(this.LaboratoryHandler.GetDistinctManufacturerList(ActionModes.Laboratory, form.Type, form.SubType, form.Provider));
            }

            viewModel.Items = items;
            viewModel.TypeOfMaterialList = base.SelectListifyItems(this.LaboratoryHandler.GetDistinctTypeList(ActionModes.Laboratory));
            viewModel.Form = form;
            viewModel.BackUrl = this.Url.Action("Index", "Laboratory");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Лаборатория: Состав партии";
            viewModel.IsLabMode = true;

            return View(viewModel);
        }

        #region Json

        [HttpPost]
        public JsonResult GetIndexationJson(string type, string subType, string provider, string manufacturer)
        {
            var indexationList = base.SelectListifyItemsFromDictionary(this.LaboratoryHandler.GetDistinctFilledIndexationLabList(type, subType, provider, manufacturer));

            return Json(indexationList);
        }

        #endregion
    }
}