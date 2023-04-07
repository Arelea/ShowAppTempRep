using System;
using System.Collections.Generic;

namespace AppNov14.Models.Batch
{
    public sealed class BatchReturnDisplayModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Line { get; set; }

        public int StatusId { get; set; }

        public List<BatchQuantityModel> ValueList { get; set; }
    }

    public sealed class BatchQuantityModel 
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public List<QuantityPerCustomerModel> Params { get; set; }

        public decimal SumQuantity { get; set; }

        public int SumPackage { get; set; }

        public decimal? SumReturnQuantity { get; set; }

        public int? SumReturnPackage { get; set; }
    }

    public sealed class QuantityPerCustomerModel
    {
        public decimal Quantity { get; set; }

        public int Package { get; set; }

        public DateTime SoldDate { get; set; }
    }
}
