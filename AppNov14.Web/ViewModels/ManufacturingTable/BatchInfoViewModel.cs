using System.Web.WebPages.Html;
using System.Collections.Generic;
using AppNov14.Models.Batch;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.ManufacturingTable
{
    public sealed class BatchInfoViewModel : BaseViewModel
    {
        public int? BatchId { get; set; }

        public BatchDisplayModel BatchInformation { get; set; }

        public List<SelectListItem> BatchList { get; set; }
    }
}