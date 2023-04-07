using AppNov14.Models.Base;
using AppNov14.Models.ManufacturingTable;
using AppNov14.Models.Warehouse;
using System;
using System.Collections.Generic;
using System.Data;

namespace AppNov14.Handlers.Interfaces
{
    public interface ILaboratoryHandler : IBaseDataHandler
    {
        MethodResult ReplenishWarehouse(ManufacturingTableWriteModel model);

        MethodResult WriteOffWarehouse(ManufacturingTableWriteModel model);

        List<ManufacturingTableWriteModel> GetLastAdding();

        List<ManufacturingTableWriteModel> GetItemsByStartAndEndDate(DateTime dateStart, DateTime dateEnd);

        ManufacturingTableWriteModel GetById(int id);

        MethodResult Edit(ManufacturingTableWriteModel model);

        MethodResult Delete(int id);

        int GetRecordCountByDate(DateTime dateStart, DateTime dateEnd);

        DataTable GetDataTableForExcel(DateTime dateStart, DateTime dateEnd);

        List<ManufacturingTableWriteModel> GetItemsByConsignmentNumber(string number);

        List<IndexDataModel> GetIndexationByWarehouse(int mode, string type, string subType, string provider, string manufacturer);
    }
}
