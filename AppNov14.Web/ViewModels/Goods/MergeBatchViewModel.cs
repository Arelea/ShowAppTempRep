using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.WebPages.Html;
using AppNov14.Models.Batch;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Goods
{
    public sealed class MergeBatchViewModel : BaseViewModel
    {
        public BatchExtendedDisplayModel Item { get; set; }

        public MergeBatchForm Form { get; set; }

        public List<SelectListItem> ParentBatches { get; set; }
    }

    public sealed class MergeBatchForm : BaseForm
    {

        [DisplayFormat(DataFormatString = "{0,000}", ApplyFormatInEditMode = true)]
        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public int Package { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int ParentBatchId { get; set; }

        public string Remark { get; set; }

        [Required]
        public int Id { get; set; }
    }
}