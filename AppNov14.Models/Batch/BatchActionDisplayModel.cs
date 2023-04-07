using System;

namespace AppNov14.Models.Batch
{
    public class BatchActionDisplayModel
    {
        public int HistoryId { get; set; }

        public int BatchId { get; set; }

        public string TypeName { get; set; }

        public string BatchName { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public decimal Quantity { get; set; }

        public int Package { get; set; }

        public decimal LeftQuantity { get; set; }

        public int LeftPackage { get; set; }

        public DateTime SoldDate { get; set; }
}
}
