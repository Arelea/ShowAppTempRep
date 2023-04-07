using System;
using System.Collections.Generic;
using AppNov14.Models.Batch;

namespace AppNov14.Handlers.Result.Goods
{
    public sealed class GetBatchForReturnResult
    {
        public BatchReturnDisplayModel Batch { get; set; }

        public Dictionary<int, string> Customers { get; set; }
    }
}