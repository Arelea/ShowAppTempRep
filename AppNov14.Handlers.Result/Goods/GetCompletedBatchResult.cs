using System;
using AppNov14.Models.Batch;

namespace AppNov14.Handlers.Result.Goods
{
    public sealed class GetCompletedBatchResult
    {
        public BatchDisplayModel Batch { get; set; }
    }
}