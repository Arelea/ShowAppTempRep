using AppNov14.Helpers;
using AppNov14.Models.Base;
using AppNov14.Models.Manufacturing;
using AppNov14.Repositories.Extensions;
using AppNov14.Repositories.Interfaces;
using AppNov14.SqlDataAccess;
using Castle.Core.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;

namespace AppNov14.Repositories
{
    public sealed class ManufacturingRepository : BaseDataRepository, IManufacturingRepository
    {
        public ManufacturingRepository(IConfiguration configuration)
            : base(configuration)
        {
        }

        public MethodResult ReplenishWarehouse(ManufacturingRecordReplenishModel model)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var warehouse = context.Warehouse.FirstOrDefault(m => m.Id == model.WarehouseId);
                warehouse.Leftovers += model.Quantity;

                var isIndexExist = context.ManufacturingIndexes.Any(m => m.Index == model.Index && m.WarehouseId == model.WarehouseId);
                if (isIndexExist)
                {
                    return new MethodResult(false, ErrorMessages.IndexIdNotFoundOrAlreadyExist);
                }

                var newIndex = new ManufacturingIndex()
                {
                    WarehouseId = model.WarehouseId,
                    Index = model.Index,
                    AutoDate = DateTime.Now,
                    Leftovers = model.Quantity,
                    ManufacturingDate = model.ManufacturingDate,
                    ExpirationDate = model.ExpirationDate,
                };
                var newRecord = new ManufacturingRecord()
                {
                    Quantity = model.Quantity,
                    InsertDate = model.InsertDate,
                    DocDate = model.DocDate,
                    WarehouseId = model.WarehouseId,
                    Leftovers = warehouse.Leftovers,
                    ManufacturingIndex = newIndex,
                    IpAddress = model.IpAddress,
                    Employee = model.Employee,
                    Remarks = model.Remarks,
                    ActionType = model.ActionType,
                };

                var replenishmentDocumentId = context.ReplenishmentDocuments.FirstOrDefault(m => m.Name == model.ReplenishmentDocument)?.Id;
                if (!replenishmentDocumentId.HasValue)
                {
                    var newReplenishmentDocument = new ReplenishmentDocument()
                    {
                        Name = model.ReplenishmentDocument,
                        InsertDate = model.InsertDate,
                    };

                    newRecord.ReplenishmentDocument = newReplenishmentDocument;
                }
                else
                {
                    newRecord.ReplenishmentDocumentId = replenishmentDocumentId;
                }

                context.ManufacturingRecords.InsertOnSubmit(newRecord);
                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }

        public MethodResult WriteOffWarehouse(ManufacturingRecordWriteOffModel model)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var warehouse = context.Warehouse.FirstOrDefault(m => m.Id == model.WarehouseId);
                warehouse.Leftovers -= model.Quantity;

                if (warehouse.Leftovers < 0)
                {
                    return new MethodResult(false, ErrorMessages.WarehouseBelowZero + (warehouse.Leftovers + model.Quantity));
                }

                var index = context.ManufacturingIndexes.FirstOrDefault(m => m.Id == model.IndexId);
                index.Leftovers -= model.Quantity;
                if (index.Leftovers < 0)
                {
                    return new MethodResult(false, ErrorMessages.IndexBelowZero + (index.Leftovers + model.Quantity));
                }

                var newRecord = new ManufacturingRecord()
                {
                    Quantity = model.Quantity,
                    InsertDate = model.InsertDate,
                    DocDate = model.DocDate,
                    WarehouseId = model.WarehouseId,
                    Leftovers = warehouse.Leftovers,
                    IndexId = model.IndexId,
                    IpAddress = model.IpAddress,
                    Employee = model.Employee,
                    Remarks = model.Remarks,
                    ActionType = model.ActionType,
                };

                if (model.IsNewBatch)
                {
                    var newBatchHistory = new EntitySet<BatchHistory>() 
                    { 
                        new BatchHistory()
                        {
                            Text = BatchHistoryTexts.GetTextByAction(BatchHistoryActions.CreateAction, model.Batch.Name),
                            OperationTypeId = BatchHistoryOperations.UsualOperation,
                            ActionTypeId = BatchHistoryActions.CreateAction,
                            InsertDate = model.Batch.InsertDate,
                        } 
                    };

                    var newBatch = new Batch()
                    {
                        Name = model.Batch.Name,
                        LineId = model.Batch.LineId,
                        TypeId = model.Batch.TypeId,
                        InsertDate = model.Batch.InsertDate,
                        StatusId = model.Batch.StatusId,
                        BatchHistories = newBatchHistory,
                        LastUpdateDate = model.Batch.InsertDate,
                        CreateDate = model.Batch.CreateDate,
                    };

                    newRecord.Batch = newBatch;
                }
                else
                {
                    newRecord.BatchId = model.BatchId;
                }

                context.ManufacturingRecords.InsertOnSubmit(newRecord);
                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true, newRecord.Id);
            }
        }

        public List<ManufacturingRecordDisplayModel> GetEmployeeLastAdding(string employee)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var updatedEmployee = Other.UpdatedBy + employee;
                var query = context.ManufacturingRecords
                    .Where(m => m.Employee == employee || m.Employee == updatedEmployee)
                    .OrderByDescending(m => m.Id)
                    .Take(Other.EmployeeLastAddingsCount);

                var result = RepositoryExtensions.JoinManufacturingTableList(query, context);

                return result;
            }
        }

        public List<ManufacturingRecordDisplayModel> GetEveryoneLastAdding()
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.ManufacturingRecords
                    .OrderByDescending(m => m.Id)
                    .Take(Other.EmployeeLastAddingsCount);

                var result = RepositoryExtensions.JoinManufacturingTableList(query, context);

                return result;
            }
        }

        public List<ManufacturingRecordDisplayModel> GetItemsByStartAndEndDate(DateTime dateStart, DateTime dateEnd)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.ManufacturingRecords
                    .Where(m => m.InsertDate <= dateEnd && m.InsertDate > dateStart)
                    .OrderByDescending(m => m.Id);

                var result = RepositoryExtensions.JoinManufacturingTableList(query, context);

                return result;
            }
        }

        public ManufacturingRecordDisplayModel GetById(int id)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.ManufacturingRecords
                    .Where(m => m.Id == id);

                var result = RepositoryExtensions.JoinManufacturingTable(query, context);

                return result;
            }
        }

        public MethodResult EditReplenish(ManufacturingRecordReplenishModel newRecord, int recordId)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var record = context.ManufacturingRecords.FirstOrDefault(m => m.Id == recordId);
                if (record.ManufacturingIndex == null)
                {
                    return new MethodResult(false, ErrorMessages.IndexWasDeleted);
                }

                if (record.ReplenishmentDocument.Name != newRecord.ReplenishmentDocument)
                {
                    var replenishmentDocument = context.ReplenishmentDocuments.FirstOrDefault(m => m.Name == newRecord.ReplenishmentDocument);
                    if (replenishmentDocument != null)
                    {
                        record.ReplenishmentDocumentId = replenishmentDocument.Id;
                    }
                    else
                    {
                        var newReplenishmentDocument = new ReplenishmentDocument()
                        {
                            Name = newRecord.ReplenishmentDocument,
                            InsertDate = DateTime.Now,
                        };

                        record.ReplenishmentDocument = newReplenishmentDocument;
                    }
                }
                if (record.DocDate != newRecord.DocDate)
                {
                    record.DocDate = newRecord.DocDate;
                }
                if (record.Remarks != newRecord.Remarks)
                {
                    record.Remarks = newRecord.Remarks;
                }
                if (record.ManufacturingIndex.ExpirationDate != newRecord.ExpirationDate)
                {
                    record.ManufacturingIndex.ExpirationDate = newRecord.ExpirationDate;
                }
                if (record.ManufacturingIndex.ManufacturingDate != newRecord.ManufacturingDate)
                {
                    record.ManufacturingIndex.ManufacturingDate = newRecord.ManufacturingDate;
                }

                record.Employee = newRecord.Employee;

                if (newRecord.WarehouseId == record.WarehouseId)
                {
                    if (record.ManufacturingIndex.Index != newRecord.Index)
                    {
                        record.ManufacturingIndex.Index = newRecord.Index;
                    }

                    if (record.Quantity != newRecord.Quantity)
                    {
                        if (RepositoryExtensions.CalcReplenishLeftovers(record.Warehouse.Leftovers, record.Quantity, newRecord.Quantity) < 0)
                        {
                            return new MethodResult(false, ErrorMessages.WarehouseBelowZero + (record.Warehouse.Leftovers));
                        }
                        record.Warehouse.Leftovers = RepositoryExtensions.CalcReplenishLeftovers(record.Warehouse.Leftovers, record.Quantity, newRecord.Quantity);

                        if (RepositoryExtensions.CalcReplenishLeftovers(record.ManufacturingIndex.Leftovers, record.Quantity, newRecord.Quantity) < 0)
                        {
                            return new MethodResult(false, ErrorMessages.IndexBelowZero + (record.ManufacturingIndex.Leftovers));
                        }
                        record.ManufacturingIndex.Leftovers = RepositoryExtensions.CalcReplenishLeftovers(record.ManufacturingIndex.Leftovers, record.Quantity, newRecord.Quantity);

                        var recordsForLeftoversUpdate = context.ManufacturingRecords.Where(m => m.Id >= recordId && m.WarehouseId == newRecord.WarehouseId).ToList();
                        recordsForLeftoversUpdate.ForEach(m =>
                        {
                            m.Leftovers = RepositoryExtensions.CalcReplenishLeftovers(m.Leftovers, record.Quantity, newRecord.Quantity);
                        });

                        record.Quantity = newRecord.Quantity;
                    }
                }
                else
                {
                    var isIndexUsedForWriteOff = context.ManufacturingRecords.Any(m => m.Id > recordId && m.IndexId == record.IndexId);
                    if (isIndexUsedForWriteOff)
                    {
                        return new MethodResult(false, ErrorMessages.IndexWasUsedForWriteOff);
                    }

                    if (record.Warehouse.Leftovers - record.Quantity < 0)
                    {
                        return new MethodResult(false, ErrorMessages.WarehouseBelowZero + (record.Warehouse.Leftovers));
                    }

                    var oldRecordsForLeftoversUpdate = context.ManufacturingRecords.Where(m => m.Id > recordId && m.WarehouseId == record.WarehouseId).ToList();
                    oldRecordsForLeftoversUpdate.ForEach(m =>
                    {
                        m.Leftovers = m.Leftovers - newRecord.Quantity;
                    });
                    record.Warehouse.Leftovers = record.Warehouse.Leftovers - record.Quantity;

                    if (record.ManufacturingIndex.Index != newRecord.Index)
                    {
                        record.ManufacturingIndex.Index = newRecord.Index;
                    }
                    record.ManufacturingIndex.Leftovers = RepositoryExtensions.CalcReplenishLeftovers(record.ManufacturingIndex.Leftovers, record.Quantity, newRecord.Quantity);
                    record.ManufacturingIndex.WarehouseId = newRecord.WarehouseId;

                    var newWarehouse = context.Warehouse.FirstOrDefault(m => m.Id == newRecord.WarehouseId);
                    newWarehouse.Leftovers = newWarehouse.Leftovers + newRecord.Quantity;
                    record.Warehouse = newWarehouse;

                    var lastRecordLeftovers = context.ManufacturingRecords.Where(m => m.Id < recordId && m.WarehouseId == newRecord.WarehouseId).OrderByDescending(m => m.Id).FirstOrDefault()?.Leftovers ?? 0;
                    var newRecordsForLeftoversUpdate = context.ManufacturingRecords.Where(m => m.Id > recordId && m.WarehouseId == newRecord.WarehouseId).ToList();
                    newRecordsForLeftoversUpdate.ForEach(m =>
                    {
                        m.Leftovers = m.Leftovers + newRecord.Quantity;
                    });

                    record.Leftovers = lastRecordLeftovers + newRecord.Quantity;
                    record.Quantity = newRecord.Quantity;
                }

                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult()
                {
                    IsSuccess = true,
                };
            }
        }

        public MethodResult EditWriteOff(ManufacturingRecordWriteOffModel newRecord, int recordId)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var record = context.ManufacturingRecords.FirstOrDefault(m => m.Id == recordId);
                if (record.ManufacturingIndex == null)
                {
                    return new MethodResult(false, ErrorMessages.IndexWasDeleted);
                }

                if (record.BatchId != newRecord.BatchId)
                {
                    record.BatchId = newRecord.BatchId;
                }
                if (record.DocDate != newRecord.DocDate)
                {
                    record.DocDate = newRecord.DocDate;
                }
                if (record.Remarks != newRecord.Remarks)
                {
                    record.Remarks = newRecord.Remarks;
                }

                record.Employee = newRecord.Employee;

                if (newRecord.WarehouseId == record.WarehouseId)
                {
                    if (record.IndexId != newRecord.IndexId)
                    {
                        var newIndex = context.ManufacturingIndexes.FirstOrDefault(m => m.Id == newRecord.IndexId);
                        if (newIndex == null || record.ManufacturingIndex == null)
                        {
                            return new MethodResult(false, ErrorMessages.IndexIdNotFound);
                        }

                        record.ManufacturingIndex.Leftovers = record.ManufacturingIndex.Leftovers + record.Quantity;

                        if (newIndex.Leftovers - newRecord.Quantity < 0)
                        {
                            return new MethodResult(false, ErrorMessages.IndexBelowZero + (newIndex.Leftovers));
                        }

                        var isReplenishHappend = context.ManufacturingRecords.Any(m => m.Id < recordId && m.IndexId == newRecord.IndexId && m.ActionType == RawActionTypes.Replenishment);
                        if (!isReplenishHappend)
                        {
                            return new MethodResult(false, ErrorMessages.NoReplinishForIndexBeforeWriteOff);
                        }

                        record.ManufacturingIndex = newIndex;
                        record.ManufacturingIndex.Leftovers = record.ManufacturingIndex.Leftovers - newRecord.Quantity;
                    }
                    else
                    {
                        if (RepositoryExtensions.CalcWriteOffLeftovers(record.ManufacturingIndex.Leftovers, record.Quantity, newRecord.Quantity) < 0)
                        {
                            return new MethodResult(false, ErrorMessages.IndexBelowZero + (record.ManufacturingIndex.Leftovers));
                        }
                        record.ManufacturingIndex.Leftovers = RepositoryExtensions.CalcWriteOffLeftovers(record.ManufacturingIndex.Leftovers, record.Quantity, newRecord.Quantity);
                    }

                    if (record.Quantity != newRecord.Quantity)
                    {
                        if (RepositoryExtensions.CalcWriteOffLeftovers(record.Warehouse.Leftovers, record.Quantity, newRecord.Quantity) < 0)
                        {
                            return new MethodResult(false, ErrorMessages.WarehouseBelowZero + (record.Warehouse.Leftovers));
                        }
                        record.Warehouse.Leftovers = RepositoryExtensions.CalcWriteOffLeftovers(record.Warehouse.Leftovers, record.Quantity, newRecord.Quantity);

                        var recordsForLeftoversUpdate = context.ManufacturingRecords.Where(m => m.Id >= recordId && m.WarehouseId == newRecord.WarehouseId).ToList();
                        recordsForLeftoversUpdate.ForEach(m =>
                        {
                            m.Leftovers = RepositoryExtensions.CalcWriteOffLeftovers(m.Leftovers, record.Quantity, newRecord.Quantity);
                        });

                        record.Quantity = newRecord.Quantity;
                    }
                }
                else
                {
                    var newWarehouse = context.Warehouse.FirstOrDefault(m => m.Id == newRecord.WarehouseId);
                    if (newWarehouse.Leftovers - newRecord.Quantity < 0)
                    {
                        return new MethodResult(false, ErrorMessages.WarehouseBelowZero + (newWarehouse.Leftovers));
                    }

                    var newIndexData = context.ManufacturingIndexes.FirstOrDefault(m => m.Id == newRecord.IndexId);
                    if (newIndexData.Leftovers - newRecord.Quantity < 0 || newIndexData.WarehouseId != newWarehouse.Id)
                    {
                        return new MethodResult(false, ErrorMessages.IndexBelowZero + (newIndexData.Leftovers));
                    }

                    var isReplenishHappend = context.ManufacturingRecords.Any(m => m.Id < recordId && m.IndexId == newRecord.IndexId && m.ActionType == RawActionTypes.Replenishment);
                    if (!isReplenishHappend)
                    {
                        return new MethodResult(false, ErrorMessages.NoReplinishForIndexBeforeWriteOff);
                    }

                    var oldRecordsForLeftoversUpdate = context.ManufacturingRecords.Where(m => m.Id > recordId && m.WarehouseId == record.WarehouseId).ToList();
                    oldRecordsForLeftoversUpdate.ForEach(m =>
                    {
                        m.Leftovers = m.Leftovers + record.Quantity;
                    });
                    record.Warehouse.Leftovers = record.Warehouse.Leftovers + record.Quantity;
                    record.ManufacturingIndex.Leftovers = record.ManufacturingIndex.Leftovers + record.Quantity;

                    newWarehouse.Leftovers = newWarehouse.Leftovers - newRecord.Quantity;
                    newIndexData.Leftovers = newIndexData.Leftovers - newRecord.Quantity;
                    record.Warehouse = newWarehouse;
                    record.ManufacturingIndex = newIndexData;

                    var lastRecordLeftovers = context.ManufacturingRecords.Where(m => m.Id < recordId && m.WarehouseId == newRecord.WarehouseId).OrderByDescending(m => m.Id).FirstOrDefault()?.Leftovers ?? 0;
                    var newRecordsForLeftoversUpdate = context.ManufacturingRecords.Where(m => m.Id > recordId && m.WarehouseId == newRecord.WarehouseId).ToList();
                    newRecordsForLeftoversUpdate.ForEach(m =>
                    {
                        m.Leftovers = m.Leftovers - newRecord.Quantity;
                    });

                    record.Leftovers = lastRecordLeftovers - newRecord.Quantity;
                    record.Quantity = newRecord.Quantity;
                }

                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }

        public MethodResult Delete(int id)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var record = context.ManufacturingRecords.FirstOrDefault(m => m.Id == id);
                if (record.ManufacturingIndex == null)
                {
                    return new MethodResult(false, ErrorMessages.IndexWasDeleted);
                }
                if (record.ActionType == RawActionTypes.Return)
                {
                    return new MethodResult(false, ErrorMessages.ReturnEditingNotAllowed);
                }

                if (record.ActionType == RawActionTypes.Replenishment)
                {
                    if (record.ManufacturingIndex.Leftovers - record.Quantity < 0)
                    {
                        return new MethodResult(false, ErrorMessages.IndexBelowZero + (record.ManufacturingIndex.Leftovers));
                    }

                    if (record.Warehouse.Leftovers - record.Quantity < 0)
                    {
                        return new MethodResult(false, ErrorMessages.WarehouseBelowZero + (record.Warehouse.Leftovers));
                    }

                    var isIndexUsedForWriteOff = context.ManufacturingRecords.Any(m => m.Id > record.Id && m.IndexId == record.IndexId);
                    if (isIndexUsedForWriteOff)
                    {
                        return new MethodResult(false, ErrorMessages.IndexWasUsedForWriteOff);
                    }

                    record.Warehouse.Leftovers = record.Warehouse.Leftovers - record.Quantity;
                    context.ManufacturingIndexes.DeleteOnSubmit(record.ManufacturingIndex);

                    var oldRecordsForLeftoversUpdate = context.ManufacturingRecords.Where(m => m.Id > record.Id && m.WarehouseId == record.WarehouseId).ToList();
                    oldRecordsForLeftoversUpdate.ForEach(m =>
                    {
                        m.Leftovers = m.Leftovers - record.Quantity;
                    });
                }
                else
                {
                    var newRecordsForLeftoversUpdate = context.ManufacturingRecords.Where(m => m.Id > record.Id && m.WarehouseId == record.WarehouseId).ToList();
                    newRecordsForLeftoversUpdate.ForEach(m =>
                    {
                        m.Leftovers = m.Leftovers + record.Quantity;
                    });
                    record.Warehouse.Leftovers = record.Warehouse.Leftovers + record.Quantity;
                    record.ManufacturingIndex.Leftovers = record.ManufacturingIndex.Leftovers + record.Quantity;
                }

                context.ManufacturingRecords.DeleteOnSubmit(record);
                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult()
                {
                    IsSuccess = true,
                };
            }
        }

        public int GetRecordCountByDate(DateTime dateStart, DateTime dateEnd)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.ManufacturingRecords
                    .Where(m => m.InsertDate <= dateEnd && m.InsertDate > dateStart);

                return query.Count();
            }
        }

        public DataTable GetDataTableForExcel(DateTime dateStart, DateTime dateEnd)
        {
            var pTargetUserId = new SqlParameter("@dateStart", dateStart);
            var pDataExchangeId = new SqlParameter("@dateEnd", dateEnd);

            using (var data = this.ExecuteProcedure("_Excel_GetData", true, pTargetUserId, pDataExchangeId))
            {
                if (data != null && (data.Rows.Count > 0))
                {
                    return data;
                }
            }

            return null;
        }

        public List<ManufacturingRecordDisplayModel> Search(ManufacturingSearchModel model)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.ManufacturingRecords.AsQueryable();

                if (model.DateStart.HasValue)
                {
                    query = query.Where(m => m.InsertDate > model.DateStart.Value);
                }
                if (model.DateFinish.HasValue)
                {
                    query = query.Where(m => m.InsertDate <= model.DateFinish.Value);
                }
                if (model.Id.HasValue)
                {
                    query = query.Where(m => m.Id == model.Id.Value);
                }
                if (!model.Type.IsNullOrEmpty())
                {
                    query = query.Where(m => m.Warehouse.Type == model.Type);
                }
                if (!model.SubType.IsNullOrEmpty())
                {
                    query = query.Where(m => m.Warehouse.SubType == model.SubType);
                }
                if (!model.Provider.IsNullOrEmpty())
                {
                    query = query.Where(m => m.Warehouse.Provider == model.Provider);
                }
                if (!model.Manufacturer.IsNullOrEmpty())
                {
                    query = query.Where(m => m.Warehouse.Manufacturer == model.Manufacturer);
                }
                if (!model.Index.IsNullOrEmpty())
                {
                    query = query.Where(m => m.ManufacturingIndex.Index == model.Index);
                }
                if (model.BatchId != null)
                {
                    query = query.Where(m => m.BatchId == model.BatchId);
                }
                if (model.BatchTypeId != null)
                {
                    query = query.Where(m => m.Batch.TypeId == model.BatchTypeId);
                }
                if (model.BatchLineId != null)
                {
                    query = query.Where(m => m.Batch.LineId == model.BatchLineId);
                }
                if (model.ReplenishmentDocumentId != null)
                {
                    query = query.Where(m => m.ReplenishmentDocumentId == model.ReplenishmentDocumentId);
                }
                if (model.ShowMode != RawActionTypes.All)
                {
                    query = query.Where(m => m.ActionType == model.ShowMode);
                }

                var result = RepositoryExtensions.JoinManufacturingTableList(query, context);

                return result;
            }
        }

        public ManufacturingRecordModel GetItemForCopy(int id)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var result = context.ManufacturingRecords
                    .Where(m => m.Id == id)
                    .Join(context.Warehouse, mt => mt.WarehouseId, w => w.Id, (mt, w) => new ManufacturingRecordModel()
                    {
                        Type = w.Type,
                        SubType = w.SubType,
                        Provider = w.Provider,
                        Manufacturer = w.Manufacturer,
                        IndexId = mt.IndexId,
                        DocDate = mt.DocDate,
                        Remarks = mt.Remarks,
                    })
                    .FirstOrDefault();

                return result;
            }
        }
    }
}
