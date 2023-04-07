using System;
using System.Collections.Generic;
using AppNov14.Models.Manufacturing;

namespace AppNov14.Models.Batch
{
    public class BatchDisplayModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Line { get; set; }

        public int StatusId { get; set; }

        public string StatusCssClass { get; set; }

        public DateTime InsertDate { get; set; }

        public DateTime? CreateDate { get; set; }

        public List<ManufacturingRecordShortDisplayModel> Compounds { get; set; }
    }
}