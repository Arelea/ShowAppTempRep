using System;
using System.Collections.Generic;
using AppNov14.Models.Batch;

namespace AppNov14.Handlers.Result.Goods
{
    public sealed class GetBatchForEditHistoryResult
    {
        public BatchEditModel Batch { get; set; }

        public Dictionary<int, string> Customers { get; set; }

        public Dictionary<int, string> ParentBatches { get; set; }
    }
}