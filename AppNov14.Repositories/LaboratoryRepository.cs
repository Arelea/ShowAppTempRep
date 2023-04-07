using AppNov14.Helpers;
using AppNov14.Models.Base;
using AppNov14.Models.ManufacturingTable;
using AppNov14.Repositories.Extensions;
using AppNov14.Repositories.Interfaces;
using AppNov14.SqlDataAccess;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AppNov14.Repositories
{
    public class LaboratoryRepository : BaseDataRepository, ILaboratoryRepository
    {
        public LaboratoryRepository(IConfiguration configuration)
            : base(configuration)
        {
        }

        public MethodResult ReplenishWarehouse(ManufacturngTableFullModel model)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var warehouse = context.Warehouse.Where(m => m.Id == model.WarehouseId).FirstOrDefault();
                warehouse.Leftovers += model.Quantity;

                var isIndexExist = context.LaboratoryIndexes.Any(m => m.Indexation == model.Indexation && m.WarehouseId == model.WarehouseId);
                if (isIndexExist)
                {
                    return new MethodResult()
                    {
                        IsSuccess = false,
                        Message = ErrorMessages.IndexIdNotFoundOrAlreadyExist,
                    };
                }

                var newIndex = new LaboratoryIndex()
                {
                    WarehouseId = model.WarehouseId,
                    Indexation = model.Indexation,
                    AutoDate = DateTime.Now,
                    Leftovers = model.Quantity,
                };

                var lab = new LaboratoryRecord()
                {
                    Quantity = model.Quantity,
                    AutoDate = model.AutoDate,
                    DocDate = model.DocDate,
                    WarehouseId = model.WarehouseId,
                    Leftovers = warehouse.Leftovers,
                    LaboratoryIndex = newIndex,
                    IpAddress = model.IpAddress,
                    Employee = model.Employee,
                    Remarks = model.Remarks,
                    Action = model.OperationType,
                };

                context.LaboratoryRecords.InsertOnSubmit(lab);
                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult()
                {
                    IsSuccess = true,
                };
            }
        }

        public MethodResult WriteOffWarehouse(ManufacturngTableFullModel model)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var warehouse = context.Warehouse.Where(m => m.Id == model.WarehouseId).FirstOrDefault();
                warehouse.Leftovers -= model.Quantity;

                if (warehouse.Leftovers < 0)
                {
                    return new MethodResult()
                    {
                        IsSuccess = false,
                        Message = ErrorMessages.WarehouseBelowZero + (warehouse.Leftovers + model.Quantity),
                    };
                }

                var index = context.LaboratoryIndexes.Where(m => m.Id == model.IndexId).FirstOrDefault();
                index.Leftovers -= model.Quantity;

                if (index.Leftovers < 0)
                {
                    return new MethodResult()
                    {
                        IsSuccess = false,
                        Message = ErrorMessages.IndexBelowZero + (index.Leftovers + model.Quantity),
                    };
                }

                var lab = new LaboratoryRecord()
                {
                    Quantity = model.Quantity,
                    AutoDate = model.AutoDate,
                    DocDate = model.DocDate,
                    WarehouseId = model.WarehouseId,
                    Leftovers = warehouse.Leftovers,
                    IndexId = model.IndexId,
                    IpAddress = model.IpAddress,
                    Employee = model.Employee,
                    Remarks = model.Remarks,
                    Action = model.OperationType,
                    Document = model.Document,
                    DocumentNumber = model.DocumentNumber,
                };

                var isCosignmentNumberExist = context.ConsLabNumbers.Any(m => m.Name == model.DocumentNumber);
                if (!isCosignmentNumberExist)
                {
                    var consignmentNumber = new ConsLabNumber()
                    {
                        Name = model.DocumentNumber,
                        AutoDate = model.AutoDate,
                    };
                    context.ConsLabNumbers.InsertOnSubmit(consignmentNumber);
                }

                context.LaboratoryRecords.InsertOnSubmit(lab);
                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult()
                {
                    IsSuccess = true,
                };
            }
        }

        public List<ManufacturingTableWriteModel> GetLastAdding()
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.LaboratoryRecords
                    .OrderByDescending(m => m.Id)
                    .Take(Other.EmployeeLastAddingsCount);

                var result = RepositoryExtensions.JoinLaboratoryTableList(query, context);

                return result;
            }
        }

        public List<ManufacturingTableWriteModel> GetItemsByStartAndEndDate(DateTime dateStart, DateTime dateEnd)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.LaboratoryRecords
                    .Where(m => m.AutoDate <= dateEnd && m.AutoDate > dateStart)
                    .OrderByDescending(m => m.Id);

                var result = RepositoryExtensions.JoinLaboratoryTableList(query, context);

                return result;
            }
        }

        public ManufacturingTableWriteModel GetById(int id)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.LaboratoryRecords
                    .Where(m => m.Id == id);

                var result = RepositoryExtensions.JoinLaboratoryTable(query, context);

                return result;
            }
        }

        public MethodResult EditReplenish(ManufacturngTableFullModel newRecord)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var record = context.LaboratoryRecords.Where(m => m.Id == newRecord.Id).FirstOrDefault();
                if (record.LaboratoryIndex == null)
                {
                    return new MethodResult()
                    {
                        IsSuccess = false,
                        Message = ErrorMessages.IndexWasDeleted,
                    };
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
                    if (record.LaboratoryIndex.Indexation != newRecord.Indexation)
                    {
                        record.LaboratoryIndex.Indexation = newRecord.Indexation;
                    }

                    if (record.Quantity != newRecord.Quantity)
                    {
                        if (RepositoryExtensions.CalcReplenishLeftovers(record.Warehouse.Leftovers, record.Quantity, newRecord.Quantity) < 0)
                        {
                            return new MethodResult()
                            {
                                IsSuccess = false,
                                Message = ErrorMessages.WarehouseBelowZero + (record.Warehouse.Leftovers),
                            };
                        }
                        record.Warehouse.Leftovers = RepositoryExtensions.CalcReplenishLeftovers(record.Warehouse.Leftovers, record.Quantity, newRecord.Quantity);

                        if (RepositoryExtensions.CalcReplenishLeftovers(record.LaboratoryIndex.Leftovers, record.Quantity, newRecord.Quantity) < 0)
                        {
                            return new MethodResult()
                            {
                                IsSuccess = false,
                                Message = ErrorMessages.IndexBelowZero + (record.LaboratoryIndex.Leftovers),
                            };
                        }
                        record.LaboratoryIndex.Leftovers = RepositoryExtensions.CalcReplenishLeftovers(record.LaboratoryIndex.Leftovers, record.Quantity, newRecord.Quantity);

                        var recordsForLeftoversUpdate = context.LaboratoryRecords.Where(m => m.Id >= newRecord.Id && m.WarehouseId == newRecord.WarehouseId).ToList();
                        recordsForLeftoversUpdate.ForEach(m =>
                        {
                            m.Leftovers = RepositoryExtensions.CalcReplenishLeftovers(m.Leftovers, record.Quantity, newRecord.Quantity);
                        });

                        record.Quantity = newRecord.Quantity;
                    }
                }
                else
                {
                    var isIndexUsedForWriteOff = context.LaboratoryRecords.Where(m => m.Id > record.Id && m.IndexId == record.IndexId).Any();
                    if (isIndexUsedForWriteOff)
                    {
                        return new MethodResult()
                        {
                            IsSuccess = false,
                            Message = ErrorMessages.IndexWasUsedForWriteOff,
                        };
                    }

                    if (record.Warehouse.Leftovers - record.Quantity < 0)
                    {
                        return new MethodResult()
                        {
                            IsSuccess = false,
                            Message = ErrorMessages.WarehouseBelowZero + (record.Warehouse.Leftovers),
                        };
                    }
                    var oldRecordsForLeftoversUpdate = context.LaboratoryRecords.Where(m => m.Id > newRecord.Id && m.WarehouseId == record.WarehouseId).ToList();
                    oldRecordsForLeftoversUpdate.ForEach(m =>
                    {
                        m.Leftovers = m.Leftovers - newRecord.Quantity;
                    });
                    record.Warehouse.Leftovers = record.Warehouse.Leftovers - record.Quantity;

                    if (record.LaboratoryIndex.Indexation != newRecord.Indexation)
                    {
                        record.LaboratoryIndex.Indexation = newRecord.Indexation;
                    }
                    record.LaboratoryIndex.Leftovers = RepositoryExtensions.CalcReplenishLeftovers(record.LaboratoryIndex.Leftovers, record.Quantity, newRecord.Quantity);
                    record.LaboratoryIndex.WarehouseId = newRecord.WarehouseId;

                    var newWarehouse = context.Warehouse.FirstOrDefault(m => m.Id == newRecord.WarehouseId);
                    newWarehouse.Leftovers = newWarehouse.Leftovers + newRecord.Quantity;
                    record.Warehouse = newWarehouse;

                    var lastRecordLeftovers = context.LaboratoryRecords.Where(m => m.Id < newRecord.Id && m.WarehouseId == newRecord.WarehouseId).OrderByDescending(m => m.Id).FirstOrDefault()?.Leftovers ?? 0;
                    var newRecordsForLeftoversUpdate = context.LaboratoryRecords.Where(m => m.Id > newRecord.Id && m.WarehouseId == newRecord.WarehouseId).ToList();
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

        public MethodResult EditWriteOff(ManufacturngTableFullModel newRecord)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var record = context.LaboratoryRecords.Where(m => m.Id == newRecord.Id).FirstOrDefault();
                if (record.LaboratoryIndex == null)
                {
                    return new MethodResult()
                    {
                        IsSuccess = false,
                        Message = ErrorMessages.IndexWasDeleted,
                    };
                }

                if (record.DocumentNumber != newRecord.DocumentNumber)
                {
                    var isCosignmentNumberExist = context.ConsLabNumbers.Any(m => m.Name == newRecord.DocumentNumber);
                    if (!isCosignmentNumberExist)
                    {
                        var consignmentNumber = new ConsLabNumber()
                        {
                            Name = newRecord.DocumentNumber,
                            AutoDate = DateTime.Now,
                        };
                        context.ConsLabNumbers.InsertOnSubmit(consignmentNumber);
                    }

                    record.DocumentNumber = newRecord.DocumentNumber;
                }
                if (record.Document != newRecord.Document)
                {
                    record.Document = newRecord.Document;
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
                    if (record.LaboratoryIndex.Indexation != newRecord.Indexation)
                    {
                        var newIndex = context.LaboratoryIndexes.FirstOrDefault(m => m.Id == newRecord.IndexId);
                        if (newIndex == null || record.LaboratoryIndex == null)
                        {
                            return new MethodResult()
                            {
                                IsSuccess = false,
                                Message = ErrorMessages.IndexIdNotFound,
                            };
                        }

                        record.LaboratoryIndex.Leftovers = record.LaboratoryIndex.Leftovers + record.Quantity;

                        if (newIndex.Leftovers - newRecord.Quantity < 0)
                        {
                            return new MethodResult()
                            {
                                IsSuccess = false,
                                Message = ErrorMessages.IndexBelowZero + (newIndex.Leftovers),
                            };
                        }

                        var isReplenishHappend = context.LaboratoryRecords.Any(m => m.Id < newRecord.Id && m.IndexId == newRecord.IndexId && m.Action == RawActionTypes.Replenishment);
                        if (!isReplenishHappend)
                        {
                            return new MethodResult()
                            {
                                IsSuccess = false,
                                Message = ErrorMessages.NoReplinishForIndexBeforeWriteOff,
                            };
                        }

                        record.LaboratoryIndex = newIndex;
                        record.LaboratoryIndex.Leftovers = record.LaboratoryIndex.Leftovers - newRecord.Quantity;
                    }
                    else
                    {
                        if (RepositoryExtensions.CalcWriteOffLeftovers(record.LaboratoryIndex.Leftovers, record.Quantity, newRecord.Quantity) < 0)
                        {
                            return new MethodResult()
                            {
                                IsSuccess = false,
                                Message = ErrorMessages.IndexBelowZero + (record.LaboratoryIndex.Leftovers),
                            };
                        }
                        record.LaboratoryIndex.Leftovers = RepositoryExtensions.CalcWriteOffLeftovers(record.LaboratoryIndex.Leftovers, record.Quantity, newRecord.Quantity);
                    }

                    if (record.Quantity != newRecord.Quantity)
                    {
                        if (RepositoryExtensions.CalcWriteOffLeftovers(record.Warehouse.Leftovers, record.Quantity, newRecord.Quantity) < 0)
                        {
                            return new MethodResult()
                            {
                                IsSuccess = false,
                                Message = ErrorMessages.WarehouseBelowZero + (record.Warehouse.Leftovers),
                            };
                        }
                        record.Warehouse.Leftovers = RepositoryExtensions.CalcWriteOffLeftovers(record.Warehouse.Leftovers, record.Quantity, newRecord.Quantity);

                        var recordsForLeftoversUpdate = context.LaboratoryRecords.Where(m => m.Id >= newRecord.Id && m.WarehouseId == newRecord.WarehouseId).ToList();
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
                        return new MethodResult()
                        {
                            IsSuccess = false,
                            Message = ErrorMessages.WarehouseBelowZero + (newWarehouse.Leftovers),
                        };
                    }

                    var newIndexData = context.LaboratoryIndexes.FirstOrDefault(m => m.Id == newRecord.IndexId);
                    if (newIndexData.Leftovers - newRecord.Quantity < 0 || newIndexData.WarehouseId != newWarehouse.Id)
                    {
                        return new MethodResult()
                        {
                            IsSuccess = false,
                            Message = ErrorMessages.IndexBelowZero + (newIndexData.Leftovers),
                        };
                    }

                    var isReplenishHappend = context.LaboratoryRecords.Any(m => m.Id < newRecord.Id && m.IndexId == newRecord.IndexId && m.Action == RawActionTypes.Replenishment);
                    if (!isReplenishHappend)
                    {
                        return new MethodResult()
                        {
                            IsSuccess = false,
                            Message = ErrorMessages.NoReplinishForIndexBeforeWriteOff,
                        };
                    }

                    var oldRecordsForLeftoversUpdate = context.LaboratoryRecords.Where(m => m.Id > newRecord.Id && m.WarehouseId == record.WarehouseId).ToList();
                    oldRecordsForLeftoversUpdate.ForEach(m =>
                    {
                        m.Leftovers = m.Leftovers + record.Quantity;
                    });
                    record.Warehouse.Leftovers = record.Warehouse.Leftovers + record.Quantity;
                    record.LaboratoryIndex.Leftovers = record.LaboratoryIndex.Leftovers + record.Quantity;

                    newWarehouse.Leftovers = newWarehouse.Leftovers - newRecord.Quantity;
                    newIndexData.Leftovers = newIndexData.Leftovers - newRecord.Quantity;
                    record.Warehouse = newWarehouse;
                    record.LaboratoryIndex = newIndexData;

                    var lastRecordLeftovers = context.LaboratoryRecords.Where(m => m.Id < newRecord.Id && m.WarehouseId == newRecord.WarehouseId).OrderByDescending(m => m.Id).FirstOrDefault()?.Leftovers ?? 0;
                    var newRecordsForLeftoversUpdate = context.LaboratoryRecords.Where(m => m.Id > newRecord.Id && m.WarehouseId == newRecord.WarehouseId).ToList();
                    newRecordsForLeftoversUpdate.ForEach(m =>
                    {
                        m.Leftovers = m.Leftovers - newRecord.Quantity;
                    });

                    record.Leftovers = lastRecordLeftovers - newRecord.Quantity;
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

        public MethodResult Delete(int id)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var record = context.LaboratoryRecords.Where(m => m.Id == id).FirstOrDefault();
                if (record.LaboratoryIndex == null)
                {
                    return new MethodResult()
                    {
                        IsSuccess = false,
                        Message = ErrorMessages.IndexWasDeleted,
                    };
                }

                if (record.Action == RawActionTypes.Replenishment)
                {
                    if (record.LaboratoryIndex.Leftovers - record.Quantity < 0)
                    {
                        return new MethodResult()
                        {
                            IsSuccess = false,
                            Message = ErrorMessages.IndexBelowZero + (record.LaboratoryIndex.Leftovers),
                        };
                    }

                    if (record.Warehouse.Leftovers - record.Quantity < 0)
                    {
                        return new MethodResult()
                        {
                            IsSuccess = false,
                            Message = ErrorMessages.WarehouseBelowZero + (record.Warehouse.Leftovers),
                        };
                    }

                    var isIndexUsedForWriteOff = context.LaboratoryRecords.Where(m => m.Id > record.Id && m.IndexId == record.IndexId).Any();
                    if (isIndexUsedForWriteOff)
                    {
                        return new MethodResult()
                        {
                            IsSuccess = false,
                            Message = ErrorMessages.IndexWasUsedForWriteOff,
                        };
                    }

                    record.Warehouse.Leftovers = record.Warehouse.Leftovers - record.Quantity;                
                    context.LaboratoryIndexes.DeleteOnSubmit(record.LaboratoryIndex);

                    var oldRecordsForLeftoversUpdate = context.LaboratoryRecords.Where(m => m.Id > record.Id && m.WarehouseId == record.WarehouseId).ToList();
                    oldRecordsForLeftoversUpdate.ForEach(m =>
                    {
                        m.Leftovers = m.Leftovers - record.Quantity;
                    });
                }
                else
                {
                    var newRecordsForLeftoversUpdate = context.LaboratoryRecords.Where(m => m.Id > record.Id && m.WarehouseId == record.WarehouseId).ToList();
                    newRecordsForLeftoversUpdate.ForEach(m =>
                    {
                        m.Leftovers = m.Leftovers + record.Quantity;
                    });

                    var consigmentNumber = context.ConsLabNumbers.Where(m => m.Name == record.DocumentNumber).ToList();
                    if (consigmentNumber != null && consigmentNumber.Count == 1)
                    {
                        context.ConsLabNumbers.DeleteOnSubmit(consigmentNumber.First());
                    }

                    record.Warehouse.Leftovers = record.Warehouse.Leftovers + record.Quantity;
                    record.LaboratoryIndex.Leftovers = record.LaboratoryIndex.Leftovers + record.Quantity;
                }

                context.LaboratoryRecords.DeleteOnSubmit(record);
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
                var query = context.LaboratoryRecords
                    .Where(m => m.AutoDate <= dateEnd && m.AutoDate > dateStart);

                return query.Count();
            }
        }

        public DataTable GetDataTableForExcel(DateTime dateStart, DateTime dateEnd)
        {
            var pTargetUserId = new SqlParameter("@dateStart", dateStart);
            var pDataExchangeId = new SqlParameter("@dateEnd", dateEnd);

            using (var data = this.ExecuteProcedure("_Excel_GetDataLab", true, pTargetUserId, pDataExchangeId))
            {
                if (data != null && (data.Rows.Count > 0))
                {
                    return data;
                }
            }

            return null;
        }

        public List<ManufacturingTableWriteModel> GetItemsByConsignmentNumber(string number)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.LaboratoryRecords
                    .Where(m => m.DocumentNumber == number)
                    .OrderByDescending(m => m.Id);

                var result = RepositoryExtensions.JoinLaboratoryTableList(query, context);

                return result;
            }
        }
    }
}
