using AppNov14.Models.Base;
using System;

namespace AppNov14.Models.Search
{
    public class SearchModel : BaseWarehouseModel
    {
        public int? Id { get; set; }

        public string Document { get; set; }

        public string Line { get; set; }

        public string DocumentNumber { get; set; }

        public string Indexation { get; set; }

        public int? ShowMode { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateFinish { get; set; }
    }
}