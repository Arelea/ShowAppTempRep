using AppNov14.Models.Base;
using System;

namespace AppNov14.Models.Manufacturing
{
    public class ManufacturingSearchModel : BaseWarehouseModel
    {
        public int? Id { get; set; }

        public int? BatchTypeId { get; set; }

        public int? BatchLineId { get; set; }

        public int? BatchId { get; set; }

        public string Index { get; set; }

        public int ShowMode { get; set; }

        public int? ReplenishmentDocumentId { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateFinish { get; set; }
    }
}