using System;
using System.Collections.Generic;
using System.Web.WebPages.Html;
using AppNov14.Models.Batch;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Goods
{
    public sealed class ViewBatchWarehouseViewModel : BaseViewModel
    {
        public ViewBatchWarehouseForm Form { get; set; }

        public List<SelectListItem> BatchTypeList { get; set; }

        public List<SelectListItem> BatchNameList { get; set; }

        public List<SelectListItem> BatchLineList { get; set; }

        public List<SelectListItem> BatchStatusList { get; set; }

        public List<BatchExtendedDisplayModel> Items { get; set; }
    }

    public sealed class ViewBatchWarehouseForm : BaseForm
    {
        public string Query { get; set; }

        public int? BatchId { get; set; }

        public int? TypeId { get; set; }

        public int? LineId { get; set; }

        public int? StatusId { get; set; }
    }
}
