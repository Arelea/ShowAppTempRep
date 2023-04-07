using AppNov14.Helpers;
using AppNov14.Models.ManufacturingTable;
using AppNov14.Models.Search;
using AppNov14.Repositories.Extensions;
using AppNov14.Repositories.Interfaces;
using Castle.Core.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppNov14.Repositories
{
    public class SearchRepository : BaseDataRepository, ISearchRepository
    {
        public SearchRepository(IConfiguration configuration)
           : base(configuration)
        {
        }

        public List<ManufacturingTableWriteModel> SearchLaboratory(SearchModel model)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.LaboratoryRecords.AsQueryable();

                if (model.DateStart.HasValue)
                {
                    query = query.Where(m => m.AutoDate > model.DateStart.Value);
                }
                if (model.DateFinish.HasValue)
                {
                    query = query.Where(m => m.AutoDate <= model.DateFinish.Value);
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
                if (!model.Indexation.IsNullOrEmpty())
                {
                    query = query.Where(m => m.LaboratoryIndex.Indexation == model.Indexation);
                }
                if (!model.Document.IsNullOrEmpty())
                {
                    query = query.Where(m => m.Document == model.Document);
                }
                if (!model.DocumentNumber.IsNullOrEmpty())
                {
                    query = query.Where(m => m.DocumentNumber == model.DocumentNumber);
                }
                if (model.ShowMode.HasValue && model.ShowMode.Value != RawActionTypes.All)
                {
                    query = query.Where(m => m.Action == model.ShowMode);
                }

                var result = RepositoryExtensions.JoinLaboratoryTableList(query, context);

                return result;
            }
        }
    }
}