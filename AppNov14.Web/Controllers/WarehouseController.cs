using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using AppNov14.Handlers.Interfaces;
using AppNov14.Models.Warehouse;
using AppNov14.Web.ViewModels.Warehouse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppNov14.Helpers;
using System.Linq;
using System.Web.WebPages.Html;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Castle.Core.Internal;

namespace AppNov14.Web.Controllers
{
    [Authorize]
    public class WarehouseController : BaseController
    {
        private readonly IWarehouseHandler WarehouseHandler;

        public WarehouseController(IWarehouseHandler warehouseHandler, IBaseDataHandler baseDataHandler) : base(baseDataHandler)
        {
            this.WarehouseHandler = warehouseHandler;
        }

        #region Manufacturing

        [HttpGet]
        public IActionResult CreateNewWarehouseField()
        {
            var viewModel = this.GetViewModel<WarehouseCreateNewWarehouseFieldViewModel>();

            viewModel.TypeOfMaterialList = base.SelectListifyItems(this.WarehouseHandler.GetDistinctTypeList(ActionModes.Manufacturing));
            viewModel.BackUrl = this.Url.Action("Index", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Добавление полей склада";

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateNewWarehouseField(WarehouseCreateNewWarehouseFieldForm form)
        {
            if (ModelState.IsValid)
            {
                var model = new WarehouseModel()
                {
                    Type = form.Type,
                    SubType = form.SubType,
                    Provider = form.Provider,
                    Manufacturer = form.Manufacturer,
                    Mode = ActionModes.Manufacturing,
                };

                var isSuccess = this.WarehouseHandler.AddWarehouse(model);
                if (isSuccess)
                {
                    return RedirectToAction(nameof(CreateNewWarehouseField));
                }

                throw new Exception("На складе уже есть сырье с такими параметрами");
            }

            return View(ModelState.ErrorCount);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpGet]
        public IActionResult CreateNewWarehouseType()
        {
            var viewModel = this.GetViewModel<WarehouseCreateNewWarehouseTypeViewModel>();

            viewModel.BackUrl = this.Url.Action("Index", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Добавить новый тип сырья";

            return View(viewModel);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpPost]
        public IActionResult CreateNewWarehouseType(WarehouseCreateNewWarehouseTypeForm form)
        {
            if (ModelState.IsValid)
            {
                var model = new WarehouseModel()
                {
                    Type = form.Type,
                    SubType = form.SubType,
                    Provider = form.Provider,
                    Manufacturer = form.Manufacturer,
                    Mode = ActionModes.Manufacturing,
                };

                var isSuccess = this.WarehouseHandler.AddWarehouse(model);
                if (isSuccess)
                {
                    return RedirectToAction(nameof(CreateNewWarehouseType));
                }

                throw new Exception("На складе уже есть сырье с такими параметрами");
            }

            return View(ModelState.ErrorCount);
        }

        public IActionResult WarehouseViewAll()
        {
            var viewModel = this.GetViewModel<WarehouseViewAllViewModel>();
            var handlerResult = this.WarehouseHandler.GetWarehouseList(ActionModes.Manufacturing);

            viewModel.Items = handlerResult.Items;
            viewModel.DistinctTypes = handlerResult.DistinctTypes;
            viewModel.DistinctSelectTypes = base.SelectListifyItems(handlerResult.DistinctTypes);
            viewModel.Type = handlerResult.DistinctTypes.First();
            viewModel.BackUrl = this.Url.Action("Index", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Складская таблица";

            return View(viewModel);
        }

        public IActionResult WarehouseFullInfo(WarehouseFullInfoForm form)
        {
            var viewModel = this.GetViewModel<WarehouseFullInfoViewModel>();
            var handlerResult = this.WarehouseHandler.GetFullWarehouseList(form.Type, form.SubType, form.Provider, form.Manufacturer, form.IndexName, form.Id, form.DateStart, form.DateFinish, form.ShowEmpty, form.ExpiredMode);

            viewModel.Items = handlerResult.Items;
            viewModel.TypeOfMaterialList = base.SelectListifyItemsWithNull(handlerResult.TypeList);
            viewModel.NameOfTypeMaterialList = base.SelectListifyItemsWithNull(handlerResult.SubTypeList);
            viewModel.ProviderList = base.SelectListifyItemsWithNull(handlerResult.ProviderList);
            viewModel.ManufacturerList = base.SelectListifyItemsWithNull(handlerResult.ManufacturerList);
            viewModel.ExpiredModeList = new List<SelectListItem>()
            {
                new SelectListItem() { Value = null, Text = "" },
                new SelectListItem() { Value = "1", Text = "Показать только акутальный срок годности" },
                new SelectListItem() { Value = "2", Text = "Показать только прошедший срок годности" },
            };
            viewModel.Form = form;

            viewModel.BackUrl = this.Url.Action("Index", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Подробная информация по складу сырья";

            return View(viewModel);
        }

        public IActionResult DownloadExcel()
        {
            var items = this.WarehouseHandler.GetDataTableForExcel();

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Лист 1");
                worksheet.Cells.LoadFromDataTable(items, true);
                worksheet.Column(9).Style.Numberformat.Format = "dd.mm.yyyy";
                worksheet.Column(10).Style.Numberformat.Format = "dd.mm.yyyy";
                worksheet.Column(11).Style.Numberformat.Format = "dd.mm.yyyy hh:mm:ss";
                worksheet.Cells.AutoFitColumns();
                worksheet.Cells[1, 1, 1, 11].AutoFilter = true;

                using (var heading = worksheet.Cells[1, 1, 1, 11])
                {
                    var colory = heading.Style.Fill;
                    colory.PatternType = ExcelFillStyle.Solid;
                    colory.BackgroundColor.SetColor(Color.LightBlue);
                    heading.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                }

                package.Save();
            }

            stream.Position = 0;
            var excelname = $"Warehouse.Index.{DateTime.Now}.xlsx";

            return File(stream, "application/vnd.openformats-officedocument.spreadsheetml.sheet", excelname);
        }

        #endregion

        #region Lab

        [HttpGet]
        public IActionResult LabCreateNewWarehouseField()
        {
            var viewModel = this.GetViewModel<WarehouseCreateNewWarehouseFieldViewModel>();

            viewModel.TypeOfMaterialList = base.SelectListifyItems(this.WarehouseHandler.GetDistinctTypeList(ActionModes.Laboratory));
            viewModel.BackUrl = this.Url.Action("Index", "Laboratory");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Лаборатория: Добавление новых полей";
            viewModel.IsLabMode = true;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult LabCreateNewWarehouseField(WarehouseCreateNewWarehouseFieldForm form)
        {
            if (ModelState.IsValid)
            {
                var model = new WarehouseModel()
                {
                    Type = form.Type,
                    SubType = form.SubType,
                    Provider = form.Provider,
                    Manufacturer = form.Manufacturer,
                    Mode = ActionModes.Laboratory,
                };

                var isSuccess = this.WarehouseHandler.AddWarehouse(model);
                if (isSuccess)
                {
                    return RedirectToAction(nameof(LabCreateNewWarehouseField));
                }

                throw new Exception("На складе уже есть сырье с такими параметрами");
            }

            return View(ModelState.ErrorCount);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpGet]
        public IActionResult LabCreateNewWarehouseType()
        {
            var viewModel = this.GetViewModel<WarehouseCreateNewWarehouseTypeViewModel>();

            viewModel.BackUrl = this.Url.Action("Index", "Laboratory");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Лаборатория: Добавить новый тип сырья";
            viewModel.IsLabMode = true;

            return View(viewModel);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpPost]
        public ActionResult LabCreateNewWarehouseType(WarehouseCreateNewWarehouseTypeForm form)
        {
            if (ModelState.IsValid)
            {
                var model = new WarehouseModel()
                {
                    Type = form.Type,
                    SubType = form.SubType,
                    Provider = form.Provider,
                    Manufacturer = form.Manufacturer,
                    Mode = ActionModes.Laboratory,
                };

                var isSuccess = this.WarehouseHandler.AddWarehouse(model);
                if (isSuccess)
                {
                    return RedirectToAction(nameof(LabCreateNewWarehouseType));
                }

                throw new Exception("На складе уже есть сырье с такими параметрами");
            }

            return View(ModelState.ErrorCount);
        }

        public IActionResult WarehouseViewLab(WarehouseLabViewAllForm form)
        {
            var viewModel = this.GetViewModel<WarehouseLabViewAllViewModel>();
            var mode = form.Mode ? ActionModes.Common : ActionModes.Laboratory;
            var handlerResult = this.WarehouseHandler.GetWarehouseList(mode);

            viewModel.Items = handlerResult.Items;
            viewModel.DistinctTypes = handlerResult.DistinctTypes;
            viewModel.Form = form;
            viewModel.BackUrl = this.Url.Action("Index", "Laboratory");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Лаборатория: Склад";
            viewModel.IsLabMode = true;

            return View(viewModel);
        }

        #endregion

        #region Json

        [HttpPost]
        public JsonResult GetSubTypeByTypeJson(string type, string Prefix, int mode)
        {
            var subTypesList = this.WarehouseHandler.GetDistinctSubTypeList(mode, type);
            var result = (from N in subTypesList
                          where N.Contains(Prefix)
                          select new { value = N });

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetTypeJson(string Prefix, int mode)
        {
            var typesList = this.WarehouseHandler.GetDistinctTypeList(mode);
            var result = (from N in typesList
                          where N.Contains(Prefix)
                          select new { value = N });

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetSubTypeJson(string Prefix, int mode)
        {
            var subTypesList = this.WarehouseHandler.GetDistinctSubTypeList(mode);
            var result = (from N in subTypesList
                          where N.Contains(Prefix)
                          select new { value = N });

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetProviderJson(string Prefix, int mode)
        {
            var providersList = this.WarehouseHandler.GetDistinctProviderList(mode);
            var result = (from N in providersList
                          where N.Contains(Prefix)
                          select new { value = N });

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetManufacturerJson(string Prefix, int mode)
        {
            var manufacturersList = this.WarehouseHandler.GetDistinctManufacturerList(mode);
            var result = (from N in manufacturersList
                          where N.Contains(Prefix)
                          select new { value = N });

            return Json(result);
        }

        #endregion
    }
}
