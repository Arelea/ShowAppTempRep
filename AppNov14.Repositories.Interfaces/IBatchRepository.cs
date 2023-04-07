using System;
using AppNov14.Models.Base;
using AppNov14.Models.Batch;
using System.Collections.Generic;
using System.Data;

namespace AppNov14.Repositories.Interfaces
{
    public interface IBatchRepository
    {
        MethodResult AddBatchType(string name);

        bool IsBatchExist(int batchId);

        bool IsBatchExistByName(string batchName);

        List<BatchDisplayModel> GetBatchList(List<int> statusIdList, int? batchId);

        List<BatchDisplayModel> GetBatchListWithoutChilds(List<int> statusIdList, int batchId);

        BatchModel GetBatch(int id);

        BatchDisplayModel GetDisplayBatch(int id);

        MethodResult EditBatch(BatchModel model);

        MethodResult AddCustomerName(string name);

        Dictionary<int, string> GetBatchesByIndexId(int indexId);

        MethodResult MoveCompletedBatch(int id, int initialPackage, decimal initialQuantity, DateTime completionDate, string remark);

        BatchDisplayModel GetDisplayBatchWithCompounds(int id);

        BatchFullDisplayModel GetFullDisplayBatchWithCompounds(int id);

        BatchExtendedDisplayModel GetExtendedDisplayBatch(int id);

        MethodResult SellBatch(int id, int package, decimal quantity, int customerId, DateTime soldDate, string remark);

        BatchFullDisplayModel GetFullDisplayBatchWithoutCompounds(int id);

        BatchReturnDisplayModel GetReturnDisplayBatch(int id);

        MethodResult ReturnBatch(int id, int package, decimal quantity, int customerId, DateTime returnDate, string remark);

        MethodResult MergeBatch(int id, int package, decimal quantity, int parentBatchId, DateTime date, string remark);

        MethodResult ThrowBatch(int id, string remark);

        List<BatchExtendedDisplayModel> GetExtendedDisplayBatchList(string query, int? batchId, int? typeId, int? lineId, int? statusId);

        List<BatchActionDisplayModel> GetDataByHistoryList(string query, int? batchId, int? customerId, DateTime? dateFrom, DateTime? dateTo, int actionHistoryType);

        DataTable GetBatchWarehouseExcel();

        DataTable DownloadSellingsExcel();

        BatchEditModel GetBatchForEditHistory(int id, int action, int historyId);

        MethodResult EditSellAction(int batchId, int historyId, decimal quantity, int package, int customerId);

        MethodResult EditReturnFromCustomerAction(int batchId, int historyId, decimal quantity, int package, int customerId);

        MethodResult EditMergeToOtherAction(int batchId, int historyId, decimal quantity, int package, int parentBatchId);

        MethodResult EditCompleteAction(int batchId, int historyId, decimal quantity, int package);

        MethodResult DeleteSellAction(int batchId, int historyId);

        MethodResult DeleteReturnFromCustomerAction(int batchId, int historyId);

        MethodResult DeleteCompleteAction(int batchId, int historyId);
    }
}