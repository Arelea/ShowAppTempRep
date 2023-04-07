using System;

namespace AppNov14.Models.Batch
{
    public class BatchModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? TypeId { get; set; }

        public int LineId { get; set; }

        public int StatusId { get; set; }

        public DateTime InsertDate { get; set; }

        public DateTime? CreateDate { get; set; }
    }

    public class BatchEditModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ActionName { get; set; }

        public int Package { get; set; }

        public decimal Quantity { get; set; }

        public int? CustomerId { get; set; }

        public int? ParentBatchId { get; set; }
    }
}