using AppNov14.Models.Warehouse;
using System;
using System.Collections.Generic;

namespace AppNov14.Handlers.Result.Manufacturing
{
    public class ViewIndexResult
    {
        public List<string> TypeList { get; set; }

        public List<string> SubTypeList { get; set; }

        public List<string> ProviderList { get; set; }

        public List<string> ManufacturerList { get; set; }

        public List<IndexDisplayDataModel> Items { get; set; }
    }
}