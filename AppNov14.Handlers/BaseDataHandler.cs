using AppNov14.Handlers.Interfaces;
using AppNov14.Models.Warehouse;
using AppNov14.Repositories.Interfaces;
using System;
using System.Collections.Generic;

namespace AppNov14.Handlers
{
    public class BaseDataHandler : IBaseDataHandler
    {
        private readonly IBaseDataRepository BaseDataRepository;

        public BaseDataHandler(IBaseDataRepository baseDataRepository)
        {
            BaseDataRepository = baseDataRepository;
        }

        public List<string> GetBatchTypeList()
        {
            return this.BaseDataRepository.GetBatchTypeList();
        }

        /// <summary>
        /// Возвращает уникальные значения индекса, в зависимости от типа, наименования, поставщика ипроизводителя, и где остаток не меньше нуля
        /// </summary>
        public Dictionary<int, string> GetDistinctFilledIndexationList(string type, string subType, string provider, string manufacturer)
        {
            return this.BaseDataRepository.GetDistinctFilledIndexationList( type, subType, provider, manufacturer);
        }

        /// <summary>
        /// Возвращает уникальные значения индекса лаборатории, в зависимости от типа, наименования, поставщика ипроизводителя, и где остаток не меньше нуля
        /// </summary>
        public Dictionary<string, string> GetDistinctFilledIndexationLabList(string type, string subType, string provider, string manufacturer)
        {
            return this.BaseDataRepository.GetDistinctFilledIndexationLabList(type, subType, provider, manufacturer);
        }

        /// <summary>
        /// Возвращает уникальные значения типа материала в виде листа
        /// </summary>
        public List<string> GetDistinctTypeList(int? mode)
        {
            var filter = new WarehouseFilterModel()
            {
                Mode = mode ?? default(int),
                IsFilled = false,
            };

            return this.BaseDataRepository.GetDistinctTypeList(filter);
        }

        /// <summary>
        /// Возвращает уникальные значения наименования типа материала в виде листа, зависит от типа
        /// </summary>
        public List<string> GetDistinctSubTypeList(int? mode, string type)
        {
            var filter = new WarehouseFilterModel()
            {
                Mode = mode ?? default(int),
                Type = type,
                IsFilled = false,
            };

            return this.BaseDataRepository.GetDistinctSubTypeList(filter);
        }

        /// <summary>
        /// Возвращает уникальные значения наименования типа материала в виде листа
        /// </summary>
        public List<string> GetDistinctSubTypeList(int? mode)
        {
            var filter = new WarehouseFilterModel()
            {
                Mode = mode ?? default(int),
                IsFilled = false,
            };

            return this.BaseDataRepository.GetDistinctSubTypeList(filter);
        }

        /// <summary>
        /// Возвращает уникальные значения поставщика в виде листа
        /// </summary>
        public List<string> GetDistinctProviderList(int? mode, bool isFilled = false)
        {
            var filter = new WarehouseFilterModel()
            {
                Mode = mode ?? default(int),
                IsFilled = isFilled,
            };

            return this.BaseDataRepository.GetDistinctProviderList(filter);
        }

        public List<string> GetDistinctProviderList(int? mode, string type, string subType, bool isFilled = false)
        {
            var filter = new WarehouseFilterModel()
            {
                Mode = mode ?? default(int),
                Type = type,
                SubType = subType,
                IsFilled = isFilled,
            };

            return this.BaseDataRepository.GetDistinctProviderList(filter);
        }

        /// <summary>
        /// Возвращает уникальные значения производителя в виде листа
        /// </summary>
        public List<string> GetDistinctManufacturerList(int? mode, bool isFilled = false)
        {
            var filter = new WarehouseFilterModel()
            {
                Mode = mode ?? default(int),
                IsFilled = isFilled,
            };

            return this.BaseDataRepository.GetDistinctManufacturerList(filter);
        }

        public List<string> GetDistinctManufacturerList(int? mode, string type, string subType, string provider, bool isFilled = false)
        {
            var filter = new WarehouseFilterModel()
            {
                Mode = mode ?? default(int),
                Type = type,
                SubType = subType,
                Provider = provider,
                IsFilled = isFilled,
            };

            return this.BaseDataRepository.GetDistinctManufacturerList(filter);
        }

        /// <summary>
        /// Возвращает номера партий/документов
        /// </summary>
        /// <param name="action">Параметр определяет списание или пополнение</param>
        /// <returns></returns>
        public List<string> GetDistinctConsignmentNumberList(int? action)
        {
            return this.BaseDataRepository.GetDistinctConsignmentNumberList(action);
        }

        public List<string> GetDistinctLabConsignmentNumberList()
        {
            return this.BaseDataRepository.GetDistinctLabConsignmentNumberList();
        }

        public Dictionary<int, string> GetBatchLines()
        {
            return this.BaseDataRepository.GetBatchLines();
        }

        public Dictionary<int, string> GetBatchTypes()
        {
            return this.BaseDataRepository.GetBatchTypes();
        }

        public Dictionary<int, string> GetBatches(int? statusId = null)
        {
            return this.BaseDataRepository.GetBatches(statusId);
        }
    }
}