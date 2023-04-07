using System;
using System.Collections.Generic;
using AppNov14.Models.Batch;

namespace AppNov14.Handlers.Result.Goods
{
    public sealed class GetBatchForMergeResult
    {
        public BatchExtendedDisplayModel Batch { get; set; }

        public Dictionary<int, string> ParentBatches { get; set; }
    }
}
