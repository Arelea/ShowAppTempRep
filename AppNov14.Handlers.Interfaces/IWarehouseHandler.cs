using System;
using System.Data;
using AppNov14.Handlers.Result.Warehouse;
using AppNov14.Models.Warehouse;

namespace AppNov14.Handlers.Interfaces
{
    public interface IWarehouseHandler : IBaseDataHandler
    {
        GetWarehouseListResult GetWarehouseList(int? mode);

        bool AddWarehouse(WarehouseModel model);

        GetFullWarehouseListResult GetFullWarehouseList(string type, string subType, string provider, string manufacturer, string indexName, int? id, DateTime? dateStart, DateTime? dateFinish, bool showEmpty, int? expiredMode);

        DataTable GetDataTableForExcel();
    }
}