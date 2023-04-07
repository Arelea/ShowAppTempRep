using System.Collections.Generic;
using AppNov14.Models.Batch;

namespace AppNov14.Handlers.Result.Manufacturing
{
    public sealed class BatchInfoResult
    {
        public Dictionary<int, string> BatchList { get; set; }

        public BatchDisplayModel  BatchInformation { get; set; }
    }
}