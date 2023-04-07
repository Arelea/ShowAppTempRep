using AppNov14.Models.Base;
using AppNov14.Models.Warehouse;
using System;
using System.Collections.Generic;
using System.Data;

namespace AppNov14.Repositories.Interfaces
{
    public interface IWarehouseRepository
    {
        List<WarehouseModel> GetWarehouseList(int? mode);

        bool AddWarehouse(WarehouseModel model);

        int? GetId(string type, string subType, string provider, string manufacturer, int mode);

        int? GetIndexId(string index, int warehouseId);

        bool IsIndexExist(int indexId, int warehouseId);

        int? GetIdById(int id, int mode);

        List<IndexDisplayDataModel> GetIndexationByWarehouseAndName(string type, string subType, string provider, string manufacturer, string indexName, bool byWarehouse);

        bool IsLabRecord(int warehouseId);

        int? GetLabIndexId(string index, int warehouseId);

        List<IndexDataModel> GetLabIndexationByWarehouse(int warehouseId);

        IndexDisplayDataModel GetDisplayIndex(int indexId);

        List<IndexDisplayDataModel> GetFullWarehouseList(string type, string subType, string provider, string manufacturer,
            string indexName, int? id, DateTime? dateStart, DateTime? dateFinish, bool showEmpty, int? expiredMode);

        MethodResult RestoreIndexDates(int id, DateTime? expirationDate, DateTime? manufacturingDate);

        DataTable GetDataTableForExcel();
    }
}