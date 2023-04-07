using System;
using System.Collections.Generic;
using System.Web.WebPages.Html;
using AppNov14.Models.Batch;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Goods
{
    public sealed class SellBatchListViewModel : BaseViewModel
    {
        public List<SelectListItem> BatchList { get; set; } = new List<SelectListItem>();

        public SellBatchListForm Form { get; set; }

        public List<BatchDisplayModel> Items { get; set; }
    }

    public sealed class SellBatchListForm : BaseForm
    {
        public int? BatchId { get; set; }
    }
}