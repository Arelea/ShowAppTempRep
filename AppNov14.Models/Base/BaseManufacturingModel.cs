using System;

namespace AppNov14.Models.Base
{
    public class BaseManufacturingModel : BaseWarehouseModel
    {
        public decimal Quantity { get; set; }

        public int ActionType { get; set; }

        public DateTime InsertDate { get; set; }

        public DateTime DocDate { get; set; }

        public string Remarks { get; set; }

        public string IpAddress { get; set; }

        public string Employee { get; set; }
    }
}