using System;
using System.Collections.Generic;
using AppNov14.Models.Warehouse;

namespace AppNov14.Handlers.Result.Manufacturing
{
    public sealed class IndexInfoResult
    {
        public IndexDisplayDataModel Item { get; set; }

        public Dictionary<int, string> LinkedBatches { get; set; }
    }
}