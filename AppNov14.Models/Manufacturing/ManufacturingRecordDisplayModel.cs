using AppNov14.Models.Base;
using System;

namespace AppNov14.Models.Manufacturing
{
    public class ManufacturingRecordDisplayModel : BaseManufacturingModel
    {
        public string Index { get; set; }

        public int Id { get; set; }

        public decimal Leftovers { get; set; }

        public string BatchType { get; set; }

        public string BatchLine { get; set; }

        public string BatchName { get; set; }

        public string ReplenishmentDocument { get; set; }

        public int? IndexId { get; set; }

        public int? BatchId { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime? ManufacturingDate { get; set; }
    }

    public class ManufacturingRecordShortDisplayModel : BaseWarehouseModel
    {
        public string Index { get; set; }

        public int? IndexId { get; set; }

        public int Id { get; set; }

        public decimal Quantity { get; set; }

        public DateTime DocDate { get; set; }
    }
}