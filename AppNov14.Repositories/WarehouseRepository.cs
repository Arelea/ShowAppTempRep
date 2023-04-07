using AppNov14.Helpers;
using AppNov14.Models.Warehouse;
using AppNov14.Repositories.Extensions;
using AppNov14.Repositories.Interfaces;
using AppNov14.SqlDataAccess;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using AppNov14.Models.Base;

namespace AppNov14.Repositories
{
    public class WarehouseRepository : BaseDataRepository, IWarehouseRepository
    {
        public WarehouseRepository(IConfiguration configuration)
            : base(configuration)
        {
        }

        public List<WarehouseModel> GetWarehouseList(int? mode)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var warehouseList = context.Warehouse.AsQueryable();
                if (mode != null && mode != ActionModes.Common)
                {
                    warehouseList = warehouseList.Where(m => m.Mode == mode);
                }

                var result = warehouseList.Select(m => new WarehouseModel()
                {
                    Id = m.Id,
                    Type = m.Type,
                    SubType = m.SubType,
                    Provider = m.Provider,
                    Manufacturer = m.Manufacturer,
                    Leftovers = m.Leftovers,
                    Mode = m.Mode,
                })
                    .OrderBy(m => m.Type)
                    .ThenBy(m => m.SubType)
                    .ThenBy(m => m.Provider)
                    .ThenBy(m => m.Manufacturer)
                    .ToList();

                return result;
            }
        }

        public bool AddWarehouse(WarehouseModel model)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var warehouse = context.Warehouse.FirstOrDefault(m =>
                    (m.Type == model.Type) && (m.SubType == model.SubType) && (m.Provider == model.Provider) &&
                    (m.Manufacturer == model.Manufacturer));
                if (warehouse != null)
                {
                    return false;
                }

                var newWarehouseItem = new Warehouse()
                {
                    Type = model.Type,
                    SubType = model.SubType,
                    Provider = model.Provider,
                    Manufacturer = model.Manufacturer,
                    Leftovers = 0,
                    Mode = model.Mode,
                };

                context.Warehouse.InsertOnSubmit(newWarehouseItem);
                context.TrySubmitChanges();
                transactionScope.Complete();

                return true;
            }
        }

        public int? GetId(string type, string subType, string provider, string manufacturer, int mode)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                return context.Warehouse
                    .FirstOrDefault(m =>
                        (m.Type == type) && (m.SubType == subType) && (m.Provider == provider) &&
                        (m.Manufacturer == manufacturer) && (m.Mode == mode))?.Id;
            }
        }

        public int? GetIdById(int id, int mode)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                if (mode == ActionModes.Manufacturing)
                {
                    return context.ManufacturingRecords.FirstOrDefault(m => m.Id == id)?.WarehouseId;
                }

                return context.LaboratoryRecords.FirstOrDefault(m => m.Id == id)?.WarehouseId;
            }
        }

        public int? GetIndexId(string index, int warehouseId)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                return context.ManufacturingIndexes
                    .FirstOrDefault(m => m.Index == index && m.WarehouseId == warehouseId)?.Id;
            }
        }

        public bool IsIndexExist(int indexId, int warehouseId)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                return context.ManufacturingIndexes
                    .Any(m => m.Id == indexId && m.WarehouseId == warehouseId);
            }
        }

        public int? GetLabIndexId(string index, int warehouseId)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                return context.LaboratoryIndexes
                    .FirstOrDefault(m => m.Indexation == index && m.WarehouseId == warehouseId)?.Id;
            }
        }

        public List<IndexDisplayDataModel> GetIndexationByWarehouseAndName(string type, string subType, string provider,
            string manufacturer, string indexName, bool byWarehouse)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.ManufacturingIndexes.AsQueryable();
                if (byWarehouse)
                {
                    if (!string.IsNullOrEmpty(type))
                    {
                        query = query.Where(m => m.Warehouse.Type == type);
                    }

                    if (!string.IsNullOrEmpty(subType))
                    {
                        query = query.Where(m => m.Warehouse.SubType == subType);
                    }

                    if (!string.IsNullOrEmpty(provider))
                    {
                        query = query.Where(m => m.Warehouse.Provider == provider);
                    }

                    if (!string.IsNullOrEmpty(manufacturer))
                    {
                        query = query.Where(m => m.Warehouse.Manufacturer == manufacturer);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(indexName))
                    {
                        query = query.Where(m => m.Index.Contains(indexName));
                    }
                }

                return query
                    .Select(m => new IndexDisplayDataModel()
                    {
                        Id = m.Id,
                        Index = m.Index,
                        Leftovers = m.Leftovers,
                        ExpirationDate = m.ExpirationDate,
                        AutoDate = m.AutoDate,
                    })
                    .OrderByDescending(m => m.Id)
                    .ToList();
            }
        }

        public List<IndexDataModel> GetLabIndexationByWarehouse(int warehouseId)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                return context.LaboratoryIndexes
                    .Where(m => m.WarehouseId == warehouseId)
                    .Select(m => new IndexDataModel()
                    {
                        Id = m.Id,
                        Index = m.Indexation,
                        Leftovers = m.Leftovers,
                    })
                    .ToList();
            }
        }

        public bool IsLabRecord(int warehouseId)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                return context.Warehouse.FirstOrDefault(m => m.Id == warehouseId)?.Mode == ActionModes.Laboratory;
            }
        }

        public IndexDisplayDataModel GetDisplayIndex(int indexId)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                return context.ManufacturingIndexes
                    .Where(m => m.Id == indexId)
                    .Select(m => new IndexDisplayDataModel()
                    {
                        Id = m.Id,
                        Type = m.Warehouse.Type,
                        SubType = m.Warehouse.SubType,
                        Provider = m.Warehouse.Provider,
                        Manufacturer = m.Warehouse.Manufacturer,
                        Leftovers = m.Leftovers,
                        Index = m.Index,
                        ExpirationDate = m.ExpirationDate,
                        ManufacturingDate = m.ManufacturingDate,
                        AutoDate = m.AutoDate,
                        WarehouseId = m.WarehouseId,
                    })
                    .FirstOrDefault();
            }
        }

        public List<IndexDisplayDataModel> GetFullWarehouseList(string type, string subType, string provider,
            string manufacturer,
            string indexName, int? id, DateTime? dateStart, DateTime? dateFinish, bool showEmpty, int? expiredMode)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.ManufacturingIndexes.AsQueryable();
                if (!string.IsNullOrEmpty(type))
                {
                    query = query.Where(m => m.Warehouse.Type == type);
                }
                if (!string.IsNullOrEmpty(subType))
                {
                    query = query.Where(m => m.Warehouse.SubType == subType);
                }
                if (!string.IsNullOrEmpty(provider))
                {
                    query = query.Where(m => m.Warehouse.Provider == provider);
                }
                if (!string.IsNullOrEmpty(manufacturer))
                {
                    query = query.Where(m => m.Warehouse.Manufacturer == manufacturer);
                }
                if (!string.IsNullOrEmpty(indexName))
                {
                    query = query.Where(m => m.Index.Contains(indexName));
                }
                if (id.HasValue)
                {
                    query = query.Where(m => m.WarehouseId == id.Value);
                }
                if (dateStart.HasValue)
                {
                    query = query.Where(m => m.AutoDate >= dateStart.Value);
                }
                if (dateFinish.HasValue)
                {
                    query = query.Where(m => m.AutoDate <= dateFinish.Value);
                }
                if (!showEmpty)
                {
                    query = query.Where(m => m.Leftovers != 0);
                }
                if (expiredMode.HasValue)
                {
                    if (expiredMode.Value == 1)
                    {
                        query = query.Where(m => m.ExpirationDate.HasValue && m.ExpirationDate.Value >= DateTime.Now);
                    }
                    else if (expiredMode.Value == 2)
                    {
                        query = query.Where(m => !m.ExpirationDate.HasValue || m.ExpirationDate.Value < DateTime.Now);
                    }
                }

                return query
                    .Select(m => new IndexDisplayDataModel()
                    {
                        WarehouseId = m.WarehouseId,
                        Id = m.Id,
                        Index = m.Index,
                        Leftovers = m.Leftovers,
                        ExpirationDate = m.ExpirationDate,
                        AutoDate = m.AutoDate,
                        ManufacturingDate = m.ManufacturingDate,
                        Type = m.Warehouse.Type,
                        SubType = m.Warehouse.SubType,
                        Provider = m.Warehouse.Provider,
                        Manufacturer = m.Warehouse.Manufacturer,
                    })
                    .OrderBy(m => m.Type)
                    .ThenBy(m => m.SubType)
                    .ThenBy(m => m.Provider)
                    .ThenBy(m => m.Manufacturer)
                    .ThenByDescending(m => m.Leftovers)
                    .ToList();
            }
        }

        public MethodResult RestoreIndexDates(int id, DateTime? expirationDate, DateTime? manufacturingDate)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var indexRecord = context.ManufacturingIndexes.FirstOrDefault(m => m.Id == id);
                if (indexRecord == null)
                {
                    return new MethodResult(false, ErrorMessages.IndexIdNotFound);
                }

                if (!indexRecord.ExpirationDate.HasValue && expirationDate.HasValue)
                {
                    indexRecord.ExpirationDate = expirationDate.Value;
                }
                if (!indexRecord.ManufacturingDate.HasValue && manufacturingDate.HasValue)
                {
                    indexRecord.ManufacturingDate = manufacturingDate.Value;
                }

                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }

        public DataTable GetDataTableForExcel()
        {
            using (var data = this.ExecuteProcedure("Excel_GetManufacturingWarehouseData", true))
            {
                if (data != null && (data.Rows.Count > 0))
                {
                    return data;
                }
            }

            return null;
        }
    }
}
