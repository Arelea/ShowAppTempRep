using AppNov14.Models.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using AppNov14.Handlers.Result.Goods;

namespace AppNov14.Handlers.Interfaces
{
    public interface IGoodsHandler : IBaseDataHandler
    {
        MethodResult AddCustomerName(string name);

        GetBatchListResult GetBatchList(List<int> statusesForDisplay, int statusId, int? batchId);

        GetCompletedBatchResult GetCompletedBatch(int id);

        MethodResult MoveCompletedBatch(int id, int initialPackage, decimal initialQuantity, DateTime completionDate, string remark);

        GetBatchFullInfoResult GetBatchFullInfo(int? batchId);

        GetBatchForSellResult GetBatchForSell(int? batchId);

        MethodResult SellBatch(int id, int package, decimal quantity, int customerId, DateTime soldDate, string remark);

        GetBatchForReturnResult GetBatchForReturn(int? batchId);

        MethodResult ReturnBatch(int id, int package, decimal quantity, int customerId, DateTime returnDate, string remark);

        GetBatchForMergeResult GetBatchForMerge(int? batchId);

        MethodResult MergeBatch(int id, int package, decimal quantity, int parentBatchId, DateTime date, string remark);

        GetBatchForThrowResult GetBatchForThrow(int? batchId);

        MethodResult ThrowBatch(int id, string remark);

        GetViewBatchWarehouseResult GetViewBatchWarehouse(string query, int? batchId, int? typeId, int? lineId, int? statusId);

        Dictionary<int, string> GetFilteredBatches(int? statusId, int? typeId, int? lineId);

        GetSellingsListResult GetDataByHistoryList(string query, int? batchId, int? customerId, DateTime? dateFrom, DateTime? dateTo, int actionHistoryType);

        Dictionary<int, string> GetBatchesByCustomer(int? customerId, int actionHistoryType);

        DataTable GetBatchWarehouseExcel();

        DataTable DownloadSellingsExcel();

        GetBatchForEditHistoryResult GetBatchForEditHistory(int id, int action, int historyId);

        MethodResult EditHistory(int batchId, int historyId, int action, decimal quantity, int package, int? customerId, int? parentBatchId);

        MethodResult DeleteHistory(int batchId, int action, int historyId);
    }
}