using AppNov14.Models.Warehouse;
using System.Collections.Generic;

namespace AppNov14.Repositories.Interfaces
{
    public interface IBaseDataRepository
    {
        List<string> GetBatchTypeList();

        Dictionary<int, string> GetDistinctFilledIndexationList(string type, string subType, string provider, string manufacturer);

        List<string> GetDistinctTypeList(WarehouseFilterModel filter);

        List<string> GetDistinctSubTypeList(WarehouseFilterModel filter);

        List<string> GetDistinctProviderList(WarehouseFilterModel filter);

        List<string> GetDistinctManufacturerList(WarehouseFilterModel filter);

        List<string> GetDistinctConsignmentNumberList(int? action);

        List<string> GetDistinctLabConsignmentNumberList();

        Dictionary<string, string> GetDistinctFilledIndexationLabList(string type, string subType, string provider, string manufacturer);

        Dictionary<int, string> GetBatchLines();

        Dictionary<int, string> GetBatchTypes();

        Dictionary<int, string> GetBatches(int? statusId = null);

        Dictionary<int, string> GetReplenishmentDocuments();

        Dictionary<int, string> GetBatchStatuses(List<int> statusesForDisplay = null);

        Dictionary<int, string>  GetCustomers();

        Dictionary<int, string> GetFilteredBatches(int? typeId, int? lineId, int? statusId);

        Dictionary<int, string> GetBatchesByCustomer(int? customerId, int actionHistoryType);

        Dictionary<int, string> GetTypedCustomers(int actionHistoryType);
    }
}