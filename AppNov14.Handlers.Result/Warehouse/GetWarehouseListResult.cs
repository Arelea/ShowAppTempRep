using AppNov14.Models.Warehouse;
using System;
using System.Collections.Generic;

namespace AppNov14.Handlers.Result.Warehouse
{
    public class GetWarehouseListResult
    {
        public List<WarehouseModel> Items { get; set; }

        public List<string> DistinctTypes { get; set; }
    }
}
