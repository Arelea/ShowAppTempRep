using AppNov14.Models.Manufacturing;
using System;
using System.Collections.Generic;

namespace AppNov14.Handlers.Result.Manufacturing
{
    public class EditResult
    {
        public ManufacturingRecordDisplayModel Record { get; set; }

        public List<string> TypeList { get; set; }

        public List<string> SubTypeList { get; set; }

        public List<string> ProviderList { get; set; }

        public List<string> ManufacturerList { get; set; }

        public Dictionary<int, string> IndexDictionary { get; set; }

        public Dictionary<int, string> BatchNameDictionary { get; set; }
    }
}