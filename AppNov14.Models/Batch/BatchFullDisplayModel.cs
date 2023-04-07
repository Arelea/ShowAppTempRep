using System;
using System.Collections.Generic;

namespace AppNov14.Models.Batch
{
    public class BatchExtendedDisplayModel : BatchDisplayModel
    {
        public int? CurrentPackage { get; set; }

        public decimal? CurrentQuantity { get; set; }

        public DateTime? CompletionDate { get; set; }

        public decimal? InitialQuantity { get; set; }

        public int? InitialPackage { get; set; }
    }

    public class BatchFullDisplayModel : BatchExtendedDisplayModel
    {
        public List<BatchHistoriesDisplayModel> BatchHistories { get; set; }

        public List<BatchDisplayModel> ChildBatches { get; set; }

        public List<BatchDisplayModel> ParentBatches { get; set; }
    }

    public class BatchHistoriesDisplayModel
    {
        public int Id { get; set; }

        public int BatchId { get; set; }

        public int ActionTypeId { get; set; }

        public int OperationTypeId { get; set; }

        public string ActionTypeName { get; set; }

        public string OperationTypeName { get; set; }

        public DateTime InsertDate { get; set; }

        public string Text { get; set; }

        public int? CustomerId { get; set; }

        public string CustomerName { get; set; }

        public decimal? Quantity { get; set; }

        public int? Package { get; set; }

        public decimal? LeftQuantity { get; set; }

        public int? LeftPackage { get; set; }

        public DateTime? SoldDate { get; set; }

        public string Remark { get; set; }

        public string OperationCssClass { get; set; }

        public string ActionCssClass { get; set; }

        public int? LinkedBatchId { get; set; }

        public string LinkedBatchName { get; set; }

        public DateTime? ReturnDate { get; set; }
    }

    public class TempBatchReturnModel
    {
        public decimal SumQuantity { get; set; }

        public int SumPackage { get; set; }

        public DateTime? LastSoldDate { get; set; }
    }
}
