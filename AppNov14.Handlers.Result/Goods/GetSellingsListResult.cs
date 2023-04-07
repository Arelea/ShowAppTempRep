using System;
using System.Collections.Generic;
using AppNov14.Models.Batch;

namespace AppNov14.Handlers.Result.Goods
{
    public sealed class GetSellingsListResult
    {
        public Dictionary<int, string> BatchNameDictionary { get; set; }

        public Dictionary<int, string> CustomersDictionary { get; set; }

        public List<BatchActionDisplayModel> Items { get; set; }
    }
}