using AppNov14.Models.Batch;
using System;
using System.Collections.Generic;

namespace AppNov14.Handlers.Result.Manufacturing
{
    public class EditBatchListResult
    {
        public Dictionary<int, string> BatchDictionary { get; set; }

        public Dictionary<int, string> BatchStatusDictionary { get; set; }

        public List<BatchDisplayModel> Items { get; set; }
    }
}