using AppNov14.Models.Base;
using AppNov14.Models.ManufacturingTable;
using System;
using System.Collections.Generic;
using System.Data;

namespace AppNov14.Repositories.Interfaces
{
    public interface ILaboratoryRepository
    {
        MethodResult ReplenishWarehouse(ManufacturngTableFullModel model);

        MethodResult WriteOffWarehouse(ManufacturngTableFullModel model);

        List<ManufacturingTableWriteModel> GetLastAdding();

        List<ManufacturingTableWriteModel> GetItemsByStartAndEndDate(DateTime dateStart, DateTime dateEnd);

        ManufacturingTableWriteModel GetById(int id);

        MethodResult EditWriteOff(ManufacturngTableFullModel newRecord);

        MethodResult EditReplenish(ManufacturngTableFullModel newRecord);

        MethodResult Delete(int id);

        int GetRecordCountByDate(DateTime dateStart, DateTime dateEnd);

        DataTable GetDataTableForExcel(DateTime dateStart, DateTime dateEnd);

        List<ManufacturingTableWriteModel> GetItemsByConsignmentNumber(string number);
    }
}