using AppNov14.Models.Base;
using System;

namespace AppNov14.Models.Manufacturing
{
    public class ManufacturingRecordReplenishModel : BaseManufacturingModel
    {
        public string Index { get; set; }

        public string ReplenishmentDocument { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime? ManufacturingDate { get; set; }

        public int WarehouseId { get; set; }
    }
}