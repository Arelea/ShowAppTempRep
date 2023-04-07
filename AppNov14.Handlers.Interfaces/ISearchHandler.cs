using AppNov14.Models.ManufacturingTable;
using AppNov14.Models.Search;
using System;
using System.Collections.Generic;

namespace AppNov14.Handlers.Interfaces
{
    public interface ISearchHandler : IBaseDataHandler
    {
        List<ManufacturingTableWriteModel> SearchLaboratory(SearchModel model);
    }
}