using AppNov14.Handlers.Interfaces;
using AppNov14.Models.Base;
using AppNov14.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppNov14.Handlers.Result.Goods;
using AppNov14.Helpers;

namespace AppNov14.Handlers.Goods
{
    public sealed class GoodsHandler : BaseDataHandler, IGoodsHandler
    {
        private readonly IBatchRepository BatchRepository;

        private readonly IBaseDataRepository BaseDataRepository;

        private readonly IManufacturingRepository ManufacturingRepository;

        public GoodsHandler(IBatchRepository batchRepository, IManufacturingRepository manufacturingRepository, IBaseDataRepository baseDataRepository) : base(baseDataRepository)
        {
            this.BatchRepository = batchRepository;
            this.BaseDataRepository = baseDataRepository;
            this.ManufacturingRepository = manufacturingRepository;
        }

        public MethodResult AddCustomerName(string name)
        {
            return this.BatchRepository.AddCustomerName(name);
        }

        public GetBatchListResult GetBatchList(List<int> statusesForDisplay, int statusId, int? batchId)
        {
            var result = new GetBatchListResult();
            result.Items = this.BatchRepository.GetBatchList(new List<int>() { statusId }, batchId);
            result.BatchStatusDictionary = this.BaseDataRepository.GetBatchStatuses(statusesForDisplay);
            result.BatchDictionary = this.BaseDataRepository.GetBatches(statusId);

            return result;
        }

        public GetCompletedBatchResult GetCompletedBatch(int id)
        {
            var result = new GetCompletedBatchResult();
            result.Batch = this.BatchRepository.GetDisplayBatch(id);

            return result;
        }

        public MethodResult MoveCompletedBatch(int id, int initialPackage, decimal initialQuantity, DateTime completionDate, string remark)
        {
            return this.BatchRepository.MoveCompletedBatch(id, initialPackage, initialQuantity, completionDate, remark);
        }

        public GetBatchFullInfoResult GetBatchFullInfo(int? batchId)
        {
            var result = new GetBatchFullInfoResult();
            if (batchId.HasValue)
            {
                result.Batch = this.BatchRepository.GetFullDisplayBatchWithCompounds(batchId.Value);
            }

            return result;
        }

        public GetBatchForSellResult GetBatchForSell(int? batchId)
        {
            var result = new GetBatchForSellResult();
            if (batchId.HasValue)
            {
                result.Batch = this.BatchRepository.GetExtendedDisplayBatch(batchId.Value);
                result.Customers = this.BaseDataRepository.GetCustomers();
            }

            return result;
        }

        public MethodResult SellBatch(int id, int package, decimal quantity, int customerId, DateTime soldDate, string remark)
        {
            return this.BatchRepository.SellBatch(id, package, quantity, customerId, soldDate, remark);
        }

        public GetBatchForReturnResult GetBatchForReturn(int? batchId)
        {
            var result = new GetBatchForReturnResult();
            if (batchId.HasValue)
            {
                var batch = this.BatchRepository.GetReturnDisplayBatch(batchId.Value);
                result.Batch = batch;
                if (batch.ValueList.Any())
                {
                    result.Customers = batch.ValueList.ToDictionary(m => m.CustomerId, m => m.CustomerName);
                }
            }

            return result;
        }

        public MethodResult ReturnBatch(int id, int package, decimal quantity, int customerId, DateTime returnDate, string remark)
        {
            return this.BatchRepository.ReturnBatch(id, package, quantity, customerId, returnDate, remark);
        }

        public GetBatchForMergeResult GetBatchForMerge(int? batchId)
        {
            var result = new GetBatchForMergeResult();
            if (batchId.HasValue)
            {
                result.Batch = this.BatchRepository.GetExtendedDisplayBatch(batchId.Value);
                result.ParentBatches = this.BatchRepository.GetBatchListWithoutChilds(new List<int>() { BatchStatuses.InManufacturingProcess, BatchStatuses.Completed }, batchId.Value).ToDictionary(m => m.Id, m => m.Name);
            }

            return result;
        }

        public MethodResult MergeBatch(int id, int package, decimal quantity, int parentBatchId, DateTime date, string remark)
        {
            return this.BatchRepository.MergeBatch(id, package, quantity, parentBatchId, date, remark);
        }

        public GetBatchForThrowResult GetBatchForThrow(int? batchId)
        {
            var result = new GetBatchForThrowResult();
            if (batchId.HasValue)
            {
                result.Batch = this.BatchRepository.GetExtendedDisplayBatch(batchId.Value);
            }

            return result;
        }

        public MethodResult ThrowBatch(int id, string remark)
        {
            return this.BatchRepository.ThrowBatch(id, remark);
        }

        public GetViewBatchWarehouseResult GetViewBatchWarehouse(string query, int? batchId, int? typeId, int? lineId, int? statusId)
        {
            var result = new GetViewBatchWarehouseResult();
            result.BatchNameDictionary = this.BaseDataRepository.GetFilteredBatches(typeId, lineId, statusId);
            result.BatchLineDictionary = this.BaseDataRepository.GetBatchLines();
            result.BatchTypeDictionary = this.BaseDataRepository.GetBatchTypes();
            result.BatchStatusDictionary = this.BaseDataRepository.GetBatchStatuses();

            result.Items = this.BatchRepository.GetExtendedDisplayBatchList(query, batchId, typeId, lineId, statusId);

            return result;
        }

        public Dictionary<int, string> GetFilteredBatches(int? statusId, int? typeId, int? lineId)
        {
            return this.BaseDataRepository.GetFilteredBatches(typeId, lineId, statusId);
        }

        public GetSellingsListResult GetDataByHistoryList(string query, int? batchId, int? customerId, DateTime? dateFrom, DateTime? dateTo, int actionHistoryType)
        {
            var result = new GetSellingsListResult();
            result.BatchNameDictionary = this.BaseDataRepository.GetBatchesByCustomer(customerId, actionHistoryType);
            result.CustomersDictionary = this.BaseDataRepository.GetTypedCustomers(actionHistoryType);

            result.Items = this.BatchRepository.GetDataByHistoryList(query, batchId, customerId, dateFrom, dateTo, actionHistoryType);

            return result;
        }

        public Dictionary<int, string> GetBatchesByCustomer(int? customerId, int actionHistoryType)
        {
            return this.BaseDataRepository.GetBatchesByCustomer(customerId, actionHistoryType);
        }

        public DataTable GetBatchWarehouseExcel()
        {
            return this.BatchRepository.GetBatchWarehouseExcel();
        }

        public DataTable DownloadSellingsExcel()
        {
            return this.BatchRepository.DownloadSellingsExcel();
        }

        public GetBatchForEditHistoryResult GetBatchForEditHistory(int id, int action, int historyId)
        {
            var result = new GetBatchForEditHistoryResult();

            result.Batch = this.BatchRepository.GetBatchForEditHistory(id, action, historyId);
            if (action == BatchHistoryActions.SellAction || action == BatchHistoryActions.ReturnFromCustomerAction)
            {
                result.Customers = this.BaseDataRepository.GetCustomers();
            }
            if (action == BatchHistoryActions.MergeToOtherAction)
            {
                result.ParentBatches = this.BatchRepository.GetBatchListWithoutChilds(new List<int>() { BatchStatuses.InManufacturingProcess, BatchStatuses.Completed, BatchStatuses.Empty, BatchStatuses.NoData }, id).ToDictionary(m => m.Id, m => m.Name);
            }

            return result;

        }

        public MethodResult EditHistory(int batchId, int historyId, int action, decimal quantity, int package, int? customerId, int? parentBatchId)
        {
            if (action == BatchHistoryActions.SellAction && customerId.HasValue)
            {
                return this.BatchRepository.EditSellAction(batchId, historyId, quantity, package, customerId.Value);
            }
            else if (action == BatchHistoryActions.ReturnFromCustomerAction && customerId.HasValue)
            {
                return this.BatchRepository.EditReturnFromCustomerAction(batchId, historyId, quantity, package, customerId.Value);
            }
            //else if (action == BatchHistoryActions.MergeToOtherAction && parentBatchId.HasValue)
            //{
            //    return this.BatchRepository.EditMergeToOtherAction(batchId, historyId, quantity, package, parentBatchId.Value);
            //}
            else if (action == BatchHistoryActions.CompleteAction)
            {
                return this.BatchRepository.EditCompleteAction(batchId, historyId, quantity, package);
            }

            return new MethodResult(false, "Для другого типа действия редактирование не реализвано");
        }

        public MethodResult DeleteHistory(int batchId, int action, int historyId)
        {
            if (action == BatchHistoryActions.SellAction)
            {
                return this.BatchRepository.DeleteSellAction(batchId, historyId);
            }
            else if (action == BatchHistoryActions.ReturnFromCustomerAction)
            {
                return this.BatchRepository.DeleteReturnFromCustomerAction(batchId, historyId);
            }
            //else if (action == BatchHistoryActions.MergeToOtherAction)
            //{
            //    return this.BatchRepository.DeleteMergeToOtherAction(batchId, historyId);
            //}
            else if (action == BatchHistoryActions.CompleteAction)
            {
                return this.BatchRepository.DeleteCompleteAction(batchId, historyId);
            }

            return new MethodResult(false, "Для другого типа действия редактирование не реализвано");
        }
    }
}