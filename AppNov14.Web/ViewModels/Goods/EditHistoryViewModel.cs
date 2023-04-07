using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.WebPages.Html;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Goods
{
    public sealed class EditHistoryViewModel : BaseViewModel
    {
        public EditHistoryForm Form { get; set; }

        public List<SelectListItem> CustomersList { get; set; }

        public List<SelectListItem> ParentBatchesList { get; set; }

        public string ActionName { get; set; }
    }

    public sealed class EditHistoryForm : BaseForm
    {
        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public int Package { get; set; }

        [Required]
        public int BatchId { get; set; }

        [Required]
        public int HistoryId { get; set; }

        [Required]
        public int Action { get; set; }

        [Required]
        public int? CustomerId { get; set; }

        [Required]
        public int? ParentBatchId { get; set; }
    }
}