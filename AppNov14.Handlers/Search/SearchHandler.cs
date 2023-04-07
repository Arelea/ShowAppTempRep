using AppNov14.Handlers.Interfaces;
using AppNov14.Models.ManufacturingTable;
using AppNov14.Models.Search;
using AppNov14.Repositories.Interfaces;
using System;
using System.Collections.Generic;

namespace AppNov14.Handlers.Search
{
    public class SearchHandler : BaseDataHandler, ISearchHandler
    {
        private readonly ISearchRepository SearchRepository;

        public SearchHandler(ISearchRepository searchRepository, IBaseDataRepository baseDataRepository) : base(baseDataRepository)
        {
            SearchRepository = searchRepository;
        }

        public List<ManufacturingTableWriteModel> SearchLaboratory(SearchModel model)
        {
            return this.SearchRepository.SearchLaboratory(model);
        }
    }
}