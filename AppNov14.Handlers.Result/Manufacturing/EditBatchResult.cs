using AppNov14.Models.Batch;
using System;
using System.Collections.Generic;

namespace AppNov14.Handlers.Result.Manufacturing
{
    public class EditBatchResult
    {
        public BatchModel Record { get; set; }

        public Dictionary<int, string> BatchTypeDictionary { get; set; }

        public Dictionary<int, string> BatchStatusDictionary { get; set; }

        public Dictionary<int, string> BatchLineDictionary { get; set; }
    }
}