using System;
using System.Collections.Generic;
using AppNov14.Models.Batch;

namespace AppNov14.Handlers.Result.Goods
{
    public class GetBatchListResult
    {
        public Dictionary<int, string> BatchDictionary { get; set; }

        public Dictionary<int, string> BatchStatusDictionary { get; set; }

        public List<BatchDisplayModel> Items { get; set; }
    }
}