using System;
using System.Collections.Generic;
using AppNov14.Models.Batch;

namespace AppNov14.Handlers.Result.Goods
{
    public sealed class GetViewBatchWarehouseResult
    {
        public Dictionary<int, string> BatchTypeDictionary { get; set; }

        public Dictionary<int, string> BatchLineDictionary { get; set; }

        public Dictionary<int, string> BatchNameDictionary { get; set; }

        public Dictionary<int, string> BatchStatusDictionary { get; set; }

        public List<BatchExtendedDisplayModel> Items { get; set; }
    }
}