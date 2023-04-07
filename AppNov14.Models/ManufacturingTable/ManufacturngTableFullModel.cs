using System;

namespace AppNov14.Models.ManufacturingTable
{
    public class ManufacturngTableFullModel 
    {
        public string Line { get; set; }

        public int WarehouseId { get; set; }

        public int IndexId { get; set; }

        public string Indexation { get; set; }

        public int Id { get; set; }

        public decimal Quantity { get; set; }

        public string Document { get; set; }

        public string DocumentNumber { get; set; }

        public DateTime DocDate { get; set; }

        public string Employee { get; set; }

        public string IpAddress { get; set; }

        public DateTime AutoDate { get; set; }

        public string Remarks { get; set; }

        public int OperationType { get; set; }
    }
}