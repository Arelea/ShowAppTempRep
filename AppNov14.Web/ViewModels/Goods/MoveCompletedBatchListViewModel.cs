using AppNov14.Web.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Web.WebPages.Html;
using AppNov14.Helpers;
using AppNov14.Models.Batch;

namespace AppNov14.Web.ViewModels.Goods
{
    public sealed class MoveCompletedBatchListViewModel : BaseViewModel
    {
        public List<SelectListItem> BatchList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> BatchStatusList { get; set; } = new List<SelectListItem>();

        public MoveCompletedBatchListForm Form { get; set; }

        public List<BatchDisplayModel> Items { get; set; }
    }

    public sealed class MoveCompletedBatchListForm : BaseForm
    {
        public int? BatchId { get; set; }

        public int BatchStatusId { get; set; } = BatchStatuses.InManufacturingProcess;
    }
}