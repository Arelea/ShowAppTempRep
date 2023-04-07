using AppNov14.Handlers.Result.Manufacturing;
using AppNov14.Models.Base;
using AppNov14.Models.Batch;
using AppNov14.Models.Manufacturing;
using System;
using System.Collections.Generic;
using System.Data;

namespace AppNov14.Handlers.Interfaces
{
    public interface IManufacturingHandler : IBaseDataHandler
    {
        IncomingOrderResult GetIncomingOrder();

        MethodResult ReplenishWarehouse(ManufacturingRecordReplenishModel model);

        OutcomingOrderResult GetOutcomingOrder(int? id);

        MethodResult WriteOffWarehouse(ManufacturingRecordWriteOffModel model);

        List<ManufacturingRecordDisplayModel> GetEmployeeLastAdding(string employee);

        List<ManufacturingRecordDisplayModel> GetEveryoneLastAdding();

        List<ManufacturingRecordDisplayModel> GetItemsByStartAndEndDate(DateTime dateStart, DateTime dateEnd);

        ManufacturingRecordDisplayModel GetById(int id);

        EditResult GetForEdit(int id);

        MethodResult EditReplenish(ManufacturingRecordReplenishModel model, int recordId);

        MethodResult EditWriteOff(ManufacturingRecordWriteOffModel model, int recordId);

        MethodResult Delete(int id);

        int GetRecordCountByDate(DateTime dateStart, DateTime dateEnd);

        DataTable GetDataTableForExcel(DateTime dateStart, DateTime dateEnd);

        BatchInfoResult GetBatchInfo(int? batchId);

        MethodResult AddBatchType(string name);

        ViewIndexResult GetViewIndex(string type, string subType, string provider, string manufacturer, string indexName, bool byWarehouse);

        SearchResult Search(ManufacturingSearchModel model);

        EditBatchListResult GetBatchesForEdit(int statusId, int? batchId);

        EditBatchResult GetBatchForEdit(int id);

        MethodResult EditBatch(BatchModel model);

        IndexInfoResult GetIndexInfo(int? indexId);

        MethodResult RestoreIndexDates(int id, DateTime? expirationDate, DateTime? manufacturingDate);
    }
}