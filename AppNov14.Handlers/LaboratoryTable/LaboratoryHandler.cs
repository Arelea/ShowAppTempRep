using AppNov14.Handlers.Interfaces;
using AppNov14.Helpers;
using AppNov14.Models.Base;
using AppNov14.Models.ManufacturingTable;
using AppNov14.Models.Warehouse;
using AppNov14.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace AppNov14.Handlers.LaboratoryTable
{
    public class LaboratoryHandler : BaseDataHandler, ILaboratoryHandler
    {
        private readonly ILaboratoryRepository LaboratoryRepository;

        private readonly IWarehouseRepository WarehouseRepository;

        private readonly IBaseDataRepository BaseDataRepository;

        private readonly IBatchRepository ConsignmentRepository;

        public LaboratoryHandler(IBatchRepository consignmentRepository, ILaboratoryRepository laboratoryRepository, IWarehouseRepository warehouseRepository, IBaseDataRepository baseDataRepository) : base(baseDataRepository)
        {
            LaboratoryRepository = laboratoryRepository;
            WarehouseRepository = warehouseRepository;
            BaseDataRepository = baseDataRepository;
            ConsignmentRepository = consignmentRepository;
        }

        public MethodResult ReplenishWarehouse(ManufacturingTableWriteModel model)
        {
            var warehouseId = this.WarehouseRepository.GetId(model.Type, model.SubType, model.Provider, model.Manufacturer, ActionModes.Laboratory);
            if (warehouseId == null)
            {
                return new MethodResult()
                {
                    IsSuccess = false,
                    Message = ErrorMessages.WarehouseIdNotFound,
                };
            }

            var modelToWrite = new ManufacturngTableFullModel()
            {
                Quantity = model.Quantity,
                AutoDate = model.AutoDate,
                DocDate = model.DocDate,
                WarehouseId = warehouseId.Value,
                Indexation = model.Indexation,
                IpAddress = model.IpAddress,
                Employee = model.Employee,
                Remarks = model.Remarks,
                OperationType = model.OperationType,
                Document = model.Document,
                DocumentNumber = model.DocumentNumber,
            };

            var result = this.LaboratoryRepository.ReplenishWarehouse(modelToWrite);
            if (result.IsSuccess)
            {
                return new MethodResult()
                {
                    IsSuccess = result.IsSuccess,
                };
            }

            return new MethodResult()
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
            };
        }

        public MethodResult WriteOffWarehouse(ManufacturingTableWriteModel model)
        {
            var warehouseId = this.WarehouseRepository.GetId(model.Type, model.SubType, model.Provider, model.Manufacturer, ActionModes.Laboratory);
            if (warehouseId == null)
            {
                return new MethodResult()
                {
                    IsSuccess = false,
                    Message = ErrorMessages.WarehouseIdNotFound,
                };
            }
            var modelToWrite = new ManufacturngTableFullModel();

            var indexId = this.WarehouseRepository.GetLabIndexId(model.Indexation, warehouseId.Value);
            if (indexId == null)
            {
                return new MethodResult()
                {
                    IsSuccess = false,
                    Message = ErrorMessages.IndexIdNotFound,
                };
            }

            modelToWrite = new ManufacturngTableFullModel()
            {
                Quantity = model.Quantity,
                AutoDate = model.AutoDate,
                DocDate = model.DocDate,
                WarehouseId = warehouseId.Value,
                IndexId = indexId.Value,
                IpAddress = model.IpAddress,
                Employee = model.Employee,
                Remarks = model.Remarks,
                OperationType = model.OperationType,
                Document = model.Document,
                DocumentNumber = model.DocumentNumber,
                Line = model.Line,
            };

            var result = this.LaboratoryRepository.WriteOffWarehouse(modelToWrite);
            if (result.IsSuccess)
            {
                return new MethodResult()
                {
                    IsSuccess = result.IsSuccess,
                };
            }

            return new MethodResult()
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
            };
        }

        public List<ManufacturingTableWriteModel> GetLastAdding()
        {
            return this.LaboratoryRepository.GetLastAdding();
        }

        public List<ManufacturingTableWriteModel> GetItemsByStartAndEndDate(DateTime dateStart, DateTime dateEnd)
        {
            return this.LaboratoryRepository.GetItemsByStartAndEndDate(dateStart, dateEnd);
        }

        public ManufacturingTableWriteModel GetById(int id)
        {
            return this.LaboratoryRepository.GetById(id);
        }

        public MethodResult Edit(ManufacturingTableWriteModel model)
        {
            var warehouseIdNew = this.WarehouseRepository.GetId(model.Type, model.SubType, model.Provider, model.Manufacturer, ActionModes.Laboratory);
            var warehouseIdOld = this.WarehouseRepository.GetIdById(model.Id, ActionModes.Laboratory);
            if (warehouseIdNew == null || warehouseIdOld == null)
            {
                return new MethodResult()
                {
                    IsSuccess = false,
                    Message = ErrorMessages.WarehouseIdNotFound,
                };
            }

            int? indexId = 0;
            if (model.OperationType == RawActionTypes.WriteOff)
            {
                indexId = this.WarehouseRepository.GetLabIndexId(model.Indexation, warehouseIdNew.Value);
                if (indexId == null)
                {
                    return new MethodResult()
                    {
                        IsSuccess = false,
                        Message = ErrorMessages.IndexIdNotFound,
                    };
                }
            }

            var result = new MethodResult();
            var newRecord = new ManufacturngTableFullModel()
            {
                Id = model.Id,
                Quantity = model.Quantity,
                DocDate = model.DocDate,
                WarehouseId = warehouseIdNew.Value,
                Indexation = model.Indexation,
                Employee = model.Employee,
                Remarks = model.Remarks,
                OperationType = model.OperationType,
                Document = model.Document,
                DocumentNumber = model.DocumentNumber,
                IndexId = indexId.Value,
            };

            if (model.OperationType == RawActionTypes.Replenishment)
            {
                result = this.LaboratoryRepository.EditReplenish(newRecord);
            }
            else if (model.OperationType == RawActionTypes.WriteOff)
            {
                result = this.LaboratoryRepository.EditWriteOff(newRecord);
            }

            if (result.IsSuccess)
            {
                return new MethodResult()
                {
                    IsSuccess = result.IsSuccess,
                };
            }

            return new MethodResult()
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
            };
        }

        public MethodResult Delete(int id)
        {
            var warehouseIdOld = this.WarehouseRepository.GetIdById(id, ActionModes.Laboratory);
            if (warehouseIdOld == null)
            {
                return new MethodResult()
                {
                    IsSuccess = false,
                    Message = ErrorMessages.WarehouseIdNotFound,
                };
            }

            var result = this.LaboratoryRepository.Delete(id);
            if (result.IsSuccess)
            {
                return new MethodResult()
                {
                    IsSuccess = result.IsSuccess,
                };
            }

            return new MethodResult()
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
            };
        }

        public int GetRecordCountByDate(DateTime dateStart, DateTime dateEnd)
        {
            return this.LaboratoryRepository.GetRecordCountByDate(dateStart, dateEnd);
        }

        public DataTable GetDataTableForExcel(DateTime dateStart, DateTime dateEnd)
        {
            return this.LaboratoryRepository.GetDataTableForExcel(dateStart, dateEnd);
        }

        public List<ManufacturingTableWriteModel> GetItemsByConsignmentNumber(string number)
        {
            return this.LaboratoryRepository.GetItemsByConsignmentNumber(number);
        }

        public List<IndexDataModel> GetIndexationByWarehouse(int mode, string type, string subType, string provider, string manufacturer)
        {

            var warehouseId = this.WarehouseRepository.GetId(type, subType, provider, manufacturer, mode);
            if (warehouseId.HasValue)
            {
                return this.WarehouseRepository.GetLabIndexationByWarehouse(warehouseId.Value);
            }

            return new List<IndexDataModel>();
        }
    }
}
