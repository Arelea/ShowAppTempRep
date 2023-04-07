using System;
using System.Collections.Generic;

namespace AppNov14.Handlers.Interfaces
{
    public interface IBaseDataHandler
    {
        List<string> GetBatchTypeList();

        Dictionary<int, string> GetDistinctFilledIndexationList(string type, string subType, string provider, string manufacturer);

        Dictionary<string, string> GetDistinctFilledIndexationLabList(string type, string subType, string provider, string manufacturer);

        List<string> GetDistinctTypeList(int? mode);

        List<string> GetDistinctSubTypeList(int? mode, string type);

        List<string> GetDistinctSubTypeList(int? mode);

        List<string> GetDistinctProviderList(int? mode, bool isFilled = false);

        List<string> GetDistinctProviderList(int? mode, string type, string subType, bool isFilled = false);

        List<string> GetDistinctManufacturerList(int? mode, bool isFilled = false);

        List<string> GetDistinctManufacturerList(int? mode, string type, string subType, string provider, bool isFilled = false);

        List<string> GetDistinctConsignmentNumberList(int? action);

        List<string> GetDistinctLabConsignmentNumberList();

        Dictionary<int, string> GetBatchLines();

        Dictionary<int, string> GetBatchTypes();

        Dictionary<int, string> GetBatches(int? statusId = null);
    }
}