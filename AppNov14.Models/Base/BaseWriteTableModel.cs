using System;

namespace AppNov14.Models.Base
{
    public class BaseWriteTableModel : BaseWarehouseModel
    {
        public int Id { get; set; }

        public decimal Quantity { get; set; }

        public decimal? Leftovers { get; set; }

        public string Indexation { get; set; }

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
