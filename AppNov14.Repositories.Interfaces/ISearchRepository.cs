using AppNov14.Models.ManufacturingTable;
using AppNov14.Models.Search;
using System;
using System.Collections.Generic;

namespace AppNov14.Repositories.Interfaces
{
    public interface ISearchRepository
    {
        List<ManufacturingTableWriteModel> SearchLaboratory(SearchModel model);
    }
}