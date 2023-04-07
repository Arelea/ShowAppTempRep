using AppNov14.Helpers;
using AppNov14.Models.Warehouse;
using AppNov14.Repositories.Extensions;
using AppNov14.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;

namespace AppNov14.Repositories
{
    public class BaseDataRepository : BaseRepository, IBaseDataRepository
    {
        public BaseDataRepository(IConfiguration configuration)
            : base(configuration)
        {
        }

        public List<string> GetBatchTypeList()
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                return context.BatchTypes.Select(m => m.Name).ToList();
            }
        }

        public Dictionary<int, string> GetDistinctFilledIndexationList(string type, string subType, string provider,
            string manufacturer)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var warehouseId = context.Warehouse.FirstOrDefault(m =>
                    m.Type == type && m.SubType == subType && m.Provider == provider &&
                    m.Manufacturer == manufacturer && m.Leftovers > 0 && m.Mode == ActionModes.Manufacturing)?.Id;

                if (warehouseId == null)
                {
                    return new Dictionary<int, string>();
                }

                var result = context.ManufacturingIndexes.Where(m => m.Leftovers > 0 && m.WarehouseId == warehouseId)
                    .ToDictionary(m => m.Id, m => m.Index + " (" + m.Leftovers.ToString() + " кг.)");

                return result;
            }
        }

        public Dictionary<string, string> GetDistinctFilledIndexationLabList(string type, string subType,
            string provider, string manufacturer)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var warehouseId = context.Warehouse.FirstOrDefault(m =>
                    m.Type == type && m.SubType == subType && m.Provider == provider &&
                    m.Manufacturer == manufacturer && m.Leftovers > 0 && m.Mode == ActionModes.Laboratory)?.Id;

                if (warehouseId == null)
                {
                    return new Dictionary<string, string>();
                }

                var result = context.LaboratoryIndexes.Where(m => m.Leftovers > 0 && m.WarehouseId == warehouseId)
                    .ToDictionary(m => m.Indexation, m => m.Leftovers.ToString());

                return result;
            }
        }

        public List<string> GetDistinctTypeList(WarehouseFilterModel filter)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var warehouseList = context.Warehouse.AsQueryable();

                var result = RepositoryExtensions.GetFilteredWarehouse(warehouseList, filter).GroupBy(m => m.Type)
                    .Select(m => m.FirstOrDefault()).Select(m => m.Type).ToList();

                return result;
            }
        }

        public List<string> GetDistinctSubTypeList(WarehouseFilterModel filter)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var warehouseList = context.Warehouse.AsQueryable();

                var result = RepositoryExtensions.GetFilteredWarehouse(warehouseList, filter).GroupBy(m => m.SubType)
                    .Select(m => m.FirstOrDefault()).Select(m => m.SubType).ToList();

                return result;
            }
        }

        public List<string> GetDistinctProviderList(WarehouseFilterModel filter)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var warehouseList = context.Warehouse.AsQueryable();

                var result = RepositoryExtensions.GetFilteredWarehouse(warehouseList, filter).GroupBy(m => m.Provider)
                    .Select(m => m.FirstOrDefault()).Select(m => m.Provider).ToList();

                return result;
            }
        }

        public List<string> GetDistinctManufacturerList(WarehouseFilterModel filter)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var warehouseList = context.Warehouse.AsQueryable();

                var result = RepositoryExtensions.GetFilteredWarehouse(warehouseList, filter)
                    .GroupBy(m => m.Manufacturer).Select(m => m.FirstOrDefault()).Select(m => m.Manufacturer).ToList();

                return result;
            }
        }

        public List<string> GetDistinctConsignmentNumberList(int? action)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.Batches.Select(m => m.Name).ToList();

                return query;
            }
        }

        public List<string> GetDistinctLabConsignmentNumberList()
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var result = context.ConsLabNumbers.GroupBy(m => m.Name).Select(m => m.FirstOrDefault())
                    .Select(m => m.Name).ToList();

                return result;
            }
        }

        public Dictionary<int, string> GetBatchLines()
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                return context.BatchLines.OrderBy(m => m.Name).ToDictionary(m => m.Id, m => m.DisplayName);
            }
        }

        public Dictionary<int, string> GetBatchTypes()
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                return context.BatchTypes.OrderBy(m => m.Name).ToDictionary(m => m.Id, m => m.Name);
            }
        }

        public Dictionary<int, string> GetBatches(int? statusId = null)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.Batches.OrderByDescending(m => m.InsertDate).AsQueryable();
                if (statusId.HasValue)
                {
                    query = query.Where(m => m.StatusId == statusId.Value);
                }

                return query.ToDictionary(m => m.Id, m => m.Name);
            }
        }

        public Dictionary<int, string> GetReplenishmentDocuments()
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                return context.ReplenishmentDocuments.ToDictionary(m => m.Id, m => m.Name);
            }
        }

        public Dictionary<int, string> GetBatchStatuses(List<int> statusesForDisplay = null)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.BatchStatuses.AsQueryable();
                if (!statusesForDisplay.IsNullOrEmpty())
                {
                    query = query.Where(m => statusesForDisplay.Contains(m.Id));
                }

                return query.ToDictionary(m => m.Id, m => m.Name);
            }
        }

        public Dictionary<int, string> GetCustomers()
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                return context.Customers.OrderBy(m => m.Name).ToDictionary(m => m.Id, m => m.Name);
            }
        }

        public Dictionary<int, string> GetFilteredBatches(int? typeId, int? lineId, int? statusId)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.Batches.OrderByDescending(m => m.InsertDate).AsQueryable();
                if (statusId.HasValue)
                {
                    query = query.Where(m => m.StatusId == statusId.Value);
                }

                if (lineId.HasValue)
                {
                    query = query.Where(m => m.LineId == lineId.Value);
                }

                if (typeId.HasValue)
                {
                    query = query.Where(m => m.TypeId == typeId.Value);
                }

                return query.ToDictionary(m => m.Id, m => m.Name);
            }
        }

        public Dictionary<int, string> GetBatchesByCustomer(int? customerId, int actionHistoryType)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.Batches.Where(m => m.BatchHistories.Any(n => n.ActionTypeId == actionHistoryType));
                if (customerId.HasValue)
                {
                    query = query.Where(m => m.BatchHistories.Any(n => n.CustomerId == customerId));
                }

                return query.OrderByDescending(m => m.LastUpdateDate).ToDictionary(m => m.Id, m => m.Name);
            }
        }

        public Dictionary<int, string> GetTypedCustomers(int actionHistoryType)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                return context.Customers
                    .Where(m => m.BatchHistories.Any(n => n.ActionTypeId == actionHistoryType))
                    .OrderBy(m => m.Name).ToDictionary(m => m.Id, m => m.Name);
            }
        }
    }
}