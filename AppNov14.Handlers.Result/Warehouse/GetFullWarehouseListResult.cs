using System;
using System.Collections.Generic;
using AppNov14.Models.Warehouse;

namespace AppNov14.Handlers.Result.Warehouse
{
    public class GetFullWarehouseListResult
    {
        public List<string> TypeList { get; set; }

        public List<string> SubTypeList { get; set; }

        public List<string> ProviderList { get; set; }

        public List<string> ManufacturerList { get; set; }

        public List<IndexDisplayDataModel> Items { get; set; }
    }
}