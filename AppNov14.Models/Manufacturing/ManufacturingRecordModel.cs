using AppNov14.Models.Base;
using System;

namespace AppNov14.Models.Manufacturing
{
    public class ManufacturingRecordModel : BaseWarehouseModel
    {
        public int? IndexId { get; set; }

        public DateTime DocDate { get; set; }

        public string Remarks { get; set; }
    }
}