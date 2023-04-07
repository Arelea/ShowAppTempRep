using AppNov14.Helpers;
using AppNov14.Models.Batch;
using AppNov14.Web.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Web.WebPages.Html;

namespace AppNov14.Web.ViewModels.ManufacturingTable
{
    public class EditBatchListViewModel : BaseViewModel
    {
        public EditBatchListForm Form { get; set; }

        public List<BatchDisplayModel> Items { get; set; }

        public List<SelectListItem> BatchList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> BatchStatusList { get; set; }
    }

    public sealed class EditBatchListForm : BaseForm
    {
        public int? BatchId { get; set; }

        public int BatchStatusId { get; set; } = BatchStatuses.InManufacturingProcess;
    }
}