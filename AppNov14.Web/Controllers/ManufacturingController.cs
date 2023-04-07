using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using AppNov14.Helpers;
using AppNov14.Web.ViewModels.ManufacturingTable;
using AppNov14.Handlers.Interfaces;
using Castle.Core.Internal;
using AppNov14.Models.Manufacturing;
using AppNov14.Models.Batch;
using AppNov14.Models.Base;

namespace AppNov14.Web.Controllers
{
    [Authorize]
    public class ManufacturingController : BaseController
    {
        private readonly IManufacturingHandler ManufacturingHandler;

        public ManufacturingController(IManufacturingHandler manufacturingHandler, IBaseDataHandler baseDataHandler) : base(baseDataHandler)
        {
            this.ManufacturingHandler = manufacturingHandler;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = this.GetViewModel<ManufacturingRecordIndexViewModel>();

            viewModel.BackUrl = this.Url.Action("Index", "Home");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Производство";

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult CreateIncomingOrder()
        {
            var viewModel = this.GetViewModel<CreateIncomingOrderViewModel>();
            var handlerResult = this.ManufacturingHandler.GetIncomingOrder();

            viewModel.TypeList = base.SelectListifyItems(handlerResult.TypeList);
            viewModel.BackUrl = this.Url.Action("Index", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Поступление нового сырья на склад";

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateIncomingOrder(CreateIncomingOrderForm form)
        {
            if (ModelState.IsValid)
            {
                if (form.Quantity <= 0)
                {
                    throw new Exception(ErrorMessages.ZeroNotAllowed);
                }
                var model = new ManufacturingRecordReplenishModel()
                {
                    Type = form.Type,
                    SubType = form.SubType,
                    Provider = form.Provider,
                    Manufacturer = form.Manufacturer,
                    Quantity = form.Quantity,
                    ReplenishmentDocument = form.ReplenishmentDocument,
                    Employee = User.Identity.Name,
                    Index = form.Index,
                    DocDate = form.DocDate,
                    IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    InsertDate = DateTime.Now,
                    Remarks = form.Remarks,
                    ActionType = RawActionTypes.Replenishment,
                    ManufacturingDate = form.ManufacturingDate,
                    ExpirationDate = form.ExpirationDate,
                };

                var result = this.ManufacturingHandler.ReplenishWarehouse(model);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(CreateIncomingOrder));
                }

                throw new Exception(result.Message);
            }

            return View(ModelState.ErrorCount);
        }

        public IActionResult Copy(int? id)
        {
            if (id.HasValue)
            {
                return this.RedirectToAction(nameof(CreateOutcomingOrder), new { id = id });
            }

            return this.RedirectToAction(nameof(MyLastAddings));
        }

        [HttpGet]
        public IActionResult CreateOutcomingOrder(int? id)
        {
            var viewModel = this.GetViewModel<CreateOutcomingOrderViewModel>();
            var handlerResult = this.ManufacturingHandler.GetOutcomingOrder(id);

            viewModel.BatchTypeList = base.SelectListifyItemsFromDictionary(handlerResult.BatchTypeDictionary);
            viewModel.BatchLineList = base.SelectListifyItemsFromDictionary(handlerResult.BatchLineDictionary);
            viewModel.BatchList = base.SelectListifyItemsFromDictionaryWithNull(handlerResult.BatchDictionary);
            viewModel.TypeList = base.SelectListifyItems(handlerResult.TypeList);
            viewModel.BackUrl = this.Url.Action("Index", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Списание сырья на производство";
            if (handlerResult.Item != null)
            {
                viewModel.Form = this.CreateForm<CreateOutcomingOrderForm>();
                viewModel.Form.Fill(handlerResult.Item);
                viewModel.SubTypeList = base.SelectListifyItemsWithNull(handlerResult.SubTypeList);
                viewModel.ProviderList = base.SelectListifyItemsWithNull(handlerResult.ProviderList);
                viewModel.ManufacturerList = base.SelectListifyItemsWithNull(handlerResult.ManufacturerList);
                viewModel.IndexList = base.SelectListifyItemsFromDictionaryWithNull(handlerResult.IndexDictionary);
            }          

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateOutcomingOrder(CreateOutcomingOrderForm form)
        {
            if (form.IsNewBatch)
            {
                ModelState.Remove("form.BatchId");
            }
            else
            {
                ModelState.Remove("form.BatchName");
                ModelState.Remove("form.BatchLineId");
                ModelState.Remove("form.BatchTypeId");
                ModelState.Remove("form.CreateDate");
            }

            if (ModelState.IsValid)
            {
                if (form.Quantity <= 0)
                {
                    throw new Exception(ErrorMessages.ZeroNotAllowed);
                }
                var model = new ManufacturingRecordWriteOffModel()
                {
                    Type = form.Type,
                    SubType = form.SubType,
                    Provider = form.Provider,
                    Manufacturer = form.Manufacturer,
                    Quantity = form.Quantity,
                    Employee = User.Identity.Name,
                    IndexId = form.IndexId,
                    DocDate = form.DocDate,
                    IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    ActionType = RawActionTypes.WriteOff,
                    InsertDate = DateTime.Now,
                    Remarks = form.Remarks,
                    IsNewBatch = form.IsNewBatch,
                };

                if (form.IsNewBatch)
                {
                    model.Batch = new BatchModel()
                    {
                        TypeId = form.BatchTypeId,
                        LineId = form.BatchLineId,
                        Name = form.BatchName,
                        InsertDate = DateTime.Now,
                        StatusId = BatchStatuses.InManufacturingProcess,
                        CreateDate = form.CreateDate,
                    };
                }
                else
                {
                    model.BatchId = form.BatchId;
                }

                var result = this.ManufacturingHandler.WriteOffWarehouse(model);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(CreateOutcomingOrder));
                }

                throw new Exception(result.Message);
            }

            return View(ModelState.ErrorCount);
        }

        [HttpGet]
        public IActionResult CreateCorrectionOutcomingOrder()
        {
            var viewModel = this.GetViewModel<CreateCorrectionOutcomingOrderViewModel>();
            var handlerResult = this.ManufacturingHandler.GetOutcomingOrder(null);

            viewModel.BatchList = base.SelectListifyItemsFromDictionaryWithNull(handlerResult.BatchDictionary);
            viewModel.TypeList = base.SelectListifyItems(handlerResult.TypeList);
            viewModel.BackUrl = this.Url.Action("Index", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Списание сырья на производство (Корректировка)";

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateCorrectionOutcomingOrder(CreateCorrectionOutcomingOrderForm form)
        {
            if (ModelState.IsValid)
            {
                if (form.Quantity <= 0)
                {
                    throw new Exception(ErrorMessages.ZeroNotAllowed);
                }
                var model = new ManufacturingRecordWriteOffModel()
                {
                    Type = form.Type,
                    SubType = form.SubType,
                    Provider = form.Provider,
                    Manufacturer = form.Manufacturer,
                    Quantity = form.Quantity,
                    Employee = User.Identity.Name,
                    IndexId = form.IndexId,
                    DocDate = form.DocDate,
                    IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    ActionType = RawActionTypes.WriteOff,
                    InsertDate = DateTime.Now,
                    Remarks = form.Remarks,
                    IsNewBatch = false,
                    BatchId = form.BatchId,
                };

                var result = this.ManufacturingHandler.WriteOffWarehouse(model);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(CreateCorrectionOutcomingOrder));
                }

                throw new Exception(result.Message);
            }

            return View(ModelState.ErrorCount);
        }

        [HttpGet]
        public IActionResult MyLastAddings()
        {
            var viewModel = this.GetViewModel<MyLastAddingViewModel>();
            var items = this.ManufacturingHandler.GetEmployeeLastAdding(User.Identity.Name);

            viewModel.Items = items;
            viewModel.BackUrl = this.Url.Action("Index", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Мои последние записи";

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult EveryoneLastAddings()
        {

            var viewModel = this.GetViewModel<EveryoneLastAddingsViewModel>();
            var items = this.ManufacturingHandler.GetEveryoneLastAdding();

            viewModel.Items = items;
            viewModel.BackUrl = this.Url.Action("Index", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Общие последние записи";

            return View(viewModel);
        }

        [Authorize(Roles = "Moderator, Admin")]
        public IActionResult EditList(EditDeleteAllDataForm form)
        {
            var viewModel = this.GetViewModel<EditListViewModel>();
            if (form.DateStart == DateTime.MinValue || form.DateFinish == DateTime.MinValue)
            {
                form.DateStart = DateTime.Now.AddMonths(Other.EditFromMonths);
                form.DateFinish = DateTime.Now.AddDays(Other.EditToDays);
            }
            var items = this.ManufacturingHandler.GetItemsByStartAndEndDate(form.DateStart, form.DateFinish);

            viewModel.Form = form;
            viewModel.Items = items;
            viewModel.BackUrl = this.Url.Action("Index", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Записи главной таблицы для редактирования";

            return View(viewModel);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var viewModel = this.GetViewModel<EditViewModel>();
            var handlerResult = this.ManufacturingHandler.GetForEdit(id.Value);
            if (handlerResult.Record == null)
            {
                throw new Exception(ErrorMessages.RecordNotFound);
            }

            var form = this.CreateForm<EditForm>();
            form.Fill(handlerResult.Record);

            viewModel.Form = form;
            viewModel.TypeList = base.SelectListifyItems(handlerResult.TypeList);
            viewModel.SubTypeList = base.SelectListifyItems(handlerResult.SubTypeList);
            viewModel.ProviderList = base.SelectListifyItems(handlerResult.ProviderList);
            viewModel.ManufacturerList = base.SelectListifyItems(handlerResult.ManufacturerList);
            if (handlerResult.Record.ActionType == RawActionTypes.WriteOff)
            {
                viewModel.IndexList = base.SelectListifyItemsFromDictionary(handlerResult.IndexDictionary);
                viewModel.BatchList = base.SelectListifyItemsFromDictionary(handlerResult.BatchNameDictionary);
            }

            viewModel.BackUrl = this.Url.Action("EditList", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Редактор записей таблицы";

            return View(viewModel);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpPost]
        public IActionResult Edit(EditForm form)
        {
            if (form.ActionType == RawActionTypes.Replenishment)
            {
                ModelState.Remove("form.BatchId");
                ModelState.Remove("form.IndexId");
            }
            else
            {
                ModelState.Remove("form.Index");
                ModelState.Remove("form.ReplenishmentDocument");
            }

            if (ModelState.IsValid)
            {
                var result = new MethodResult();
                if (form.ActionType == RawActionTypes.Replenishment)
                {
                    var model = new ManufacturingRecordReplenishModel()
                    {
                        Type = form.Type,
                        SubType = form.SubType,
                        Provider = form.Provider,
                        Manufacturer = form.Manufacturer,
                        Quantity = form.Quantity,
                        ReplenishmentDocument = form.ReplenishmentDocument,
                        Employee = Other.UpdatedBy + User.Identity.Name,
                        Index = form.Index,
                        DocDate = form.DocDate,
                        IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                        Remarks = form.Remarks,
                        ActionType = form.ActionType,
                        ExpirationDate = form.ExpirationDate,
                        ManufacturingDate = form.ManufacturingDate,
                    };

                    result = this.ManufacturingHandler.EditReplenish(model, form.Id);
                }
                else if (true)
                {
                    var model = new ManufacturingRecordWriteOffModel()
                    {
                        Type = form.Type,
                        SubType = form.SubType,
                        Provider = form.Provider,
                        Manufacturer = form.Manufacturer,
                        Quantity = form.Quantity,
                        BatchId = form.BatchId.Value,
                        Employee = Other.UpdatedBy + User.Identity.Name,
                        IndexId = form.IndexId.Value,
                        DocDate = form.DocDate,
                        IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                        Remarks = form.Remarks,
                        ActionType = form.ActionType,
                    };

                    result = this.ManufacturingHandler.EditWriteOff(model, form.Id);
                }

                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(EditList));
                }

                throw new Exception(result.Message);
            }

            return View(ModelState.ErrorCount);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            var viewModel = this.GetViewModel<DeleteTableRowViewModel>();
            var record = this.ManufacturingHandler.GetById(id.Value);
            if (record == null)
            {
                throw new Exception(ErrorMessages.RecordNotFound);
            }

            var form = this.CreateForm<DeleteForm>();
            form.Fill(record);

            viewModel.Form = form;
            viewModel.BackUrl = this.Url.Action("EditList", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Удаление записи";

            return View(viewModel);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var result = this.ManufacturingHandler.Delete(id);
            if (result.IsSuccess)
            {
                return RedirectToAction(nameof(EditList));
            }

            throw new Exception(result.Message);
        }

        public IActionResult ImportExcelFile(ImportExcelFileForm form)
        {
            var viewModel = this.GetViewModel<ImportExcelFileViewModel>();
            if (form.DateStart == DateTime.MinValue || form.DateFinish == DateTime.MinValue)
            {
                form.DateStart = DateTime.Now.AddMonths(Other.EditFromMonths);
                form.DateFinish = DateTime.Now.AddDays(Other.EditToDays);
            }
            var handlerResult = this.ManufacturingHandler.GetRecordCountByDate(form.DateStart, form.DateFinish);
            
            viewModel.ResultsCount = handlerResult;
            viewModel.Form = form;
            viewModel.BackUrl = this.Url.Action("Index", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Скачать эксель выписку из базы";

            return View(viewModel);
        }

        public IActionResult DownloadExcel(string dateStart, string dateFinish)
        {
            var items = this.ManufacturingHandler.GetDataTableForExcel(Convert.ToDateTime(dateStart), Convert.ToDateTime(dateFinish));

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Лист 1");
                worksheet.Cells.LoadFromDataTable(items, true);
                worksheet.Column(13).Style.Numberformat.Format = "dd:mm:yyyy hh:mm:ss";
                worksheet.Column(16).Style.Numberformat.Format = "dd:mm:yyyy hh:mm:ss";
                worksheet.Cells.AutoFitColumns();
                worksheet.Cells[1, 1, 1, 19].AutoFilter = true;

                using (var heading = worksheet.Cells[1, 1, 1, 19])
                {
                    var colory = heading.Style.Fill;
                    colory.PatternType = ExcelFillStyle.Solid;
                    colory.BackgroundColor.SetColor(Color.LightBlue);
                    heading.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                }

                package.Save();
            }

            stream.Position = 0;
            var excelname = $"VestPlast.DataProd.{DateTime.Now}.xlsx";

            return File(stream, "application/vnd.openformats-officedocument.spreadsheetml.sheet", excelname);
        }

        public IActionResult BatchInfo(int? batchId)
        {
            var viewModel = this.GetViewModel<BatchInfoViewModel>();
            var handlerResult = this.ManufacturingHandler.GetBatchInfo(batchId);

            viewModel.BatchId = batchId;
            viewModel.BatchInformation = handlerResult.BatchInformation;
            viewModel.BatchList = base.SelectListifyItemsFromDictionary(handlerResult.BatchList);

            viewModel.BackUrl = this.Url.Action("Index", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Состав партии";

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult AddBatchType()
        {
            var viewModel = this.GetViewModel<AddBatchTypeViewModel>();

            viewModel.BackUrl = this.Url.Action("Index", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Добавление нового типа партии";

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddBatchType(string name)
        {
            if (!name.IsNullOrEmpty())
            {
                var result = this.ManufacturingHandler.AddBatchType(name);

                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(AddBatchType));
                }

                throw new Exception(result.Message);
            }

            throw new Exception();
        }

        public IActionResult ToIndexInformationList(string type, string subType, string provider, string manufacturer)
        {
            var form = this.CreateForm<ViewIndexForm>();
            form.Type = type;
            form.SubType = subType;
            form.Provider = provider;
            form.Manufacturer = manufacturer;
            form.ByWarehouse = true;

            return RedirectToAction("IndexInformationList", "Manufacturing", form);
        }

        public IActionResult IndexInformationList(ViewIndexForm form)
        {
            var viewModel = this.GetViewModel<ViewIndexViewModel>();
            var handlerResult = this.ManufacturingHandler.GetViewIndex(form.Type, form.SubType, form.Provider, form.Manufacturer, form.IndexName, form.ByWarehouse);

            viewModel.TypeOfMaterialList = base.SelectListifyItems(handlerResult.TypeList);
            viewModel.NameOfTypeMaterialList = base.SelectListifyItemsWithNull(handlerResult.SubTypeList);
            viewModel.ProviderList = base.SelectListifyItemsWithNull(handlerResult.ProviderList);
            viewModel.ManufacturerList = base.SelectListifyItemsWithNull(handlerResult.ManufacturerList);

            viewModel.Items = handlerResult.Items;
            viewModel.Form = form;
            viewModel.BackUrl = this.Url.Action("Index", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Список паспортов";

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult IndexInfo(int? indexId)
        {
            var viewModel = this.GetViewModel<IndexInfoViewModel>();
            var handlerResult = this.ManufacturingHandler.GetIndexInfo(indexId);

            viewModel.Item = handlerResult.Item;
            viewModel.LinkedBatches = handlerResult.LinkedBatches;

            viewModel.BackUrl = this.Url.Action("IndexInformationList", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Информация о паспорте";

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult RestoreIndexDates(int indexId, string name, string expirationDate, string manufacturingDate)
        {
            var viewModel = this.GetViewModel<RestoreIndexDatesViewModel>();
            viewModel.Form = this.CreateForm<RestoreIndexDatesForm>();

            viewModel.Name = name;
            viewModel.Form.Id = indexId;
            viewModel.Form.ExpirationDate = expirationDate.IsNullOrEmpty() ? (DateTime?)null : Convert.ToDateTime(expirationDate);
            viewModel.Form.ManufacturingDate = manufacturingDate.IsNullOrEmpty() ? (DateTime?)null : Convert.ToDateTime(manufacturingDate);

            viewModel.BackUrl = this.Url.Action("IndexInformationList", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Восстановить даты срока годности и изготовления";

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult RestoreIndexDates(RestoreIndexDatesForm form)
        {
            if (ModelState.IsValid || (!form.ExpirationDate.HasValue && !form.ManufacturingDate.HasValue))
            {
                var result = this.ManufacturingHandler.RestoreIndexDates(form.Id, form.ExpirationDate, form.ManufacturingDate);
                if (result.IsSuccess)
                {
                    return RedirectToAction("IndexInfo", "Manufacturing", new { indexId = form.Id });
                }

                throw new Exception(result.Message);
            }

            return View(ModelState.ErrorCount);
        }


        public IActionResult Search(SearchForm form)
        {
            var viewModel = this.GetViewModel<SearchViewModel>();
            var isReplenishment = form.ShowMode == RawActionTypes.Replenishment;
            var isWriteOff = form.ShowMode == RawActionTypes.WriteOff;
            var model = new ManufacturingSearchModel()
            {
                Id = form.Id,
                Type = form.Type,
                SubType = form.SubType,
                Provider = form.Provider,
                Manufacturer = form.Manufacturer,
                BatchTypeId = isWriteOff ? form.BatchTypeId : null,
                BatchId = isWriteOff ? form.BatchId : null,
                ReplenishmentDocumentId = isReplenishment ? form.ReplenishmentDocumentId : null,
                Index = form.Index,
                ShowMode = form.ShowMode,
                BatchLineId = isWriteOff ? form.BatchLineId : null,
                DateFinish = form.DateFinish,
                DateStart = form.DateStart,
            };

            var handlerResult = this.ManufacturingHandler.Search(model);
            viewModel.Items = handlerResult.Items;
            viewModel.Form = form;
            viewModel.TypesList = base.SelectListifyItemsWithNull(handlerResult.TypeList);
            viewModel.SubTypesList = base.SelectListifyItemsWithNull(handlerResult.SubTypeList);
            viewModel.ProviderList = base.SelectListifyItemsWithNull(handlerResult.ProviderList);
            viewModel.ManufacturerList = base.SelectListifyItemsWithNull(handlerResult.ManufacturerList);
    
            viewModel.ReplenishmentDocumentList = base.SelectListifyItemsFromDictionaryWithNull(handlerResult.ReplenishmentDocumentDictionary);

            viewModel.BatchList = base.SelectListifyItemsFromDictionaryWithNull(handlerResult.BatchDictionary);
            viewModel.BatchTypeList = base.SelectListifyItemsFromDictionaryWithNull(handlerResult.BatchTypeDictionary);
            viewModel.BatchLineList = base.SelectListifyItemsFromDictionaryWithNull(handlerResult.BatchLineDictionary);
            
            viewModel.ShowModeList = OperationTypes.OperationTypeList;
            viewModel.BackUrl = this.Url.Action("Index", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Поиск: Производство";

            return View(viewModel);
        }


        [Authorize(Roles = "Moderator, Admin")]
        public IActionResult EditBatchList(EditBatchListForm form)
        {
            var viewModel = this.GetViewModel<EditBatchListViewModel>();
            var handlerResult = this.ManufacturingHandler.GetBatchesForEdit(form.BatchStatusId, form.BatchId);

            viewModel.BatchList = base.SelectListifyItemsFromDictionaryWithNull(handlerResult.BatchDictionary);
            viewModel.BatchStatusList = base.SelectListifyItemsFromDictionary(handlerResult.BatchStatusDictionary);
            viewModel.Form = form;
            viewModel.Items = handlerResult.Items;
            viewModel.BackUrl = this.Url.Action("Index", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Список партий для редактирования";

            return View(viewModel);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpGet]
        public IActionResult EditBatch(int? id)
        {
            var viewModel = this.GetViewModel<EditBatchViewModel>();
            var handlerResult = this.ManufacturingHandler.GetBatchForEdit(id.Value);
            if (handlerResult.Record == null)
            {
                throw new Exception(ErrorMessages.RecordNotFound);
            }

            var form = this.CreateForm<EditBatchForm>();
            form.Fill(handlerResult.Record);

            viewModel.Form = form;
            viewModel.BatchTypeList = base.SelectListifyItemsFromDictionary(handlerResult.BatchTypeDictionary);
            viewModel.BatchLineList = base.SelectListifyItemsFromDictionary(handlerResult.BatchLineDictionary);
            viewModel.BatchStatusList = base.SelectListifyItemsFromDictionary(handlerResult.BatchStatusDictionary);

            viewModel.BackUrl = this.Url.Action("EditBatchList", "Manufacturing");
            viewModel.BackUrlName = "Назад";
            viewModel.PageName = "Редактор партий";

            return View(viewModel);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpPost]
        public IActionResult EditBatch(EditBatchForm form)
        {
            if (ModelState.IsValid)
            {
                var model = new BatchModel()
                {
                    TypeId = form.BatchTypeId,
                    LineId = form.BatchLineId,
                    Name = form.BatchName,
                    StatusId = form.BatchStatusId,
                    Id = form.Id,   
                    CreateDate = form.CreateDate,
                };

                var result = this.ManufacturingHandler.EditBatch(model);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(EditBatchList));
                }

                throw new Exception(result.Message);
            }

            return View(ModelState.ErrorCount);
        }

        #region Json

        [HttpPost]
        public JsonResult GetSubTypeByTypeJson(string type, int mode)
        {
            var subTypesList = base.SelectListifyItems(this.ManufacturingHandler.GetDistinctSubTypeList(mode, type));

            return Json(subTypesList);
        }

        [HttpPost]
        public JsonResult GetProviderByTypeAndSubTypeJson(string type, string subType, int mode)
        {
            var providerList = base.SelectListifyItems(this.ManufacturingHandler.GetDistinctProviderList(mode, type, subType));

            return Json(providerList);
        }

        [HttpPost]
        public JsonResult GetFilledProviderByTypeAndSubTypeJson(string type, string subType, int mode)
        {
            var providerList = base.SelectListifyItems(this.ManufacturingHandler.GetDistinctProviderList(mode, type, subType, isFilled: true));

            return Json(providerList);
        }

        [HttpPost]
        public JsonResult GetManufacturerByTypeAndSubTypeAndProviderJson(string type, string subType, string provider, int mode)
        {
            var manufacturerList = base.SelectListifyItems(this.ManufacturingHandler.GetDistinctManufacturerList(mode, type, subType, provider));

            return Json(manufacturerList);
        }

        [HttpPost]
        public JsonResult GetFilledManufacturerByTypeAndSubTypeAndProviderJson(string type, string subType, string provider, int mode)
        {
            var manufacturerList = base.SelectListifyItems(this.ManufacturingHandler.GetDistinctManufacturerList(mode, type, subType, provider, isFilled: true));

            return Json(manufacturerList);
        }

        [HttpPost]
        public JsonResult GetIndexationJson(string type, string subType, string provider, string manufacturer)
        {
            var indexationList = base.SelectListifyItemsFromDictionary(this.ManufacturingHandler.GetDistinctFilledIndexationList(type, subType, provider, manufacturer));

            return Json(indexationList);
        }

        [HttpPost]
        public JsonResult GetBatchByStatusId(int batchStatusId)
        {
            var batchList = base.SelectListifyItemsFromDictionary(this.ManufacturingHandler.GetBatches(batchStatusId));

            return Json(batchList);
        }

        [HttpPost]
        public JsonResult GetBatchTypesJson(string prefix)
        {
            var batchTypeList = this.ManufacturingHandler.GetBatchTypeList();
            var result = (from N in batchTypeList
                          where N.Contains(prefix)
                          select new { value = N });

            return Json(result);
        }

        #endregion

        #region JsonSearch

        [HttpPost]
        public JsonResult GetFullSubTypeJson(int mode)
        {
            var subTypesList = base.SelectListifyItemsWithNull(this.ManufacturingHandler.GetDistinctSubTypeList(mode));

            return Json(subTypesList);
        }

        #endregion  
    }
}