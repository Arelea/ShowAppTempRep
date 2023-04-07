using AppNov14.Models.Manufacturing;
using System;
using System.Collections.Generic;

namespace AppNov14.Handlers.Result.Manufacturing
{
    public class OutcomingOrderResult
    {
        public List<string> TypeList { get; set; }

        public Dictionary<int, string> BatchLineDictionary { get; set; }

        public Dictionary<int, string> BatchTypeDictionary { get; set; }

        public Dictionary<int, string> BatchDictionary { get; set; }

        public ManufacturingRecordModel Item { get; set; }

        public List<string> SubTypeList { get; set; }

        public List<string> ProviderList { get; set; }

        public List<string> ManufacturerList { get; set; }

        public Dictionary<int, string> IndexDictionary { get; set; }
    }
}