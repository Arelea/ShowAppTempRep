using AppNov14.Models.Base;
using AppNov14.Models.Manufacturing;
using System;
using System.Collections.Generic;
using System.Data;

namespace AppNov14.Repositories.Interfaces
{
    public interface IManufacturingRepository
    {
        MethodResult ReplenishWarehouse(ManufacturingRecordReplenishModel model);

        MethodResult WriteOffWarehouse(ManufacturingRecordWriteOffModel model);

        List<ManufacturingRecordDisplayModel> GetEmployeeLastAdding(string employee);

        List<ManufacturingRecordDisplayModel> GetEveryoneLastAdding();

        List<ManufacturingRecordDisplayModel> GetItemsByStartAndEndDate(DateTime dateStart, DateTime dateEnd);

        ManufacturingRecordDisplayModel GetById(int id);

        MethodResult EditWriteOff(ManufacturingRecordWriteOffModel newRecord, int recordId);

        MethodResult EditReplenish(ManufacturingRecordReplenishModel newRecord, int recordId);

        MethodResult Delete(int id);

        int GetRecordCountByDate(DateTime dateStart, DateTime dateEnd);

        DataTable GetDataTableForExcel(DateTime dateStart, DateTime dateEnd);

        List<ManufacturingRecordDisplayModel> Search(ManufacturingSearchModel model);

        ManufacturingRecordModel GetItemForCopy(int id);
    }
}