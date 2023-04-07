using System;
using System.Collections.Generic;
using System.Data;
using AppNov14.Handlers.Interfaces;
using AppNov14.Handlers.Result.Manufacturing;
using AppNov14.Helpers;
using AppNov14.Models.Base;
using AppNov14.Models.Batch;
using AppNov14.Models.Manufacturing;
using AppNov14.Models.Warehouse;
using AppNov14.Repositories.Interfaces;

namespace AppNov14.Handlers.ManufacturingTable
{
    public sealed class ManufacturingHandler : BaseDataHandler, IManufacturingHandler
    {
        private readonly IManufacturingRepository ManufacturingRepository;

        private readonly IWarehouseRepository WarehouseRepository;

        private readonly IBaseDataRepository BaseDataRepository;

        private readonly IBatchRepository BatchRepository;

        public ManufacturingHandler(IBatchRepository batchRepository, IManufacturingRepository manufacturingRepository, IWarehouseRepository warehouseRepository, IBaseDataRepository baseDataRepository) : base(baseDataRepository)
        {
            ManufacturingRepository = manufacturingRepository;
            WarehouseRepository = warehouseRepository;
            BaseDataRepository = baseDataRepository;
            BatchRepository = batchRepository;
        }

        public IncomingOrderResult GetIncomingOrder()
        {
            var result = new IncomingOrderResult
            {
                TypeList = this.GetDistinctTypeList(ActionModes.Manufacturing),
            };

            return result;
        }

        public MethodResult ReplenishWarehouse(ManufacturingRecordReplenishModel model)
        {
            var warehouseId = this.WarehouseRepository.GetId(model.Type, model.SubType, model.Provider, model.Manufacturer, ActionModes.Manufacturing);
            if (warehouseId == null)
            {
                return new MethodResult(false, ErrorMessages.WarehouseIdNotFound);
            }

            model.WarehouseId = warehouseId.Value;
            var result = this.ManufacturingRepository.ReplenishWarehouse(model);
           
            return result;
        }

        public OutcomingOrderResult GetOutcomingOrder(int? id)
        {
            var result = new OutcomingOrderResult
            {
                TypeList = this.GetDistinctTypeList(ActionModes.Manufacturing),
                BatchLineDictionary = this.GetBatchLines(),
                BatchTypeDictionary = this.GetBatchTypes(),
                BatchDictionary = this.GetBatches(id.HasValue ? BatchStatuses.InManufacturingProcess : (int?)null),
            };

            if (id.HasValue)
            {
                result.Item = this.ManufacturingRepository.GetItemForCopy(id.Value);
                result.SubTypeList = this.GetDistinctSubTypeList(ActionModes.Manufacturing, result.Item.Type);
                result.ProviderList = this.GetDistinctProviderList(ActionModes.Manufacturing, result.Item.Type, result.Item.SubType);
                result.ManufacturerList = this.GetDistinctManufacturerList(ActionModes.Manufacturing, result.Item.Type, result.Item.SubType, result.Item.Provider);   
                result.IndexDictionary = this.GetDistinctFilledIndexationList(result.Item.Type, result.Item.SubType, result.Item.Provider, result.Item.Manufacturer);
            }

            return result;
        }

        public MethodResult WriteOffWarehouse(ManufacturingRecordWriteOffModel model)
        {
            var warehouseId = this.WarehouseRepository.GetId(model.Type, model.SubType, model.Provider, model.Manufacturer, ActionModes.Manufacturing);
            if (warehouseId == null)
            {
                return new MethodResult(false, ErrorMessages.WarehouseIdNotFound);
            }

            var isIndexExist = this.WarehouseRepository.IsIndexExist(model.IndexId, warehouseId.Value);
            if (!isIndexExist)
            {
                return new MethodResult(false, ErrorMessages.IndexIdNotFound);
            }

            if (!model.IsNewBatch)
            {
                var isBatchExist = this.BatchRepository.IsBatchExist(model.BatchId);
                if (!isBatchExist)
                {
                    return new MethodResult(false, ErrorMessages.BatchNotFound);
                }
            }
            else
            {
                var isBatchExist = this.BatchRepository.IsBatchExistByName(model.Batch.Name);
                if (isBatchExist)
                {
                    return new MethodResult(false, ErrorMessages.DocumentNumberAlreadyExist);
                }
            }

            model.WarehouseId = warehouseId.Value;

            var result = this.ManufacturingRepository.WriteOffWarehouse(model);

            return result;
        }

        public List<ManufacturingRecordDisplayModel> GetEmployeeLastAdding(string employee)
        {
            return this.ManufacturingRepository.GetEmployeeLastAdding(employee);
        }

        public List<ManufacturingRecordDisplayModel> GetEveryoneLastAdding()
        {
            return this.ManufacturingRepository.GetEveryoneLastAdding();
        }

        public List<ManufacturingRecordDisplayModel> GetItemsByStartAndEndDate(DateTime dateStart, DateTime dateEnd)
        {
            return this.ManufacturingRepository.GetItemsByStartAndEndDate(dateStart, dateEnd);
        }

        public ManufacturingRecordDisplayModel GetById(int id)
        {
            return this.ManufacturingRepository.GetById(id);
        }

        public EditResult GetForEdit(int id)
        {
            var result = new EditResult();

            var record = this.ManufacturingRepository.GetById(id);
            result.Record = record;
            result.TypeList = this.GetDistinctTypeList(ActionModes.Manufacturing);
            result.SubTypeList = this.GetDistinctSubTypeList(ActionModes.Manufacturing, record.Type);
            result.ProviderList = this.GetDistinctProviderList(ActionModes.Manufacturing, record.Type, record.SubType);
            result.ManufacturerList = this.GetDistinctManufacturerList(ActionModes.Manufacturing, record.Type, record.SubType, record.Provider);
            if (record.ActionType == RawActionTypes.WriteOff)
            {
                result.IndexDictionary = this.GetDistinctFilledIndexationList(record.Type, record.SubType, record.Provider, record.Manufacturer);
                result.BatchNameDictionary = this.GetBatches(null);
            }

            return result;
        }

        public MethodResult EditReplenish(ManufacturingRecordReplenishModel model, int recordId)
        {
            if (model.ActionType == RawActionTypes.Return)
            {
                return new MethodResult(false, ErrorMessages.ReturnEditingNotAllowed);
            }

            var warehouseIdNew = this.WarehouseRepository.GetId(model.Type, model.SubType, model.Provider, model.Manufacturer, ActionModes.Manufacturing);
            var warehouseIdOld = this.WarehouseRepository.GetIdById(recordId, ActionModes.Manufacturing);
            if (warehouseIdNew == null || warehouseIdOld == null)
            {
                return new MethodResult(false, ErrorMessages.WarehouseIdNotFound);
            }
            model.WarehouseId = warehouseIdNew.Value;
            var result = this.ManufacturingRepository.EditReplenish(model, recordId);

            return result;
        }

        public MethodResult EditWriteOff(ManufacturingRecordWriteOffModel model, int recordId)
        {
            if (model.ActionType == RawActionTypes.Return)
            {
                return new MethodResult(false, ErrorMessages.ReturnEditingNotAllowed);
            }

            var warehouseIdNew = this.WarehouseRepository.GetId(model.Type, model.SubType, model.Provider, model.Manufacturer, ActionModes.Manufacturing);
            var warehouseIdOld = this.WarehouseRepository.GetIdById(recordId, ActionModes.Manufacturing);
            if (warehouseIdNew == null || warehouseIdOld == null)
            {
                return new MethodResult(false, ErrorMessages.WarehouseIdNotFound);
            }

            var isIndexExist = this.WarehouseRepository.IsIndexExist(model.IndexId, warehouseIdNew.Value);
            if (!isIndexExist)
            {
                return new MethodResult(false, ErrorMessages.IndexIdNotFound);
            }
            model.WarehouseId = warehouseIdNew.Value;
            var result = this.ManufacturingRepository.EditWriteOff(model, recordId);

            return result;
        }

        public MethodResult Delete(int id)
        {
            var warehouseIdOld = this.WarehouseRepository.GetIdById(id, ActionModes.Manufacturing);
            if (warehouseIdOld == null)
            {
                return new MethodResult(false, ErrorMessages.WarehouseIdNotFound);
            }

            var result = this.ManufacturingRepository.Delete(id);

            return result;
        }

        public int GetRecordCountByDate(DateTime dateStart, DateTime dateEnd)
        {
            return this.ManufacturingRepository.GetRecordCountByDate(dateStart, dateEnd);
        }

        public DataTable GetDataTableForExcel(DateTime dateStart, DateTime dateEnd)
        {
            return this.ManufacturingRepository.GetDataTableForExcel(dateStart, dateEnd);
        }

        public BatchInfoResult GetBatchInfo(int? batchId)
        {
            var result = new BatchInfoResult();
            result.BatchList = this.BaseDataRepository.GetBatches();
            if (batchId.HasValue)
            {
                result.BatchInformation = this.BatchRepository.GetDisplayBatchWithCompounds(batchId.Value);
            }

            return result;
        }

        public IndexInfoResult GetIndexInfo(int? indexId)
        {
            var result = new IndexInfoResult();
            if (indexId.HasValue)
            {
                result.Item = this.WarehouseRepository.GetDisplayIndex(indexId.Value);
                result.LinkedBatches = this.BatchRepository.GetBatchesByIndexId(indexId.Value);
            }

            return result;
        }

        public MethodResult AddBatchType(string name)
        {
            return this.BatchRepository.AddBatchType(name);
        }

        public ViewIndexResult GetViewIndex(string type, string subType, string provider, string manufacturer, string indexName, bool byWarehouse)
        {
            var result = new ViewIndexResult();
            result.Items = new List<IndexDisplayDataModel>();
            result.TypeList = this.GetDistinctTypeList(ActionModes.Manufacturing);
            if (!string.IsNullOrEmpty(type))
            {
                result.SubTypeList = this.GetDistinctSubTypeList(ActionModes.Manufacturing, type);
            }
            if (!string.IsNullOrEmpty(subType))
            {
                result.ProviderList = this.GetDistinctProviderList(ActionModes.Manufacturing, type, subType);
            }
            if (!string.IsNullOrEmpty(provider))
            {
                result.ManufacturerList = this.GetDistinctManufacturerList(ActionModes.Manufacturing, type, subType, provider);
            }

            result.Items = this.WarehouseRepository.GetIndexationByWarehouseAndName(type, subType, provider, manufacturer, indexName, byWarehouse);

            return result;
        }

        public MethodResult RestoreIndexDates(int id, DateTime? expirationDate, DateTime? manufacturingDate)
        {
            return this.WarehouseRepository.RestoreIndexDates(id, expirationDate, manufacturingDate);
        }

        public SearchResult Search(ManufacturingSearchModel model)
        {
            var result = new SearchResult();
            result.Items = this.ManufacturingRepository.Search(model);
            result.TypeList = this.GetDistinctTypeList(ActionModes.Manufacturing);
            result.SubTypeList = this.GetDistinctSubTypeList(ActionModes.Manufacturing);
            result.ProviderList = this.GetDistinctProviderList(ActionModes.Manufacturing);
            result.ManufacturerList = this.GetDistinctManufacturerList(ActionModes.Manufacturing);
           
            result.ReplenishmentDocumentDictionary = this.BaseDataRepository.GetReplenishmentDocuments();             
            result.BatchDictionary = this.BaseDataRepository.GetBatches();
            result.BatchTypeDictionary = this.BaseDataRepository.GetBatchTypes();
            result.BatchLineDictionary = this.BaseDataRepository.GetBatchLines();        

            return result;
        }

        public EditBatchListResult GetBatchesForEdit(int statusId, int? batchId)
        {
            var result = new EditBatchListResult();
            result.Items = this.BatchRepository.GetBatchList(new List<int>() { statusId }, batchId);            
            result.BatchStatusDictionary = this.BaseDataRepository.GetBatchStatuses();
            result.BatchDictionary = this.BaseDataRepository.GetBatches(statusId);

            return result;
        }

        public EditBatchResult GetBatchForEdit(int id)
        {
            var result = new EditBatchResult();
            result.Record = this.BatchRepository.GetBatch(id);
            result.BatchStatusDictionary = this.BaseDataRepository.GetBatchStatuses();
            result.BatchTypeDictionary = this.BaseDataRepository.GetBatchTypes();
            result.BatchLineDictionary = this.BaseDataRepository.GetBatchLines();

            return result;
        }

        public MethodResult EditBatch(BatchModel model)
        {
            return this.BatchRepository.EditBatch(model);
        }
    }
}