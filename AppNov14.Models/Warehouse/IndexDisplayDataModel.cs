using System;
using AppNov14.Models.Base;

namespace AppNov14.Models.Warehouse
{
    public class IndexDisplayDataModel : BaseWarehouseModel
    {
        public DateTime? ExpirationDate { get; set; }

        public DateTime? ManufacturingDate { get; set; }

        public DateTime? AutoDate { get; set; }

        public decimal Leftovers { get; set; }

        public string Index { get; set; }

        public int Id { get; set; }

        public int WarehouseId { get; set; }
    }
}