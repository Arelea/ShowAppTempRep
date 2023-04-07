using System;
using System.Collections.Generic;
using System.Data;
using AppNov14.Handlers.Interfaces;
using AppNov14.Handlers.Result.Warehouse;
using AppNov14.Models.Warehouse;
using AppNov14.Repositories.Interfaces;
using System.Linq;
using AppNov14.Helpers;

namespace AppNov14.Handlers.Warehouse
{
    public sealed class WarehouseHandler : BaseDataHandler, IWarehouseHandler
    {
        private readonly IWarehouseRepository WarehouseRepository;

        public WarehouseHandler(IWarehouseRepository warehouseRepository, IBaseDataRepository baseDataRepository) : base (baseDataRepository)
        {
            WarehouseRepository = warehouseRepository;
        }

        public GetWarehouseListResult GetWarehouseList(int? mode)
        {
            var items = this.WarehouseRepository.GetWarehouseList(mode);
            var result = new GetWarehouseListResult()
            {
                Items = items,
                DistinctTypes = items.Select(m => m.Type).Distinct().ToList(),
            };

            return result;
        }

        public bool AddWarehouse(WarehouseModel model)
        {
            return this.WarehouseRepository.AddWarehouse(model);
        }

        public GetFullWarehouseListResult GetFullWarehouseList(string type, string subType, string provider,
            string manufacturer, string indexName, int? id, DateTime? dateStart, DateTime? dateFinish, bool showEmpty, int? expiredMode)
        {
            var result = new GetFullWarehouseListResult();
            result.Items = new List<IndexDisplayDataModel>();
            result.TypeList = this.GetDistinctTypeList(ActionModes.Manufacturing);
            if (!string.IsNullOrEmpty(type))
            {
                result.SubTypeList = this.GetDistinctSubTypeList(ActionModes.Manufacturing, type);
            }
            if (!string.IsNullOrEmpty(subType))
            {
                result.ProviderList = this.GetDistinctProviderList(ActionModes.Manufacturing, type, subType);
            }
            if (!string.IsNullOrEmpty(provider))
            {
                result.ManufacturerList = this.GetDistinctManufacturerList(ActionModes.Manufacturing, type, subType, provider);
            }

            result.Items = this.WarehouseRepository.GetFullWarehouseList(type, subType, provider, manufacturer, indexName, id, dateStart, dateFinish, showEmpty, expiredMode);

            return result;
        }

        public DataTable GetDataTableForExcel()
        {
            return this.WarehouseRepository.GetDataTableForExcel();
        }
    }
}
