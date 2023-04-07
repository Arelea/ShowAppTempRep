using AppNov14.Helpers;
using AppNov14.Models.Base;
using AppNov14.Models.Batch;
using AppNov14.Repositories.Extensions;
using AppNov14.Repositories.Interfaces;
using AppNov14.SqlDataAccess;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Castle.Core.Internal;

namespace AppNov14.Repositories
{
    public sealed class BatchRepository : BaseDataRepository, IBatchRepository
    {
        public BatchRepository(IConfiguration configuration)
            : base(configuration)
        {
        }

        public bool IsBatchExist(int batchId)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                return context.Batches.Any(m => m.Id == batchId);
            }
        }

        public MethodResult AddBatchType(string name)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var isBatchTypeExist = context.BatchTypes.Any(m => m.Name == name);
                if (isBatchTypeExist)
                {
                    return new MethodResult(false, ErrorMessages.DocumentTypeAlreadyExist);
                }

                var newBatchType = new BatchType()
                {
                    Name = name,
                    InsertDate = DateTime.Now,
                };

                context.BatchTypes.InsertOnSubmit(newBatchType);
                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }

        public bool IsBatchExistByName(string batchName)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                return context.Batches.Any(m => m.Name == batchName);
            }
        }

        public List<BatchDisplayModel> GetBatchList(List<int> statusIdList, int? batchId)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.Batches.Where(m => statusIdList.Contains(m.StatusId));
                if (batchId.HasValue)
                {
                    query = query.Where(m => m.Id == batchId.Value);
                }

                var result = query.Select(m => new BatchDisplayModel()
                {
                    Id = m.Id,
                    Type = m.BatchType.Name,
                    StatusId = m.StatusId,
                    Line = m.BatchLine.Name,
                    InsertDate = m.InsertDate,
                    Name = m.Name,
                    CreateDate = m.CreateDate,
                    StatusCssClass = m.BatchStatus.CssClass,
                }).OrderByDescending(m => m.InsertDate).ToList();

                return result;
            }
        }

        public List<BatchDisplayModel> GetBatchListWithoutChilds(List<int> statusIdList, int batchId)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.Batches.Where(m => statusIdList.Contains(m.StatusId) && m.ParentChildBatches.All(n => n.ChildBatchId != batchId));

                var result = query.Select(m => new BatchDisplayModel()
                {
                    Id = m.Id,
                    Type = m.BatchType.Name,
                    StatusId = m.StatusId,
                    Line = m.BatchLine.Name,
                    InsertDate = m.InsertDate,
                    Name = m.Name,
                    CreateDate = m.CreateDate,
                    StatusCssClass = m.BatchStatus.CssClass,
                }).OrderByDescending(m => m.InsertDate).ToList();

                return result;
            }
        }

        public BatchDisplayModel GetDisplayBatch(int id)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.Batches.Where(m => m.Id == id);

                var result = query.Select(m => new BatchDisplayModel()
                {
                    Id = m.Id,
                    Type = m.BatchType.Name,
                    StatusId = m.StatusId,
                    Line = m.BatchLine.Name,
                    InsertDate = m.InsertDate,
                    Name = m.Name,
                    CreateDate = m.CreateDate,
                    StatusCssClass = m.BatchStatus.CssClass,
                }).FirstOrDefault();

                return result;
            }
        }

        public BatchDisplayModel GetDisplayBatchWithCompounds(int id)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.Batches.Where(m => m.Id == id);

                var result = query.Select(m => new BatchDisplayModel()
                {
                    Id = m.Id,
                    Type = m.BatchType.Name,
                    StatusId = m.StatusId,
                    Line = m.BatchLine.Name,
                    InsertDate = m.InsertDate,
                    Name = m.Name,
                    CreateDate = m.CreateDate,
                    StatusCssClass = m.BatchStatus.CssClass,
                }).FirstOrDefault();

                if (result != null)
                {
                    var queryManuf = context.ManufacturingRecords
                        .Where(m => m.BatchId == id)
                        .OrderByDescending(m => m.Id);

                    result.Compounds = RepositoryExtensions.JoinShortManufacturingTableList(queryManuf, context);
                }

                return result;
            }
        }

        public BatchModel GetBatch(int id)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var batch = context.Batches
                    .Where(m => m.Id == id)
                    .Select(m => new BatchModel()
                    {
                        Id = m.Id,
                        TypeId = m.TypeId,
                        StatusId = m.StatusId,
                        LineId = m.LineId,
                        Name = m.Name,
                        CreateDate = m.CreateDate,
                    }).FirstOrDefault();

                return batch;
            }
        }

        public MethodResult EditBatch(BatchModel model)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var record = context.Batches.FirstOrDefault(m => m.Id == model.Id);
                if (record == null)
                {
                    return new MethodResult(false, ErrorMessages.BatchNotFound);
                }
                if (record.Name != model.Name)
                {
                    var isNameExist = context.Batches.Any(m => m.Name == model.Name);
                    if (isNameExist)
                    {
                        return new MethodResult(false, ErrorMessages.BatchNameAlreadyExist);
                    }
                    record.Name = model.Name;
                }
                if (record.TypeId != model.TypeId)
                {
                    record.TypeId = model.TypeId;
                }
                if (record.LineId != model.LineId)
                {
                    record.LineId = model.LineId;
                }
                if (record.StatusId != model.StatusId)
                {
                    record.StatusId = model.StatusId;
                }
                if (record.CreateDate != model.CreateDate)
                {
                    record.CreateDate = model.CreateDate;
                }

                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }

        public MethodResult AddCustomerName(string name)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var isCustomerExist = context.Customers.Any(m => m.Name == name);
                if (isCustomerExist)
                {
                    return new MethodResult(false, ErrorMessages.CustomerNameAlreadyExist);
                }

                var newCustomerName = new Customer()
                {
                    Name = name,
                };

                context.Customers.InsertOnSubmit(newCustomerName);
                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }

        public Dictionary<int, string> GetBatchesByIndexId(int indexId)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.Batches
                    .Where(m => m.ManufacturingRecords.Any(n => n.IndexId == indexId));

                return query.ToDictionary(m => m.Id, m => m.Name);
            }
        }

        public MethodResult MoveCompletedBatch(int id, int initialPackage, decimal initialQuantity, DateTime completionDate, string remark)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var batch = context.Batches.FirstOrDefault(m => m.Id == id && (m.StatusId == BatchStatuses.NoData || m.StatusId == BatchStatuses.InManufacturingProcess));
                if (batch == null)
                {
                    return new MethodResult(false, ErrorMessages.BatchNotFound);
                }
                if (initialPackage <= 0 || initialQuantity <= 0)
                {
                    return new MethodResult(false, ErrorMessages.ZeroNotAllowed);
                }
                if (batch.CreateDate.HasValue && (batch.CreateDate.Value > completionDate))
                {
                    return new MethodResult(false, ErrorMessages.BatchCreateDateException);
                }

                batch.StatusId = BatchStatuses.Completed;
                batch.InitialPackage = initialPackage;
                batch.InitialQuantity = initialQuantity;
                batch.CurrentPackage = initialPackage;
                batch.CurrentQuantity = initialQuantity;
                batch.CompletionDate = completionDate;
                batch.LastUpdateDate = DateTime.Now;

                var newBatchHistory = new BatchHistory()
                {
                    Text = BatchHistoryTexts.GetTextByAction(BatchHistoryActions.CompleteAction, batch.Name),
                    OperationTypeId = BatchHistoryOperations.UsualOperation,
                    ActionTypeId = BatchHistoryActions.CompleteAction,
                    Package = initialPackage,
                    Quantity = initialQuantity,
                    LeftPackage = initialPackage,
                    LeftQuantity = initialQuantity,
                    Remark = remark,
                    InsertDate = DateTime.Now,
                };

                batch.BatchHistories.Add(newBatchHistory);

                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }

        public BatchFullDisplayModel GetFullDisplayBatchWithCompounds(int id)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.Batches.Where(m => m.Id == id);

                var result = query.Select(m => new BatchFullDisplayModel()
                {
                    Id = m.Id,
                    Type = m.BatchType.Name,
                    StatusId = m.StatusId,
                    Line = m.BatchLine.Name,
                    InsertDate = m.InsertDate,
                    Name = m.Name,
                    CreateDate = m.CreateDate,
                    InitialPackage = m.InitialPackage,
                    InitialQuantity = m.InitialQuantity,
                    CompletionDate = m.CompletionDate,
                    CurrentQuantity = m.CurrentQuantity,
                    CurrentPackage = m.CurrentPackage,
                    StatusCssClass = m.BatchStatus.CssClass,
                    BatchHistories = m.BatchHistories.Select(h => new BatchHistoriesDisplayModel()
                    {
                        Text = h.Text,
                        Remark = h.Remark,
                        Quantity = h.Quantity,
                        Package = h.Package,
                        Id = h.Id,
                        BatchId = h.BatchId,
                        InsertDate = h.InsertDate,
                        CustomerId = h.CustomerId,
                        CustomerName = h.CustomerId.HasValue ? h.Customer.Name : null,
                        SoldDate = h.SoldDate,
                        ActionTypeId = h.ActionTypeId,
                        OperationTypeId = h.OperationTypeId,
                        LinkedBatchId = h.LinkedBatchId,
                        LinkedBatchName = h.BatchLinked.Name,
                        ActionTypeName = h.BatchHistoryActionType.Name,
                        OperationTypeName = h.BatchHistoryOperationType.Name,
                        ActionCssClass = h.BatchHistoryActionType.CssClass,
                        OperationCssClass = h.BatchHistoryOperationType.CssClass,
                        LeftPackage = h.LeftPackage,
                        LeftQuantity = h.LeftQuantity,
                        ReturnDate = h.ReturnDate,
                    })
                    .OrderBy(h => h.InsertDate)
                    .ToList(),
                    ChildBatches = m.ParentChildBatches.Select(c => new BatchDisplayModel()
                    {
                        Name = c.ChildrenBatch.Name,
                        Id = c.ChildBatchId,
                        Type = c.ChildrenBatch.BatchType.Name,
                        Line = c.ChildrenBatch.BatchLine.DisplayName,

                    }).ToList(),
                    ParentBatches = m.ChildrenChildBatches.Select(c => new BatchDisplayModel()
                    {
                        Name = c.ParentBatch.Name,
                        Id = c.BatchId,
                        Type = c.ParentBatch.BatchType.Name,
                        Line = c.ParentBatch.BatchLine.DisplayName,

                    }).ToList(),
                }).FirstOrDefault();

                if (result != null)
                {
                    var queryManuf = context.ManufacturingRecords
                        .Where(m => m.BatchId == id)
                        .OrderByDescending(m => m.Id);

                    result.Compounds = RepositoryExtensions.JoinShortManufacturingTableList(queryManuf, context);
                }

                return result;
            }
        }

        public BatchExtendedDisplayModel GetExtendedDisplayBatch(int id)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.Batches.Where(m => m.Id == id);

                var result = query.Select(m => new BatchExtendedDisplayModel()
                {
                    Id = m.Id,
                    Type = m.BatchType.Name,
                    StatusId = m.StatusId,
                    Line = m.BatchLine.Name,
                    InsertDate = m.InsertDate,
                    Name = m.Name,
                    CreateDate = m.CreateDate,
                    InitialPackage = m.InitialPackage,
                    InitialQuantity = m.InitialQuantity,
                    CompletionDate = m.CompletionDate,
                    CurrentQuantity = m.CurrentQuantity,
                    CurrentPackage = m.CurrentPackage,
                    StatusCssClass = m.BatchStatus.CssClass,
                }).FirstOrDefault();

                return result;
            }
        }

        public MethodResult SellBatch(int id, int package, decimal quantity, int customerId, DateTime soldDate, string remark)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var batch = context.Batches.FirstOrDefault(m => m.Id == id && m.StatusId == BatchStatuses.Completed);
                if (batch == null)
                {
                    return new MethodResult(false, ErrorMessages.BatchNotFound);
                }
                if (package <= 0 || quantity <= 0)
                {
                    return new MethodResult(false, ErrorMessages.ZeroNotAllowed);
                }
                var customerExist = context.Customers.Any(m => m.Id == customerId);
                if (!customerExist)
                {
                    return new MethodResult(false, ErrorMessages.CustomerNotExist);
                }
                if ((batch.CurrentPackage > batch.InitialPackage) || (batch.CurrentPackage - package < 0))
                {
                    return new MethodResult(false, ErrorMessages.BatchPackageBelowZero);
                }
                if ((batch.CurrentQuantity > batch.InitialQuantity) || (batch.CurrentQuantity - quantity < 0))
                {
                    return new MethodResult(false, ErrorMessages.BatchQuantityBelowZero);
                }
                if (batch.CompletionDate.HasValue && (batch.CompletionDate.Value > soldDate))
                {
                    return new MethodResult(false, ErrorMessages.BatchSoldDateException);
                }
                if ((batch.CurrentPackage - package == 0) && (batch.CurrentQuantity - quantity > 0) || (batch.CurrentPackage - package > 0) && (batch.CurrentQuantity - quantity == 0))
                {
                    return new MethodResult(false, ErrorMessages.BatchAllMustBeNullException);
                }

                batch.CurrentPackage = batch.CurrentPackage - package;
                batch.CurrentQuantity = batch.CurrentQuantity - quantity;
                batch.LastUpdateDate = DateTime.Now;

                var newBatchHistory = new BatchHistory()
                {
                    Text = BatchHistoryTexts.GetTextByAction(BatchHistoryActions.SellAction, batch.Name),
                    OperationTypeId = BatchHistoryOperations.UsualOperation,
                    ActionTypeId = BatchHistoryActions.SellAction,
                    Package = package,
                    Quantity = quantity,
                    LeftPackage = batch.CurrentPackage,
                    LeftQuantity = batch.CurrentQuantity,
                    Remark = remark,
                    InsertDate = DateTime.Now,
                    SoldDate = soldDate,
                    CustomerId = customerId,
                };

                batch.BatchHistories.Add(newBatchHistory);

                if (batch.CurrentPackage == 0 && batch.CurrentQuantity == 0)
                {
                    var newBatchHistoryInfo = new BatchHistory()
                    {
                        Text = $"Партия «{batch.Name}» полностью израсходована.",
                        OperationTypeId = BatchHistoryOperations.InformationOperation,
                        ActionTypeId = BatchHistoryActions.NoneAction,
                        InsertDate = DateTime.Now.AddMinutes(1),
                    };
                    batch.BatchHistories.Add(newBatchHistoryInfo);
                    batch.StatusId = BatchStatuses.Empty;
                }

                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }

        public BatchFullDisplayModel GetFullDisplayBatchWithoutCompounds(int id)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.Batches.Where(m => m.Id == id);

                var result = query.Select(m => new BatchFullDisplayModel()
                {
                    Id = m.Id,
                    Type = m.BatchType.Name,
                    StatusId = m.StatusId,
                    Line = m.BatchLine.Name,
                    InsertDate = m.InsertDate,
                    Name = m.Name,
                    CreateDate = m.CreateDate,
                    InitialPackage = m.InitialPackage,
                    InitialQuantity = m.InitialQuantity,
                    CompletionDate = m.CompletionDate,
                    CurrentQuantity = m.CurrentQuantity,
                    CurrentPackage = m.CurrentPackage,
                    StatusCssClass = m.BatchStatus.CssClass,
                    BatchHistories = m.BatchHistories.Select(h => new BatchHistoriesDisplayModel()
                    {
                        Text = h.Text,
                        Remark = h.Remark,
                        Quantity = h.Quantity,
                        Package = h.Package,
                        Id = h.Id,
                        BatchId = h.BatchId,
                        InsertDate = h.InsertDate,
                        CustomerId = h.CustomerId,
                        CustomerName = h.CustomerId.HasValue ? h.Customer.Name : null,
                        SoldDate = h.SoldDate,
                        ActionTypeId = h.ActionTypeId,
                        OperationTypeId = h.OperationTypeId,
                        LinkedBatchId = h.LinkedBatchId,
                        ActionTypeName = h.BatchHistoryActionType.Name,
                        OperationTypeName = h.BatchHistoryOperationType.Name,
                        ActionCssClass = h.BatchHistoryActionType.CssClass,
                        OperationCssClass = h.BatchHistoryOperationType.CssClass,
                        LeftPackage = h.LeftPackage,
                        LeftQuantity = h.LeftQuantity,
                        ReturnDate = h.ReturnDate,
                    })
                    .OrderBy(h => h.InsertDate)
                    .ToList(),
                }).FirstOrDefault();

                return result;
            }
        }

        public BatchReturnDisplayModel GetReturnDisplayBatch(int id)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.Batches.Where(m => m.Id == id);

                var result = query.Select(m => new BatchReturnDisplayModel()
                {
                    Id = m.Id,
                    Type = m.BatchType.Name,
                    StatusId = m.StatusId,
                    Line = m.BatchLine.Name,
                    Name = m.Name,
                    ValueList = m.BatchHistories.Where(h => h.Quantity.HasValue && h.Package.HasValue && h.CustomerId.HasValue && h.SoldDate.HasValue && h.ActionTypeId == BatchHistoryActions.SellAction).GroupBy(h => h.CustomerId).Select(h => h.FirstOrDefault()).Select(h => new BatchQuantityModel()
                    {
                        CustomerId = h.CustomerId.Value,
                        CustomerName = h.Customer.Name,
                    })
                     .ToList(),
                }).FirstOrDefault();

                if (result != null && result.ValueList != null && result.ValueList.Any())
                {
                    foreach (var item in result.ValueList)
                    {
                        var parameters = context.BatchHistories
                            .Where(m => m.CustomerId == item.CustomerId && m.ActionTypeId == BatchHistoryActions.SellAction && m.BatchId == result.Id)
                            .Select(m => new QuantityPerCustomerModel
                            {
                                Quantity = m.Quantity.Value,
                                Package = m.Package.Value,
                                SoldDate = m.SoldDate.Value,
                            }).ToList();

                        var returnParameters = context.BatchHistories
                            .Where(m => m.CustomerId == item.CustomerId && m.ActionTypeId == BatchHistoryActions.ReturnFromCustomerAction && m.BatchId == result.Id)
                            .Select(m => new
                            {
                                ReturnQuantity = m.Quantity.Value,
                                ReturnPackage = m.Package.Value,
                            }).ToList();

                        item.Params = new List<QuantityPerCustomerModel>();
                        item.Params.AddRange(parameters);
                        item.SumPackage = parameters.Sum(m => m.Package);
                        item.SumQuantity = parameters.Sum(m => m.Quantity);
                        if (returnParameters.Any())
                        {
                            item.SumReturnPackage = returnParameters.Sum(m => m.ReturnPackage);
                            item.SumReturnQuantity = returnParameters.Sum(m => m.ReturnQuantity);
                        }
                    }
                }

                return result;
            }
        }

        public MethodResult ReturnBatch(int id, int package, decimal quantity, int customerId, DateTime returnDate, string remark)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var batch = context.Batches.FirstOrDefault(m => m.Id == id && (m.StatusId == BatchStatuses.Completed || m.StatusId == BatchStatuses.Empty));
                if (batch == null)
                {
                    return new MethodResult(false, ErrorMessages.BatchNotFound);
                }
                if (package <= 0 || quantity <= 0)
                {
                    return new MethodResult(false, ErrorMessages.ZeroNotAllowed);
                }
                var customerExist = context.Customers.Any(m => m.Id == customerId);
                if (!customerExist)
                {
                    return new MethodResult(false, ErrorMessages.CustomerNotExist);
                }

                var returnSumValues = context.Batches
                    .Where(m => m.Id == batch.Id && m.BatchHistories.Any(n => n.ActionTypeId == BatchHistoryActions.ReturnFromCustomerAction && n.CustomerId == customerId))
                    .Select(m => new
                    {
                        SumQuantity = m.BatchHistories.Where(r => r.CustomerId == customerId && r.ActionTypeId == BatchHistoryActions.ReturnFromCustomerAction).Sum(r => r.Quantity),
                        SumPackage = m.BatchHistories.Where(r => r.CustomerId == customerId && r.ActionTypeId == BatchHistoryActions.ReturnFromCustomerAction).Sum(r => r.Package),
                    }).FirstOrDefault();

                var sumValues = context.Batches
                    .Where(m => m.Id == batch.Id && m.BatchHistories.Any(n => n.ActionTypeId == BatchHistoryActions.SellAction && n.CustomerId == customerId))
                    .Select(m => new TempBatchReturnModel
                    {
                        SumQuantity = (int)m.BatchHistories.Where(r => r.CustomerId == customerId && r.ActionTypeId == BatchHistoryActions.SellAction).Sum(r => r.Quantity),
                        SumPackage = (int)m.BatchHistories.Where(r => r.CustomerId == customerId && r.ActionTypeId == BatchHistoryActions.SellAction).Sum(r => r.Package),
                        LastSoldDate = m.BatchHistories.Select(r => r.SoldDate).OrderByDescending(r => r.Value).FirstOrDefault(),
                    }).FirstOrDefault();
                if (sumValues == null)
                {
                    return new MethodResult(false, ErrorMessages.BatchReturnSumNotFound);
                }

                if (returnSumValues != null && returnSumValues.SumPackage.HasValue && returnSumValues.SumQuantity.HasValue)
                {
                    sumValues.SumPackage = sumValues.SumPackage - returnSumValues.SumPackage.Value;
                    sumValues.SumQuantity = sumValues.SumQuantity - returnSumValues.SumQuantity.Value;
                }

                if (quantity > sumValues.SumQuantity || quantity > batch.InitialQuantity)
                {
                    return new MethodResult(false, ErrorMessages.BatchReturnSumQuantityOverflow);
                }
                if (package > sumValues.SumPackage || package > batch.InitialPackage)
                {
                    return new MethodResult(false, ErrorMessages.BatchReturnSumPackageOverflow);
                }
                if (returnDate < sumValues.LastSoldDate)
                {
                    return new MethodResult(false, ErrorMessages.BatchReturnDateOverflow);
                }

                if ((quantity == sumValues.SumQuantity && package < sumValues.SumPackage) || (quantity < sumValues.SumQuantity && package == sumValues.SumPackage))
                {
                    return new MethodResult(false, ErrorMessages.BatchReturnException);
                }


                batch.CurrentPackage = batch.CurrentPackage + package;
                batch.CurrentQuantity = batch.CurrentQuantity + quantity;
                batch.LastUpdateDate = DateTime.Now;

                var newBatchHistory = new BatchHistory()
                {
                    Text = BatchHistoryTexts.GetTextByAction(BatchHistoryActions.ReturnFromCustomerAction, batch.Name),
                    OperationTypeId = BatchHistoryOperations.UsualOperation,
                    ActionTypeId = BatchHistoryActions.ReturnFromCustomerAction,
                    Package = package,
                    Quantity = quantity,
                    LeftPackage = batch.CurrentPackage,
                    LeftQuantity = batch.CurrentQuantity,
                    Remark = remark,
                    InsertDate = DateTime.Now,
                    ReturnDate = returnDate,
                    CustomerId = customerId,
                };

                batch.BatchHistories.Add(newBatchHistory);

                if (batch.StatusId == BatchStatuses.Empty)
                {
                    var newBatchHistoryInfo = new BatchHistory()
                    {
                        Text = $"Партия «{batch.Name}» в наличии на складе.",
                        OperationTypeId = BatchHistoryOperations.InformationOperation,
                        ActionTypeId = BatchHistoryActions.NoneAction,
                        InsertDate = DateTime.Now.AddMinutes(1),
                    };
                    batch.BatchHistories.Add(newBatchHistoryInfo);
                    batch.StatusId = BatchStatuses.Completed;
                }

                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }

        public MethodResult MergeBatch(int id, int package, decimal quantity, int parentBatchId, DateTime date, string remark)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var parentBatch = context.Batches.FirstOrDefault(m => m.Id == parentBatchId && (m.StatusId == BatchStatuses.InManufacturingProcess || m.StatusId == BatchStatuses.Completed));
                var childBatch = context.Batches.FirstOrDefault(m => m.Id == id && m.StatusId == BatchStatuses.Completed);
                if (childBatch == null || parentBatch == null)
                {
                    return new MethodResult(false, ErrorMessages.BatchNotFound);
                }
                if (parentBatch.ChildrenChildBatches.Any(n => n.ChildBatchId == childBatch.Id) || childBatch.ParentChildBatches.Any(m => m.BatchId == parentBatchId) || childBatch.ChildrenChildBatches.Any(m => m.ChildBatchId == parentBatchId) || parentBatch.ParentChildBatches.Any(m => m.BatchId == childBatch.Id))
                {
                    return new MethodResult(false, ErrorMessages.BatchMergeException);
                }
                if (package <= 0 || quantity <= 0)
                {
                    return new MethodResult(false, ErrorMessages.ZeroNotAllowed);
                }
                if ((childBatch.CurrentPackage > childBatch.InitialPackage) || (childBatch.CurrentPackage - package < 0))
                {
                    return new MethodResult(false, ErrorMessages.BatchPackageBelowZero);
                }
                if ((childBatch.CurrentQuantity > childBatch.InitialQuantity) || (childBatch.CurrentQuantity - quantity < 0))
                {
                    return new MethodResult(false, ErrorMessages.BatchQuantityBelowZero);
                }
                if ((childBatch.CurrentPackage - package == 0) && (childBatch.CurrentQuantity - quantity > 0) || (childBatch.CurrentPackage - package > 0) && (childBatch.CurrentQuantity - quantity == 0))
                {
                    return new MethodResult(false, ErrorMessages.BatchAllMustBeNullException);
                }

                childBatch.CurrentPackage = childBatch.CurrentPackage - package;
                childBatch.CurrentQuantity = childBatch.CurrentQuantity - quantity;
                childBatch.LastUpdateDate = DateTime.Now;

                var newBatchHistory = new BatchHistory()
                {
                    Text = BatchHistoryTexts.GetTextByAction(BatchHistoryActions.MergeToOtherAction, childBatch.Name, parentBatch.Name),
                    OperationTypeId = BatchHistoryOperations.UsualOperation,
                    ActionTypeId = BatchHistoryActions.MergeToOtherAction,
                    Package = package,
                    Quantity = quantity,
                    LeftPackage = childBatch.CurrentPackage,
                    LeftQuantity = childBatch.CurrentQuantity,
                    Remark = remark,
                    InsertDate = DateTime.Now,
                    LinkedBatchId = parentBatch.Id,
                };

                childBatch.BatchHistories.Add(newBatchHistory);

                if (childBatch.CurrentPackage == 0 && childBatch.CurrentQuantity == 0)
                {
                    var newBatchHistoryInfo = new BatchHistory()
                    {
                        Text = $"Партия «{childBatch.Name}» полностью израсходована.",
                        OperationTypeId = BatchHistoryOperations.InformationOperation,
                        ActionTypeId = BatchHistoryActions.NoneAction,
                        InsertDate = DateTime.Now.AddMinutes(1),
                    };
                    childBatch.BatchHistories.Add(newBatchHistoryInfo);
                    childBatch.StatusId = BatchStatuses.Empty;
                }

                if (parentBatch.ParentChildBatches.All(m => m.ChildBatchId != childBatch.Id))
                {
                    var batchConnection = new ChildBatch()
                    {
                        ChildBatchId = childBatch.Id,
                        Date = date,
                    };
                    parentBatch.ParentChildBatches.Add(batchConnection);
                }

                parentBatch.CurrentPackage = parentBatch.CurrentPackage + package;
                parentBatch.CurrentQuantity = parentBatch.CurrentQuantity + quantity;
                parentBatch.LastUpdateDate = DateTime.Now;

                var newParentBatchHistory = new BatchHistory()
                {
                    Text = BatchHistoryTexts.GetTextByAction(BatchHistoryActions.MergingFromOtherAction, childBatch.Name, parentBatch.Name),
                    OperationTypeId = BatchHistoryOperations.UsualOperation,
                    ActionTypeId = BatchHistoryActions.MergingFromOtherAction,
                    Remark = remark,
                    InsertDate = DateTime.Now,
                    LinkedBatchId = childBatch.Id,
                    Package = package,
                    Quantity = quantity,
                    LeftPackage = parentBatch.CurrentPackage,
                    LeftQuantity = parentBatch.CurrentQuantity,
                };

                parentBatch.BatchHistories.Add(newParentBatchHistory);

                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }

        public MethodResult ThrowBatch(int id, string remark)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var batch = context.Batches.FirstOrDefault(m => m.Id == id && m.StatusId == BatchStatuses.Completed);
                if (batch == null)
                {
                    return new MethodResult(false, ErrorMessages.BatchNotFound);
                }

                batch.LastUpdateDate = DateTime.Now;

                var newBatchHistory = new BatchHistory()
                {
                    Text = BatchHistoryTexts.GetTextByAction(BatchHistoryActions.UtilizationAction, batch.Name),
                    OperationTypeId = BatchHistoryOperations.UsualOperation,
                    ActionTypeId = BatchHistoryActions.UtilizationAction,
                    Package = batch.CurrentPackage,
                    Quantity = batch.CurrentQuantity,
                    LeftPackage = 0,
                    LeftQuantity = 0,
                    Remark = remark,
                    InsertDate = DateTime.Now,
                };

                batch.CurrentPackage = 0;
                batch.CurrentQuantity = 0;
                batch.BatchHistories.Add(newBatchHistory);

                var newBatchHistoryInfo = new BatchHistory()
                {
                    Text = $"Партия «{batch.Name}» полностью израсходована.",
                    OperationTypeId = BatchHistoryOperations.InformationOperation,
                    ActionTypeId = BatchHistoryActions.NoneAction,
                    InsertDate = DateTime.Now.AddMinutes(1),
                };
                batch.BatchHistories.Add(newBatchHistoryInfo);
                batch.StatusId = BatchStatuses.Empty;

                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }

        public List<BatchExtendedDisplayModel> GetExtendedDisplayBatchList(string query, int? batchId, int? typeId, int? lineId, int? statusId)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var querySearch = context.Batches.OrderByDescending(m => m.InsertDate).AsQueryable();
                if (statusId.HasValue)
                {
                    querySearch = querySearch.Where(m => m.StatusId == statusId.Value);
                }
                if (lineId.HasValue)
                {
                    querySearch = querySearch.Where(m => m.LineId == lineId.Value);
                }
                if (typeId.HasValue)
                {
                    querySearch = querySearch.Where(m => m.TypeId == typeId.Value);
                }
                if (batchId.HasValue)
                {
                    querySearch = querySearch.Where(m => m.Id == batchId.Value);
                }
                if (!query.IsNullOrEmpty())
                {
                    querySearch = querySearch.Where(m => m.Name.Contains(query) || m.BatchLine.Name.Contains(query) || m.BatchType.Name.Contains(query));
                }

                var result = querySearch.Select(m => new BatchExtendedDisplayModel()
                {
                    Id = m.Id,
                    Type = m.BatchType.Name,
                    StatusId = m.StatusId,
                    Line = m.BatchLine.Name,
                    InsertDate = m.InsertDate,
                    Name = m.Name,
                    CreateDate = m.CreateDate,
                    InitialPackage = m.InitialPackage,
                    InitialQuantity = m.InitialQuantity,
                    CompletionDate = m.CompletionDate,
                    CurrentQuantity = m.CurrentQuantity,
                    CurrentPackage = m.CurrentPackage,
                    StatusCssClass = m.BatchStatus.CssClass,
                }).ToList();

                return result;
            }
        }

        public List<BatchActionDisplayModel> GetDataByHistoryList(string query, int? batchId, int? customerId, DateTime? dateFrom, DateTime? dateTo, int actionHistoryType)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var querySearch = context.BatchHistories
                    .Where(m => m.ActionTypeId == actionHistoryType && m.SoldDate.HasValue && m.CustomerId.HasValue && m.Quantity.HasValue && m.Package.HasValue && m.LeftQuantity.HasValue && m.LeftPackage.HasValue)
                    .AsQueryable();

                if (customerId.HasValue)
                {
                    querySearch = querySearch.Where(m => m.CustomerId == customerId);
                }
                if (dateFrom.HasValue)
                {
                    querySearch = querySearch.Where(m => m.SoldDate >= dateFrom);
                }
                if (dateTo.HasValue)
                {
                    querySearch = querySearch.Where(m => m.SoldDate <= dateTo);
                }
                if (batchId.HasValue)
                {
                    querySearch = querySearch.Where(m => m.BatchId == batchId.Value);
                }
                if (!query.IsNullOrEmpty())
                {
                    querySearch = querySearch.Where(m => m.Batch.Name.Contains(query) || m.Customer.Name.Contains(query) || m.Batch.BatchType.Name.Contains(query));
                }

                var result = querySearch.OrderByDescending(m => m.Id).Select(m => new BatchActionDisplayModel()
                {
                    HistoryId = m.Id,
                    BatchId = m.Batch.Id,
                    SoldDate = m.SoldDate.Value,
                    TypeName = m.Batch.BatchType.Name,
                    BatchName = m.Batch.Name,
                    Package = m.Package.Value,
                    Quantity = m.Quantity.Value,
                    LeftQuantity = m.LeftQuantity.Value,
                    LeftPackage = m.LeftPackage.Value,
                    CustomerId = m.CustomerId.Value,
                    CustomerName = m.Customer.Name,
                }).ToList();

                return result;
            }
        }

        public DataTable GetBatchWarehouseExcel()
        {
            using (var data = this.ExecuteProcedure("Excel_GetBatchWarehouseExcel", true))
            {
                if (data != null && (data.Rows.Count > 0))
                {
                    return data;
                }
            }

            return null;
        }

        public DataTable DownloadSellingsExcel()
        {
            using (var data = this.ExecuteProcedure("Excel_GetDownloadSellingsExcel", true))
            {
                if (data != null && (data.Rows.Count > 0))
                {
                    return data;
                }
            }

            return null;
        }

        public BatchEditModel GetBatchForEditHistory(int id, int action, int historyId)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var result = context.BatchHistories
                    .Where(m => m.BatchId == id && m.Id == historyId)
                    .Select(m => new BatchEditModel()
                    {
                        Id = m.Id,
                        CustomerId = action == BatchHistoryActions.SellAction || action == BatchHistoryActions.ReturnFromCustomerAction ? m.CustomerId : null,
                        Package = m.Package.Value,
                        Quantity = m.Quantity.Value,
                        ParentBatchId = action == BatchHistoryActions.MergeToOtherAction ? m.LinkedBatchId : null,
                        Name = m.Batch.Name,
                        ActionName = m.BatchHistoryActionType.Name,
                    }).FirstOrDefault();

                return result;
            }
        }

        public MethodResult EditSellAction(int batchId, int historyId, decimal quantity, int package, int customerId)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var batchHistory = context.BatchHistories.FirstOrDefault(m => m.Id == historyId && m.ActionTypeId == BatchHistoryActions.SellAction && m.BatchId == batchId);
                if (batchHistory == null)
                {
                    return new MethodResult(false, ErrorMessages.BatchNotFound);
                }

                batchHistory.Batch.LastUpdateDate = DateTime.Now;

                if (batchHistory.CustomerId != customerId)
                {
                    batchHistory.CustomerId = customerId;
                }
                if (batchHistory.Quantity != quantity)
                {
                    if (quantity < batchHistory.Quantity)
                    {
                        batchHistory.Batch.CurrentQuantity = batchHistory.Batch.CurrentQuantity + (batchHistory.Quantity - quantity);
                        var batchHistoriesForQuantityChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id >= historyId).ToList();
                        batchHistoriesForQuantityChange.ForEach(m =>
                        {
                            m.LeftQuantity = m.LeftQuantity + (batchHistory.Quantity - quantity);
                        });
                    }
                    else if (quantity > batchHistory.Quantity)
                    {
                        if ((batchHistory.Batch.CurrentQuantity - (quantity - batchHistory.Quantity) < 0) || (batchHistory.LeftQuantity - (quantity - batchHistory.Quantity) < 0))
                        {
                            return new MethodResult(false, ErrorMessages.BatchQuantityBelowZero);
                        }

                        batchHistory.Batch.CurrentQuantity = batchHistory.Batch.CurrentQuantity - (quantity - batchHistory.Quantity);
                        var batchHistoriesForQuantityChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id >= historyId).ToList();
                        batchHistoriesForQuantityChange.ForEach(m =>
                        {
                            m.LeftQuantity = m.LeftQuantity - (quantity - batchHistory.Quantity);
                        });
                    }
                    batchHistory.Quantity = quantity;
                }

                if (batchHistory.Package != package)
                {
                    if (package < batchHistory.Package)
                    {
                        batchHistory.Batch.CurrentPackage = batchHistory.Batch.CurrentPackage + (batchHistory.Package - package);
                        var batchHistoriesForPackageChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id >= historyId).ToList();
                        batchHistoriesForPackageChange.ForEach(m =>
                        {
                            m.LeftPackage = m.LeftPackage + (batchHistory.Package - package);
                        });
                    }
                    else if (package > batchHistory.Package)
                    {
                        if ((batchHistory.Batch.CurrentPackage - (package - batchHistory.Package) < 0) || (batchHistory.LeftPackage - (package - batchHistory.Package) < 0))
                        {
                            return new MethodResult(false, ErrorMessages.BatchPackageBelowZero);
                        }

                        batchHistory.Batch.CurrentPackage = batchHistory.Batch.CurrentPackage - (package - batchHistory.Package);
                        var batchHistoriesForPackageChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id >= historyId).ToList();
                        batchHistoriesForPackageChange.ForEach(m =>
                        {
                            m.LeftPackage = m.LeftPackage - (package - batchHistory.Package);
                        });
                    }
                    batchHistory.Package = package;
                }

                //if ((batchHistory.Batch.CurrentPackage > 0 && batchHistory.Batch.CurrentQuantity == 0) || (batchHistory.Batch.CurrentPackage == 0 && batchHistory.Batch.CurrentQuantity > 0))
                //{
                //    return new MethodResult(false, ErrorMessages.BatchAllMustBeZeroOrAboveException);
                //}

                if (batchHistory.Batch.CurrentPackage == 0 && batchHistory.Batch.CurrentQuantity == 0 && batchHistory.Batch.StatusId == BatchStatuses.Completed)
                {
                    batchHistory.Batch.StatusId = BatchStatuses.Empty;
                }
                else if (batchHistory.Batch.CurrentPackage > 0 && batchHistory.Batch.CurrentQuantity > 0 && batchHistory.Batch.StatusId == BatchStatuses.Empty)
                {
                    batchHistory.Batch.StatusId = BatchStatuses.Completed;
                }

                batchHistory.OperationTypeId = BatchHistoryOperations.EditOperation;

                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }

        public MethodResult EditReturnFromCustomerAction(int batchId, int historyId, decimal quantity, int package, int customerId)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var batchHistory = context.BatchHistories.FirstOrDefault(m => m.Id == historyId && m.ActionTypeId == BatchHistoryActions.ReturnFromCustomerAction && m.BatchId == batchId);
                if (batchHistory == null)
                {
                    return new MethodResult(false, ErrorMessages.BatchNotFound);
                }

                batchHistory.Batch.LastUpdateDate = DateTime.Now;

                if (batchHistory.CustomerId != customerId)
                {
                    batchHistory.CustomerId = customerId;
                }
                if (batchHistory.Quantity != quantity)
                {
                    if (quantity > batchHistory.Quantity)
                    {
                        batchHistory.Batch.CurrentQuantity = batchHistory.Batch.CurrentQuantity + (quantity - batchHistory.Quantity);
                        var batchHistoriesForQuantityChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id >= historyId).ToList();
                        batchHistoriesForQuantityChange.ForEach(m =>
                        {
                            m.LeftQuantity = m.LeftQuantity + (quantity - batchHistory.Quantity);
                        });
                    }
                    else if (quantity < batchHistory.Quantity)
                    {
                        if ((batchHistory.Batch.CurrentQuantity - (batchHistory.Quantity - quantity) < 0) || (batchHistory.LeftQuantity - (batchHistory.Quantity - quantity) < 0))
                        {
                            return new MethodResult(false, ErrorMessages.BatchQuantityBelowZero);
                        }

                        batchHistory.Batch.CurrentQuantity = batchHistory.Batch.CurrentQuantity - (batchHistory.Quantity - quantity);
                        var batchHistoriesForQuantityChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id >= historyId).ToList();
                        batchHistoriesForQuantityChange.ForEach(m =>
                        {
                            m.LeftQuantity = m.LeftQuantity - (batchHistory.Quantity - quantity);
                        });
                    }
                    batchHistory.Quantity = quantity;
                }

                if (batchHistory.Package != package)
                {
                    if (package > batchHistory.Package)
                    {
                        batchHistory.Batch.CurrentPackage = batchHistory.Batch.CurrentPackage + (package - batchHistory.Package);
                        var batchHistoriesForPackageChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id >= historyId).ToList();
                        batchHistoriesForPackageChange.ForEach(m =>
                        {
                            m.LeftPackage = m.LeftPackage + (package - batchHistory.Package);
                        });
                    }
                    else if (package < batchHistory.Package)
                    {
                        if ((batchHistory.Batch.CurrentPackage - (batchHistory.Package - package) < 0) || (batchHistory.LeftPackage - (batchHistory.Package - package) < 0))
                        {
                            return new MethodResult(false, ErrorMessages.BatchPackageBelowZero);
                        }

                        batchHistory.Batch.CurrentPackage = batchHistory.Batch.CurrentPackage - (batchHistory.Package - package);
                        var batchHistoriesForPackageChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id >= historyId).ToList();
                        batchHistoriesForPackageChange.ForEach(m =>
                        {
                            m.LeftPackage = m.LeftPackage - (batchHistory.Package - package);
                        });
                    }
                    batchHistory.Package = package;
                }

                //if ((batchHistory.Batch.CurrentPackage > 0 && batchHistory.Batch.CurrentQuantity == 0) || (batchHistory.Batch.CurrentPackage == 0 && batchHistory.Batch.CurrentQuantity > 0))
                //{
                //    return new MethodResult(false, ErrorMessages.BatchAllMustBeZeroOrAboveException);
                //}

                if (batchHistory.Batch.CurrentPackage == 0 && batchHistory.Batch.CurrentQuantity == 0 && batchHistory.Batch.StatusId == BatchStatuses.Completed)
                {
                    batchHistory.Batch.StatusId = BatchStatuses.Empty;
                    var newBatchHistoryInfo = new BatchHistory()
                    {
                        Text = $"Партия «{batchHistory.Batch.Name}» полностью израсходована.",
                        OperationTypeId = BatchHistoryOperations.InformationOperation,
                        ActionTypeId = BatchHistoryActions.NoneAction,
                        InsertDate = DateTime.Now.AddMinutes(1),
                    };
                    batchHistory.Batch.BatchHistories.Add(newBatchHistoryInfo);
                }
                else if (batchHistory.Batch.CurrentPackage > 0 && batchHistory.Batch.CurrentQuantity > 0 && batchHistory.Batch.StatusId == BatchStatuses.Empty)
                {
                    batchHistory.Batch.StatusId = BatchStatuses.Completed;
                    var newBatchHistoryInfo = new BatchHistory()
                    {
                        Text = $"Партия «{batchHistory.Batch.Name}» в наличии на складе.",
                        OperationTypeId = BatchHistoryOperations.InformationOperation,
                        ActionTypeId = BatchHistoryActions.NoneAction,
                        InsertDate = DateTime.Now.AddMinutes(1),
                    };
                    batchHistory.Batch.BatchHistories.Add(newBatchHistoryInfo);
                }

                batchHistory.OperationTypeId = BatchHistoryOperations.EditOperation;

                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }

        public MethodResult EditMergeToOtherAction(int batchId, int historyId, decimal quantity, int package, int parentBatchId)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var batchHistory = context.BatchHistories.FirstOrDefault(m => m.Id == historyId && m.ActionTypeId == BatchHistoryActions.MergeToOtherAction && m.BatchId == batchId);
                if (batchHistory == null)
                {
                    return new MethodResult(false, ErrorMessages.BatchNotFound);
                }

                batchHistory.Batch.LastUpdateDate = DateTime.Now;

                if (batchHistory.LinkedBatchId != parentBatchId)
                {
                    var oldBatchHistoryRecord = context.BatchHistories.FirstOrDefault(m => m.ActionTypeId == BatchHistoryActions.MergingFromOtherAction && m.BatchId == batchHistory.LinkedBatchId && m.Package == batchHistory.Package && m.Quantity == batchHistory.Quantity);
                    if (oldBatchHistoryRecord == null)
                    {
                        return new MethodResult(false, ErrorMessages.BatchNotFound);
                    }
                    if (!oldBatchHistoryRecord.Batch.BatchHistories.Any(m => m.LinkedBatchId.HasValue && m.LinkedBatchId == batchHistory.LinkedBatchId && m.Id != historyId))
                    {
                        var oldChildBatch = context.ChildBatches.FirstOrDefault(m => m.ChildBatchId == batchHistory.Batch.Id && m.BatchId == oldBatchHistoryRecord.Batch.Id);
                        context.ChildBatches.DeleteOnSubmit(oldChildBatch);
                    }

                    var newBatchParent = context.Batches.FirstOrDefault(m => m.Id == parentBatchId);
                    if (newBatchParent == null)
                    {
                        return new MethodResult(false, ErrorMessages.BatchNotFound);
                    }
                    batchHistory.LinkedBatchId = parentBatchId;
                    if (newBatchParent.ParentChildBatches.All(m => m.ChildBatchId != batchHistory.Batch.Id))
                    {
                        var batchConnection = new ChildBatch()
                        {
                            ChildBatchId = batchHistory.Batch.Id,
                            Date = DateTime.Now,
                        };
                        newBatchParent.ParentChildBatches.Add(batchConnection);
                    }
                    var newParentBatchHistory = new BatchHistory()
                    {
                        Text = BatchHistoryTexts.GetTextByAction(BatchHistoryActions.MergingFromOtherAction, batchHistory.Batch.Name, newBatchParent.Name),
                        OperationTypeId = BatchHistoryOperations.UsualOperation,
                        ActionTypeId = BatchHistoryActions.MergingFromOtherAction,
                        Remark = oldBatchHistoryRecord.Remark,
                        InsertDate = DateTime.Now,
                        LinkedBatchId = batchHistory.Batch.Id,
                    };
                    newBatchParent.BatchHistories.Add(newParentBatchHistory);
                    context.BatchHistories.DeleteOnSubmit(oldBatchHistoryRecord);
                }
                if (batchHistory.Quantity != quantity)
                {
                    if (quantity < batchHistory.Quantity)
                    {
                        batchHistory.Batch.CurrentQuantity = batchHistory.Batch.CurrentQuantity + (batchHistory.Quantity - quantity);
                        var batchHistoriesForQuantityChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id >= historyId).ToList();
                        batchHistoriesForQuantityChange.ForEach(m =>
                        {
                            m.LeftQuantity = m.LeftQuantity + (batchHistory.Quantity - quantity);
                        });
                    }
                    else if (quantity > batchHistory.Quantity)
                    {
                        if ((batchHistory.Batch.CurrentQuantity - (quantity - batchHistory.Quantity) < 0) || (batchHistory.LeftQuantity - (quantity - batchHistory.Quantity) < 0))
                        {
                            return new MethodResult(false, ErrorMessages.BatchQuantityBelowZero);
                        }

                        batchHistory.Batch.CurrentQuantity = batchHistory.Batch.CurrentQuantity - (quantity - batchHistory.Quantity);
                        var batchHistoriesForQuantityChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id >= historyId).ToList();
                        batchHistoriesForQuantityChange.ForEach(m =>
                        {
                            m.LeftQuantity = m.LeftQuantity - (quantity - batchHistory.Quantity);
                        });
                    }
                    batchHistory.Quantity = quantity;
                }

                if (batchHistory.Package != package)
                {
                    if (package < batchHistory.Package)
                    {
                        batchHistory.Batch.CurrentPackage = batchHistory.Batch.CurrentPackage + (batchHistory.Package - package);
                        var batchHistoriesForPackageChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id >= historyId).ToList();
                        batchHistoriesForPackageChange.ForEach(m =>
                        {
                            m.LeftPackage = m.LeftPackage + (batchHistory.Package - package);
                        });
                    }
                    else if (package > batchHistory.Package)
                    {
                        if ((batchHistory.Batch.CurrentPackage - (package - batchHistory.Package) < 0) || (batchHistory.LeftPackage - (package - batchHistory.Package) < 0))
                        {
                            return new MethodResult(false, ErrorMessages.BatchPackageBelowZero);
                        }

                        batchHistory.Batch.CurrentPackage = batchHistory.Batch.CurrentPackage - (package - batchHistory.Package);
                        var batchHistoriesForPackageChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id >= historyId).ToList();
                        batchHistoriesForPackageChange.ForEach(m =>
                        {
                            m.LeftPackage = m.LeftPackage - (package - batchHistory.Package);
                        });
                    }
                    batchHistory.Package = package;
                }

                //if ((batchHistory.Batch.CurrentPackage > 0 && batchHistory.Batch.CurrentQuantity == 0) || (batchHistory.Batch.CurrentPackage == 0 && batchHistory.Batch.CurrentQuantity > 0))
                //{
                //    return new MethodResult(false, ErrorMessages.BatchAllMustBeZeroOrAboveException);
                //}

                if (batchHistory.Batch.CurrentPackage == 0 && batchHistory.Batch.CurrentQuantity == 0 && batchHistory.Batch.StatusId == BatchStatuses.Completed)
                {
                    batchHistory.Batch.StatusId = BatchStatuses.Empty;
                }
                else if (batchHistory.Batch.CurrentPackage > 0 && batchHistory.Batch.CurrentQuantity > 0 && batchHistory.Batch.StatusId == BatchStatuses.Empty)
                {
                    batchHistory.Batch.StatusId = BatchStatuses.Completed;
                }

                batchHistory.OperationTypeId = BatchHistoryOperations.EditOperation;

                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }

        public MethodResult EditCompleteAction(int batchId, int historyId, decimal quantity, int package)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var batchHistory = context.BatchHistories.FirstOrDefault(m => m.Id == historyId && m.ActionTypeId == BatchHistoryActions.CompleteAction && m.BatchId == batchId);
                if (batchHistory == null)
                {
                    return new MethodResult(false, ErrorMessages.BatchNotFound);
                }

                batchHistory.Batch.LastUpdateDate = DateTime.Now;

                if (batchHistory.Quantity != quantity)
                {
                    if (quantity > batchHistory.Quantity)
                    {
                        batchHistory.Batch.CurrentQuantity = batchHistory.Batch.CurrentQuantity + (quantity - batchHistory.Quantity);
                        batchHistory.Batch.InitialQuantity = batchHistory.Batch.InitialQuantity + (quantity - batchHistory.Quantity);
                        var batchHistoriesForQuantityChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id >= historyId).ToList();
                        batchHistoriesForQuantityChange.ForEach(m =>
                        {
                            m.LeftQuantity = m.LeftQuantity + (quantity - batchHistory.Quantity);
                        });
                    }
                    else if (quantity < batchHistory.Quantity)
                    {
                        if ((batchHistory.Batch.CurrentQuantity - (batchHistory.Quantity - quantity) < 0) || (batchHistory.LeftQuantity - (batchHistory.Quantity - quantity) < 0))
                        {
                            return new MethodResult(false, ErrorMessages.BatchQuantityBelowZero);
                        }

                        batchHistory.Batch.CurrentQuantity = batchHistory.Batch.CurrentQuantity - (batchHistory.Quantity - quantity);
                        batchHistory.Batch.InitialQuantity = batchHistory.Batch.InitialQuantity - (batchHistory.Quantity - quantity);
                        var batchHistoriesForQuantityChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id >= historyId).ToList();
                        batchHistoriesForQuantityChange.ForEach(m =>
                        {
                            m.LeftQuantity = m.LeftQuantity - (batchHistory.Quantity - quantity);
                        });
                    }
                    batchHistory.Quantity = quantity;
                }

                if (batchHistory.Package != package)
                {
                    if (package > batchHistory.Package)
                    {
                        batchHistory.Batch.CurrentPackage = batchHistory.Batch.CurrentPackage + (package - batchHistory.Package);
                        batchHistory.Batch.InitialPackage = batchHistory.Batch.InitialPackage + (package - batchHistory.Package);
                        var batchHistoriesForPackageChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id >= historyId).ToList();
                        batchHistoriesForPackageChange.ForEach(m =>
                        {
                            m.LeftPackage = m.LeftPackage + (package - batchHistory.Package);
                        });
                    }
                    else if (package < batchHistory.Package)
                    {
                        if ((batchHistory.Batch.CurrentPackage - (batchHistory.Package - package) < 0) || (batchHistory.LeftPackage - (batchHistory.Package - package) < 0))
                        {
                            return new MethodResult(false, ErrorMessages.BatchPackageBelowZero);
                        }

                        batchHistory.Batch.CurrentPackage = batchHistory.Batch.CurrentPackage - (batchHistory.Package - package);
                        batchHistory.Batch.InitialPackage = batchHistory.Batch.InitialPackage - (batchHistory.Package - package);
                        var batchHistoriesForPackageChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id >= historyId).ToList();
                        batchHistoriesForPackageChange.ForEach(m =>
                        {
                            m.LeftPackage = m.LeftPackage - (batchHistory.Package - package);
                        });
                    }
                    batchHistory.Package = package;
                }

                //if ((batchHistory.Batch.CurrentPackage > 0 && batchHistory.Batch.CurrentQuantity == 0) || (batchHistory.Batch.CurrentPackage == 0 && batchHistory.Batch.CurrentQuantity > 0))
                //{
                //    return new MethodResult(false, ErrorMessages.BatchAllMustBeZeroOrAboveException);
                //}

                if (batchHistory.Batch.CurrentPackage == 0 && batchHistory.Batch.CurrentQuantity == 0 && batchHistory.Batch.StatusId == BatchStatuses.Completed)
                {
                    batchHistory.Batch.StatusId = BatchStatuses.Empty;
                    var newBatchHistoryInfo = new BatchHistory()
                    {
                        Text = $"Партия «{batchHistory.Batch.Name}» полностью израсходована.",
                        OperationTypeId = BatchHistoryOperations.InformationOperation,
                        ActionTypeId = BatchHistoryActions.NoneAction,
                        InsertDate = DateTime.Now.AddMinutes(1),
                    };
                    batchHistory.Batch.BatchHistories.Add(newBatchHistoryInfo);
                }
                else if (batchHistory.Batch.CurrentPackage > 0 && batchHistory.Batch.CurrentQuantity > 0 && batchHistory.Batch.StatusId == BatchStatuses.Empty)
                {
                    batchHistory.Batch.StatusId = BatchStatuses.Completed;
                    var newBatchHistoryInfo = new BatchHistory()
                    {
                        Text = $"Партия «{batchHistory.Batch.Name}» в наличии на складе.",
                        OperationTypeId = BatchHistoryOperations.InformationOperation,
                        ActionTypeId = BatchHistoryActions.NoneAction,
                        InsertDate = DateTime.Now.AddMinutes(1),
                    };
                    batchHistory.Batch.BatchHistories.Add(newBatchHistoryInfo);
                }

                batchHistory.OperationTypeId = BatchHistoryOperations.EditOperation;

                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }

        public MethodResult DeleteSellAction(int batchId, int historyId)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var batchHistory = context.BatchHistories.FirstOrDefault(m => m.Id == historyId && m.ActionTypeId == BatchHistoryActions.SellAction && m.BatchId == batchId);
                var batch = context.Batches.FirstOrDefault(m => m.Id == batchId);
                if (batchHistory == null || batch == null)
                {
                    return new MethodResult(false, ErrorMessages.BatchNotFound);
                }

                batch.CurrentQuantity = batch.CurrentQuantity + batchHistory.Quantity;
                batch.CurrentPackage = batch.CurrentPackage + batchHistory.Package;

                var batchHistoriesForQuantityChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id > historyId).ToList();
                batchHistoriesForQuantityChange.ForEach(m =>
                {
                    m.LeftQuantity = m.LeftQuantity + batchHistory.Quantity;
                    m.LeftPackage = m.LeftPackage + batchHistory.Package;
                });

                if (batch.CurrentQuantity == 0 && batch.CurrentPackage == 0)
                {
                    batch.StatusId = BatchStatuses.Empty;
                }
                else
                {
                    batch.StatusId = BatchStatuses.Completed;
                }

                context.BatchHistories.DeleteOnSubmit(batchHistory);
                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }

        public MethodResult DeleteReturnFromCustomerAction(int batchId, int historyId)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var batchHistory = context.BatchHistories.FirstOrDefault(m => m.Id == historyId && m.ActionTypeId == BatchHistoryActions.ReturnFromCustomerAction && m.BatchId == batchId);
                var batch = context.Batches.FirstOrDefault(m => m.Id == batchId);
                if (batchHistory == null || batch == null)
                {
                    return new MethodResult(false, ErrorMessages.BatchNotFound);
                }

                if (batch.CurrentQuantity - batchHistory.Quantity < 0)
                {
                    return new MethodResult(false, ErrorMessages.BatchQuantityBelowZero);
                }

                if (batch.CurrentPackage - batchHistory.Package < 0)
                {
                    return new MethodResult(false, ErrorMessages.BatchPackageBelowZero);
                }
                batch.CurrentQuantity = batch.CurrentQuantity - batchHistory.Quantity;
                batch.CurrentPackage = batch.CurrentPackage - batchHistory.Package;

                var batchHistoriesForQuantityChange = context.BatchHistories.Where(m => m.BatchId == batchId && m.Id > historyId).ToList();
                batchHistoriesForQuantityChange.ForEach(m =>
                {
                    m.LeftQuantity = m.LeftQuantity - batchHistory.Quantity;
                    m.LeftPackage = m.LeftPackage - batchHistory.Package;
                });

                if (batch.CurrentQuantity == 0 && batch.CurrentPackage == 0)
                {
                    batch.StatusId = BatchStatuses.Empty;
                }
                else
                {
                    batch.StatusId = BatchStatuses.Completed;
                }

                context.BatchHistories.DeleteOnSubmit(batchHistory);
                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }

        public MethodResult DeleteCompleteAction(int batchId, int historyId)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var batchHistory = context.BatchHistories.FirstOrDefault(m => m.Id == historyId && m.ActionTypeId == BatchHistoryActions.CompleteAction && m.BatchId == batchId);
                var batch = context.Batches.FirstOrDefault(m => m.Id == batchId);
                if (batchHistory == null || batch == null)
                {
                    return new MethodResult(false, ErrorMessages.BatchNotFound);
                }

                batch.CurrentQuantity = (int?)null;
                batch.CurrentPackage = (int?)null;

                batch.InitialQuantity = (int?)null;
                batch.InitialPackage = (int?)null;

                var batchHistoriesAny = context.BatchHistories.Any(m => m.BatchId == batchId && m.Id > historyId);
                if (batchHistoriesAny)
                {
                    return new MethodResult(false, ErrorMessages.BatchNotFound);
                }

                batch.StatusId = BatchStatuses.InManufacturingProcess;

                context.BatchHistories.DeleteOnSubmit(batchHistory);
                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult(true);
            }
        }
    }
}