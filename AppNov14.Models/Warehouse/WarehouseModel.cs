using AppNov14.Models.Base;
using System;

namespace AppNov14.Models.Warehouse
{
    public class WarehouseModel : BaseWarehouseModel
    {
        public int Id { get; set; }

        public decimal Leftovers { get; set; }

        public int Mode { get; set; }
    }
}