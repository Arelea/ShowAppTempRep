using AppNov14.Models.Base;
using AppNov14.Models.Batch;
using System;

namespace AppNov14.Models.Manufacturing
{
    public class ManufacturingRecordWriteOffModel : BaseManufacturingModel
    {
        public int IndexId { get; set; }

        public int WarehouseId { get; set; }

        public int BatchId { get; set; }

        public bool IsNewBatch { get; set; }

        public BatchModel Batch { get; set; }
    }
}