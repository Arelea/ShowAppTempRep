using AppNov14.Helpers;
using AppNov14.Models.Manufacturing;
using AppNov14.Models.ManufacturingTable;
using AppNov14.Models.Warehouse;
using AppNov14.SqlDataAccess;
using Castle.Core.Internal;
using System.Collections.Generic;
using System.Linq;

namespace AppNov14.Repositories.Extensions
{
    public static class RepositoryExtensions
    {
        public static List<ManufacturingRecordDisplayModel> JoinManufacturingTableList(IQueryable<ManufacturingRecord> query, BaseDataContext context)
        {
            var result = query
                .Join(context.Warehouse, mt => mt.WarehouseId, w => w.Id, (mt, w) => new ManufacturingRecordDisplayModel
                {
                    Id = mt.Id,
                    Type = w.Type,
                    SubType = w.SubType,
                    Provider = w.Provider,
                    Manufacturer = w.Manufacturer,
                    Quantity = mt.Quantity,
                    Leftovers = mt.Leftovers,                 
                    DocDate = mt.DocDate,
                    Employee = mt.Employee,
                    IpAddress = mt.IpAddress,
                    InsertDate = mt.InsertDate,
                    Remarks = mt.Remarks,
                    ActionType = mt.ActionType,
                    BatchName = mt.Batch.Name,
                    BatchLine = mt.Batch.BatchLine.Name,
                    BatchType = mt.Batch.BatchType.Name,
                    ReplenishmentDocument = mt.ReplenishmentDocument.Name,
                    Index = mt.ManufacturingIndex.Index,
                    IndexId = mt.IndexId,
                    BatchId = mt.BatchId,
                    ExpirationDate = mt.ManufacturingIndex.ExpirationDate,
                    ManufacturingDate = mt.ManufacturingIndex.ManufacturingDate,
                })                         
                .OrderByDescending(r => r.Id)
                .ToList();

            return result;
        }

        public static ManufacturingRecordDisplayModel JoinManufacturingTable(IQueryable<ManufacturingRecord> query, BaseDataContext context)
        {
            var result = query
                .Join(context.Warehouse, mt => mt.WarehouseId, w => w.Id, (mt, w) => new ManufacturingRecordDisplayModel
                {
                    Id = mt.Id,
                    Type = w.Type,
                    SubType = w.SubType,
                    Provider = w.Provider,
                    Manufacturer = w.Manufacturer,
                    Quantity = mt.Quantity,
                    Leftovers = mt.Leftovers,
                    DocDate = mt.DocDate,
                    Employee = mt.Employee,
                    IpAddress = mt.IpAddress,
                    InsertDate = mt.InsertDate,
                    Remarks = mt.Remarks,
                    ActionType = mt.ActionType,
                    BatchName = mt.Batch.Name,
                    BatchLine = mt.Batch.BatchLine.Name,
                    BatchType = mt.Batch.BatchType.Name,
                    ReplenishmentDocument = mt.ReplenishmentDocument.Name,
                    Index = mt.ManufacturingIndex.Index,
                    IndexId = mt.IndexId,
                    BatchId = mt.BatchId,
                    ExpirationDate = mt.ManufacturingIndex.ExpirationDate,
                    ManufacturingDate = mt.ManufacturingIndex.ManufacturingDate,
                }).FirstOrDefault();

            return result;
        }

        public static decimal CalcReplenishLeftovers(decimal currentLeftovers, decimal currentQuantity, decimal newQuantity)
        {
            return currentLeftovers - currentQuantity + newQuantity;
        }

        public static decimal CalcWriteOffLeftovers(decimal currentLeftovers, decimal currentQuantity, decimal newQuantity)
        {
            return currentLeftovers + currentQuantity - newQuantity;
        }

        public static IQueryable<Warehouse> GetFilteredWarehouse(IQueryable<Warehouse> query, WarehouseFilterModel filter)
        {
            if (!filter.Type.IsNullOrEmpty())
            {
                query = query.Where(m => m.Type == filter.Type);
            }
            if (!filter.SubType.IsNullOrEmpty())
            {
                query = query.Where(m => m.SubType == filter.SubType);
            }
            if (!filter.Provider.IsNullOrEmpty())
            {
                query = query.Where(m => m.Provider == filter.Provider);
            }
            if (!filter.Manufacturer.IsNullOrEmpty())
            {
                query = query.Where(m => m.Manufacturer == filter.Manufacturer);
            }
            if (filter.Mode != default(int) && filter.Mode != ActionModes.Common)
            {
                query = query.Where(m => m.Mode == filter.Mode);
            }
            if (filter.Id != default(int))
            {
                query = query.Where(m => m.Id == filter.Id);
            }
            if (filter.IsFilled)
            {
                query = query.Where(m => m.Leftovers > 0);
            }

            return query;
        }

        public static List<ManufacturingTableWriteModel> JoinLaboratoryTableList(IQueryable<LaboratoryRecord> query, BaseDataContext context)
        {
            var result = query
                .Join(context.Warehouse, mt => mt.WarehouseId, w => w.Id, (mt, w) => new
                {
                    Id = mt.Id,
                    Type = w.Type,
                    SubType = w.SubType,
                    Provider = w.Provider,
                    Manufacturer = w.Manufacturer,
                    Quantity = mt.Quantity,
                    Leftovers = mt.Leftovers,
                    Document = mt.Document,
                    DocumentNumber = mt.DocumentNumber,
                    DocDate = mt.DocDate,
                    Employee = mt.Employee,
                    IpAddress = mt.IpAddress,
                    AutoDate = mt.AutoDate,
                    Remarks = mt.Remarks,
                    OperationType = mt.Action,
                    IndexId = mt.IndexId,
                })
                .Join(context.LaboratoryIndexes, mtw => mtw.IndexId, i => i.Id, (mtw, i) => new ManufacturingTableWriteModel()
                {
                    Id = mtw.Id,
                    Type = mtw.Type,
                    SubType = mtw.SubType,
                    Provider = mtw.Provider,
                    Manufacturer = mtw.Manufacturer,
                    Quantity = mtw.Quantity,
                    Leftovers = mtw.Leftovers,
                    Document = mtw.Document,
                    DocumentNumber = mtw.DocumentNumber,
                    DocDate = mtw.DocDate,
                    Employee = mtw.Employee,
                    IpAddress = mtw.IpAddress,
                    AutoDate = mtw.AutoDate,
                    Remarks = mtw.Remarks,
                    OperationType = mtw.OperationType,
                    Indexation = i.Indexation,
                })
                .OrderByDescending(r => r.Id)
                .ToList();

            return result;
        }

        public static ManufacturingTableWriteModel JoinLaboratoryTable(IQueryable<LaboratoryRecord> query, BaseDataContext context)
        {
            var result = query
                .Join(context.Warehouse, mt => mt.WarehouseId, w => w.Id, (mt, w) => new
                {
                    Id = mt.Id,
                    Type = w.Type,
                    SubType = w.SubType,
                    Provider = w.Provider,
                    Manufacturer = w.Manufacturer,
                    Quantity = mt.Quantity,
                    Leftovers = mt.Leftovers,
                    Document = mt.Document,
                    DocumentNumber = mt.DocumentNumber,
                    DocDate = mt.DocDate,
                    Employee = mt.Employee,
                    IpAddress = mt.IpAddress,
                    AutoDate = mt.AutoDate,
                    Remarks = mt.Remarks,
                    OperationType = mt.Action,
                    IndexId = mt.IndexId,
                })
                .Join(context.LaboratoryIndexes, mtw => mtw.IndexId, i => i.Id, (mtw, i) => new ManufacturingTableWriteModel()
                {
                    Id = mtw.Id,
                    Type = mtw.Type,
                    SubType = mtw.SubType,
                    Provider = mtw.Provider,
                    Manufacturer = mtw.Manufacturer,
                    Quantity = mtw.Quantity,
                    Leftovers = mtw.Leftovers,
                    Document = mtw.Document,
                    DocumentNumber = mtw.DocumentNumber,
                    DocDate = mtw.DocDate,
                    Employee = mtw.Employee,
                    IpAddress = mtw.IpAddress,
                    AutoDate = mtw.AutoDate,
                    Remarks = mtw.Remarks,
                    OperationType = mtw.OperationType,
                    Indexation = i.Indexation,
                }).FirstOrDefault();

            return result;
        }

        public static List<ManufacturingRecordShortDisplayModel> JoinShortManufacturingTableList(IQueryable<ManufacturingRecord> query, BaseDataContext context)
        {
            var result = query
                .Join(context.Warehouse, mt => mt.WarehouseId, w => w.Id, (mt, w) => new ManufacturingRecordShortDisplayModel
                {
                    Id = mt.Id,
                    Type = w.Type,
                    SubType = w.SubType,
                    Provider = w.Provider,
                    Manufacturer = w.Manufacturer,
                    Quantity = mt.Quantity,
                    DocDate = mt.DocDate,
                    Index = mt.ManufacturingIndex.Index,
                    IndexId = mt.IndexId,
                })
                .OrderByDescending(r => r.Id)
                .ToList();

            return result;
        }
    }
}