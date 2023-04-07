using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.WebPages.Html;
using AppNov14.Models.Batch;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Goods
{
    public sealed class ReturnBatchViewModel : BaseViewModel
    {
        public BatchReturnDisplayModel Item { get; set; }

        public ReturnBatchForm Form { get; set; }

        public List<SelectListItem> Customers { get; set; }
    }

    public sealed class ReturnBatchForm : BaseForm
    {

        [DisplayFormat(DataFormatString = "{0,000}", ApplyFormatInEditMode = true)]
        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public int Package { get; set; }

        [Required]
        public DateTime ReturnDate { get; set; }

        [Required]
        public int? CustomerId { get; set; }

        public string Remark { get; set; }

        [Required]
        public int Id { get; set; }
    }
}